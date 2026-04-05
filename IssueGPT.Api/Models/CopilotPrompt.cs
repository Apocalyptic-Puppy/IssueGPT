namespace IssueGPT.Api.Models;

public class CopilotPrompt
{
    public int Id { get; set; }
    public int AnalysisId { get; set; }
    public string PromptType { get; set; } = string.Empty; // understand-code, implement, testing, db-migration, prevent-hallucination
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Analysis Analysis { get; set; } = null!;
}
