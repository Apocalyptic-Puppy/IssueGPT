namespace IssueGPT.Api.Models;

public class Issue
{
    public int Id { get; set; }
    public int RepositoryId { get; set; }
    public long GitHubIssueNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string State { get; set; } = "open"; // open, closed
    public string Author { get; set; } = string.Empty;
    public string? LabelsJson { get; set; } // JSON array
    public string? Assignee { get; set; }
    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SyncedAt { get; set; }

    public Repository Repository { get; set; } = null!;
    public ICollection<IssueComment> Comments { get; set; } = new List<IssueComment>();
    public ICollection<Analysis> Analyses { get; set; } = new List<Analysis>();
}
