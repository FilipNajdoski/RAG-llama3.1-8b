using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RagBackend.Services;

public class EmbeddingService
{
    public float[] GetEmbedding(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<float>();

        var psi = new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = "run nomic-embed-text",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return Array.Empty<float>();

        // Feed text via stdin
        process.StandardInput.WriteLine(text);
        process.StandardInput.Close();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Console.WriteLine("Ollama output: " + output);
        if (!string.IsNullOrWhiteSpace(error))
            Console.WriteLine("Ollama error: " + error);

        if (string.IsNullOrWhiteSpace(output)) return Array.Empty<float>();

        // Parse embedding numbers (space or comma separated)
        try
        {
            var numbers = Regex.Matches(output, @"-?\d+(\.\d+)?")
                               .Select(m => float.Parse(m.Value))
                               .ToArray();
            return numbers;
        }
        catch
        {
            return Array.Empty<float>();
        }
    }
}
