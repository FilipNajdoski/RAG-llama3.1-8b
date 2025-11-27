using UglyToad.PdfPig;

namespace RagBackend.Utils;

public static class PdfHelper
{
    public static string ExtractText(string filePath)
    {
        using PdfDocument document = PdfDocument.Open(filePath);
        var text = "";

        foreach (var page in document.GetPages())
        {
            text += page.Text;
            text += "\n";
        }

        return text;
    }
}
