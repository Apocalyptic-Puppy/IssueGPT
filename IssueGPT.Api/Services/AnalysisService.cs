using System.Text.Json;
using OpenAI;
using OpenAI.Chat;

namespace IssueGPT.Api.Services;

public class AnalysisService
{
    private readonly OpenAIClient _client;
    private readonly ILogger<AnalysisService> _logger;
    private readonly string _model;

    public AnalysisService(IConfiguration configuration, ILogger<AnalysisService> logger)
    {
        _logger = logger;
        var apiKey = configuration["OPENAI_API_KEY"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");
        }

        _model = configuration["OPENAI_MODEL"] ?? "gpt-4-turbo";
        _client = new OpenAIClient(apiKey);
    }

    public async Task<AnalysisResult> AnalyzeIssueAsync(string issueTitle, string issueBody, List<string> comments, List<string> labels)
    {
        try
        {
            var prompt = BuildAnalysisPrompt(issueTitle, issueBody, comments, labels);

            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompt)
            };

            var chatCompletion = await _client.GetChatCompletionsAsync(
                new ChatCompletionOptions
                {
                    Model = _model,
                    Messages = messages,
                    Temperature = 0.7f,
                    MaxTokens = 2000,
                    ResponseFormat = ChatCompletionResponseFormat.JsonObject
                });

            var responseContent = chatCompletion.Value.Choices[0].Message.Content[0].Text;

            var analysisResult = JsonSerializer.Deserialize<AnalysisResult>(responseContent);
            analysisResult!.RawLlmResponse = responseContent;

            return analysisResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing issue with LLM");
            throw;
        }
    }

    private string BuildAnalysisPrompt(string issueTitle, string issueBody, List<string> comments, List<string> labels)
    {
        var commentsText = string.Join("\n---\n", comments.Take(5)); // Limit to first 5 comments

        return $@"You are an expert GitHub issue analyst. Analyze the following GitHub issue and provide structured insights.

Issue Title: {issueTitle}

Issue Body:
{issueBody}

Related Comments (first 5):
{commentsText}

Labels: {string.Join(", ", labels)}

Analyze this issue and respond ONLY with valid JSON in this exact format:
{{
  ""summary"": ""2-3 sentence clear summary of what needs to be done"",
  ""issueType"": ""bug|feature|refactor|tech-debt|unclear"",
  ""executionPlan"": ""Step-by-step execution plan as bullet points"",
  ""risks"": ""Potential risks and side effects"",
  ""clarifyingQuestions"": ""List of questions to clarify requirements"",
  ""taskBreakdown"": ""Decomposed subtasks as bullet points""
}}

Focus on practical, actionable insights. Be concise.";
    }
}

public class AnalysisResult
{
    public string Summary { get; set; } = string.Empty;
    public string IssueType { get; set; } = string.Empty;
    public string ExecutionPlan { get; set; } = string.Empty;
    public string Risks { get; set; } = string.Empty;
    public string ClarifyingQuestions { get; set; } = string.Empty;
    public string TaskBreakdown { get; set; } = string.Empty;
    public string RawLlmResponse { get; set; } = string.Empty;
}
