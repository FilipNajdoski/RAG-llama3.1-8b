using RagBackend.Models;
using System.Text.RegularExpressions;

namespace RagBackend.Services;

public class TextSplitter
{
    public List<Chunk> SplitDocument(string documentName, string text)
    {
        var chunks = new List<Chunk>();

        // Simple sentence splitter (splits on . ! ? followed by space or newline)
        var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+")
                             .Where(s => !string.IsNullOrWhiteSpace(s))
                             .ToArray();

        int id = 0;
        foreach (var sentence in sentences)
        {
            chunks.Add(new Chunk
            {
                Id = id,
                DocumentName = documentName,
                Text = sentence.Trim()
            });
            id++;
        }

        return chunks;
    }
}
