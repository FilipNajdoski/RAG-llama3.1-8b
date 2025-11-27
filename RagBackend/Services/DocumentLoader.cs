using RagBackend.Models;
using RagBackend.Utils;

namespace RagBackend.Services;

public class DocumentLoader
{
    private readonly string _documentsPath;

    public DocumentLoader()
    {
        _documentsPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Documents");

        if (!Directory.Exists(_documentsPath))
            Directory.CreateDirectory(_documentsPath);
    }

    public List<DocumentRecord> LoadAllDocuments()
    {
        var results = new List<DocumentRecord>();

        var files = Directory.GetFiles(_documentsPath);

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file).ToLower();
            string content = string.Empty;

            switch (ext)
            {
                case ".txt":
                case ".md":
                    content = File.ReadAllText(file);
                    break;

                case ".pdf":
                    content = PdfHelper.ExtractText(file);
                    break;

                default:
                    continue; // unsupported format
            }

            results.Add(new DocumentRecord
            {
                FileName = Path.GetFileName(file),
                Content = content,
                Extension = ext
            });
        }

        return results;
    }
}
