using Octokit;

namespace IssueGPT.Api.Services;

public class GitHubService
{
    private readonly GitHubClient _client;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(IConfiguration configuration, ILogger<GitHubService> logger)
    {
        _logger = logger;
        var token = configuration["GITHUB_TOKEN"];
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("GITHUB_TOKEN environment variable is not set");
        }

        _client = new GitHubClient(new ProductHeaderValue("IssueGPT"))
        {
            Credentials = new Credentials(token)
        };
    }

    public async Task<GitHubIssueData> FetchIssueAsync(string owner, string repo, int issueNumber)
    {
        try
        {
            var issue = await _client.Issue.Get(owner, repo, issueNumber);
            var comments = await _client.Issue.Comment.GetAll(owner, repo, issueNumber);

            var labels = issue.Labels.Select(l => l.Name).ToList();

            return new GitHubIssueData
            {
                IssueNumber = issue.Number,
                Title = issue.Title,
                Body = issue.Body ?? string.Empty,
                State = issue.State.StringValue,
                Author = issue.User.Login,
                Labels = labels,
                Assignee = issue.Assignee?.Login,
                Url = issue.HtmlUrl,
                Comments = comments.Select(c => new GitHubCommentData
                {
                    CommentId = c.Id,
                    Author = c.User.Login,
                    Body = c.Body,
                    CreatedAt = c.CreatedAt.DateTime
                }).ToList(),
                CreatedAt = issue.CreatedAt.DateTime,
                UpdatedAt = issue.UpdatedAt.DateTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching GitHub issue {owner}/{repo}#{issueNumber}");
            throw;
        }
    }
}

public class GitHubIssueData
{
    public int IssueNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = new();
    public string? Assignee { get; set; }
    public string Url { get; set; } = string.Empty;
    public List<GitHubCommentData> Comments { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GitHubCommentData
{
    public long CommentId { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
