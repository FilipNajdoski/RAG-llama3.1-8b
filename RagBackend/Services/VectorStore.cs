using RagBackend.Models;

namespace RagBackend.Services;

public class VectorStore
{
    private readonly List<VectorRecord> _vectors = new List<VectorRecord>();

    public void AddVector(Chunk chunk, float[] embedding)
    {
        _vectors.Add(new VectorRecord
        {
            Chunk = chunk,
            Embedding = embedding
        });
    }

    public List<(Chunk, float)> Search(float[] queryEmbedding, int topK = 3)
    {
        // Cosine similarity
        var results = new List<(Chunk, float)>();

        foreach (var record in _vectors)
        {
            float score = CosineSimilarity(queryEmbedding, record.Embedding);
            results.Add((record.Chunk, score));
        }

        return results
            .OrderByDescending(r => r.Item2)
            .Take(topK)
            .ToList();
    }

    private float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length) return 0;

        float dot = 0;
        float magA = 0;
        float magB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }

        if (magA == 0 || magB == 0) return 0;

        return dot / ((float)Math.Sqrt(magA) * (float)Math.Sqrt(magB));
    }
}

public class VectorRecord
{
    public Chunk Chunk { get; set; } = new Chunk();
    public float[] Embedding { get; set; } = Array.Empty<float>();
}
