using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using IssueGPT.Api.Data;
using IssueGPT.Api.Models;
using IssueGPT.Api.Services;

namespace IssueGPT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly GitHubService _githubService;
    private readonly AnalysisService _analysisService;
    private readonly CopilotPromptService _copilotPromptService;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<IssuesController> _logger;

    public IssuesController(
        GitHubService githubService,
        AnalysisService analysisService,
        CopilotPromptService copilotPromptService,
        AppDbContext dbContext,
        ILogger<IssuesController> logger)
    {
        _githubService = githubService;
        _analysisService = analysisService;
        _copilotPromptService = copilotPromptService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeIssue([FromBody] AnalyzeIssueRequest request)
    {
        try
        {
            _logger.LogInformation($"Analyzing GitHub issue: {request.Owner}/{request.Repo}#{request.IssueNumber}");

            // Parse repository info
            var owner = request.Owner;
            var repo = request.Repo;
            var issueNumber = request.IssueNumber;

            // Fetch or create repository
            var repository = await _dbContext.Repositories.FirstOrDefaultAsync(r =>
                r.Owner == owner && r.Name == repo);

            if (repository == null)
            {
                repository = new Repository
                {
                    Owner = owner,
                    Name = repo,
                    Url = $"https://github.com/{owner}/{repo}"
                };
                _dbContext.Repositories.Add(repository);
                await _dbContext.SaveChangesAsync();
            }

            // Fetch GitHub issue data
            var githubIssue = await _githubService.FetchIssueAsync(owner, repo, issueNumber);

            // Store or update issue in DB
            var issue = await _dbContext.Issues.FirstOrDefaultAsync(i =>
                i.RepositoryId == repository.Id && i.GitHubIssueNumber == issueNumber);

            if (issue == null)
            {
                issue = new Issue
                {
                    RepositoryId = repository.Id,
                    GitHubIssueNumber = issueNumber,
                    Title = githubIssue.Title,
                    Body = githubIssue.Body,
                    State = githubIssue.State,
                    Author = githubIssue.Author,
                    LabelsJson = JsonSerializer.Serialize(githubIssue.Labels),
                    Assignee = githubIssue.Assignee,
                    GitHubUrl = githubIssue.Url,
                    SyncedAt = DateTime.UtcNow
                };
                _dbContext.Issues.Add(issue);
                await _dbContext.SaveChangesAsync();

                // Store comments
                foreach (var comment in githubIssue.Comments)
                {
                    var issueComment = new IssueComment
                    {
                        IssueId = issue.Id,
                        GitHubCommentId = comment.CommentId,
                        Author = comment.Author,
                        Body = comment.Body,
                        CreatedAt = comment.CreatedAt
                    };
                    _dbContext.IssueComments.Add(issueComment);
                }
                await _dbContext.SaveChangesAsync();
            }

            // Analyze issue with LLM
            var commentBodies = githubIssue.Comments.Select(c => $"{c.Author}: {c.Body}").ToList();
            var analysisResult = await _analysisService.AnalyzeIssueAsync(
                githubIssue.Title,
                githubIssue.Body,
                commentBodies,
                githubIssue.Labels);

            // Store analysis
            var analysis = new Analysis
            {
                IssueId = issue.Id,
                Summary = analysisResult.Summary,
                IssueType = analysisResult.IssueType,
                ExecutionPlan = analysisResult.ExecutionPlan,
                Risks = analysisResult.Risks,
                ClarifyingQuestions = analysisResult.ClarifyingQuestions,
                TaskBreakdown = analysisResult.TaskBreakdown,
                RawLlmResponse = analysisResult.RawLlmResponse,
                ModelName = "gpt-4-turbo"
            };
            _dbContext.Analyses.Add(analysis);
            await _dbContext.SaveChangesAsync();

            // Generate Copilot prompts
            var prompts = _copilotPromptService.GeneratePrompts(
                githubIssue.Title,
                analysisResult.ExecutionPlan,
                analysisResult.TaskBreakdown,
                analysisResult.IssueType);

            foreach (var prompt in prompts)
            {
                var copilotPrompt = new CopilotPrompt
                {
                    AnalysisId = analysis.Id,
                    PromptType = prompt.PromptType,
                    Content = prompt.Content
                };
                _dbContext.CopilotPrompts.Add(copilotPrompt);
            }
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Successfully analyzed issue: {issue.Id}");

            return Ok(new
            {
                issueId = issue.Id,
                analysisId = analysis.Id,
                summary = analysis.Summary,
                issueType = analysis.IssueType,
                executionPlan = analysis.ExecutionPlan,
                risks = analysis.Risks,
                clarifyingQuestions = analysis.ClarifyingQuestions,
                taskBreakdown = analysis.TaskBreakdown,
                copilotPrompts = prompts.Select(p => new
                {
                    type = p.PromptType,
                    content = p.Content
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing GitHub issue");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{issueId}")]
    public async Task<IActionResult> GetAnalysis(int issueId)
    {
        try
        {
            var issue = await _dbContext.Issues
                .Include(i => i.Analyses)
                .ThenInclude(a => a.CopilotPrompts)
                .FirstOrDefaultAsync(i => i.Id == issueId);

            if (issue == null)
                return NotFound();

            var latestAnalysis = issue.Analyses.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
            if (latestAnalysis == null)
                return NotFound();

            return Ok(new
            {
                issueId = issue.Id,
                title = issue.Title,
                url = issue.GitHubUrl,
                analysisId = latestAnalysis.Id,
                summary = latestAnalysis.Summary,
                issueType = latestAnalysis.IssueType,
                executionPlan = latestAnalysis.ExecutionPlan,
                risks = latestAnalysis.Risks,
                clarifyingQuestions = latestAnalysis.ClarifyingQuestions,
                taskBreakdown = latestAnalysis.TaskBreakdown,
                copilotPrompts = latestAnalysis.CopilotPrompts.Select(p => new
                {
                    type = p.PromptType,
                    content = p.Content
                }).ToList(),
                createdAt = latestAnalysis.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching analysis");
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class AnalyzeIssueRequest
{
    public string Owner { get; set; } = string.Empty;
    public string Repo { get; set; } = string.Empty;
    public int IssueNumber { get; set; }
}
