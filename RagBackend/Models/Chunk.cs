namespace RagBackend.Models;

public class Chunk
{
    public int Id { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
