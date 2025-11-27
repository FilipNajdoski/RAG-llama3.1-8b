namespace RagBackend.Models;

public class QueryResponse
{
    public string Question { get; set; } = string.Empty;
    public List<Answer> Answers { get; set; } = new List<Answer>();
}

public class Answer
{
    public string DocumentName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public float Score { get; set; }
}
