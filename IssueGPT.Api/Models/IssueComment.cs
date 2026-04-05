namespace IssueGPT.Api.Models;

public class IssueComment
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public long GitHubCommentId { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Issue Issue { get; set; } = null!;
}
