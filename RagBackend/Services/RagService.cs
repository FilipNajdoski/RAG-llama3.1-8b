using RagBackend.Models;
using System.Diagnostics;

namespace RagBackend.Services;

public class RagService
{
    private readonly EmbeddingService _embeddingService;
    private readonly VectorStore _vectorStore;

    public RagService(EmbeddingService embeddingService, VectorStore vectorStore)
    {
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
    }
    public void AddChunk(Chunk chunk)
    {
        if (chunk == null || string.IsNullOrWhiteSpace(chunk.Text))
            return;

        // Generate embedding for the chunk
        var embedding = _embeddingService.GetEmbedding(chunk.Text);

        if (embedding.Length > 0)
            _vectorStore.AddVector(chunk, embedding);
    }
    // Retrieve top-k relevant chunks
    public List<Chunk> RetrieveTopChunks(string question, int topK = 3)
    {
        var queryEmbedding = _embeddingService.GetEmbedding(question);
        if (queryEmbedding.Length == 0) return new List<Chunk>();

        return _vectorStore.Search(queryEmbedding, topK)
                           .Select(r => r.Item1)
                           .ToList();
    }

    // Async: Generate concise answer using Ollama
    public async Task<string> GenerateAnswerWithLLMAsync(string question, List<Chunk> chunks, string modelName = "llama3.1:8b")
    {
        if (chunks == null || chunks.Count == 0)
            return "No relevant context found.";

        // Join all top-k chunks into a single context string
        string context = string.Join("\n", chunks.Select(c => c.Text));

        string prompt = $@"You are an assistant. Using the following context, answer the question concisely:

Context:
{context}

Question: {question}
Answer:";

        var psi = new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = $"run {modelName}",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };

        try
        {
            process.Start();

            // Write prompt asynchronously
            await process.StandardInput.WriteLineAsync(prompt);
            process.StandardInput.Close(); // Important to signal EOF

            // Read stdout and stderr asynchronously
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);

            process.WaitForExit();

            string output = outputTask.Result.Trim();
            string error = errorTask.Result.Trim();

            if (!string.IsNullOrWhiteSpace(error))
                Console.WriteLine("Ollama error: " + error);

            return output;
        }
        catch (Exception ex)
        {
            Console.WriteLine("LLM integration exception: " + ex.Message);
            return "Error: could not generate answer.";
        }
    }
}
