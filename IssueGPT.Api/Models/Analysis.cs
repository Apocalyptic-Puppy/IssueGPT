namespace IssueGPT.Api.Models;

public class Analysis
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string IssueType { get; set; } = string.Empty; // bug, feature, refactor, tech-debt, unclear
    public string ExecutionPlan { get; set; } = string.Empty;
    public string Risks { get; set; } = string.Empty;
    public string ClarifyingQuestions { get; set; } = string.Empty;
    public string TaskBreakdown { get; set; } = string.Empty;
    public string RawLlmResponse { get; set; } = string.Empty;
    public string ModelName { get; set; } = "gpt-4-turbo";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Issue Issue { get; set; } = null!;
    public ICollection<CopilotPrompt> CopilotPrompts { get; set; } = new List<CopilotPrompt>();
}
