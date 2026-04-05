namespace IssueGPT.Api.Services;

public class CopilotPromptService
{
    private readonly ILogger<CopilotPromptService> _logger;

    public CopilotPromptService(ILogger<CopilotPromptService> logger)
    {
        _logger = logger;
    }

    public List<CopilotPromptItem> GeneratePrompts(string issueTitle, string executionPlan, string taskBreakdown, string issueType)
    {
        var prompts = new List<CopilotPromptItem>();

        prompts.Add(GenerateUnderstandCodePrompt(issueTitle, executionPlan));
        prompts.Add(GenerateImplementationPrompt(issueTitle, taskBreakdown));
        prompts.Add(GenerateTestingPrompt(issueTitle, taskBreakdown));
        prompts.Add(GenerateDbMigrationPrompt(issueTitle, executionPlan));
        prompts.Add(GeneratePreventHallucinationPrompt(issueTitle, executionPlan));

        return prompts;
    }

    private CopilotPromptItem GenerateUnderstandCodePrompt(string issueTitle, string executionPlan)
    {
        return new CopilotPromptItem
        {
            PromptType = "understand-code",
            Content = $@"GitHub Issue: {issueTitle}

Execution Plan:
{executionPlan}

Please:
1. Identify the modules and components in the codebase most relevant to this issue
2. Explain the current execution flow
3. Point out where changes should be implemented
4. List any related files or patterns I should be aware of

Focus on understanding the current architecture before proposing changes."
        };
    }

    private CopilotPromptItem GenerateImplementationPrompt(string issueTitle, string taskBreakdown)
    {
        return new CopilotPromptItem
        {
            PromptType = "implement",
            Content = $@"GitHub Issue: {issueTitle}

Task Breakdown:
{taskBreakdown}

Please:
1. Implement the requested changes following existing code conventions
2. Keep the scope focused on this issue - avoid unrelated refactors
3. Explain what each modified file does
4. Ensure backward compatibility where applicable

Provide the implementation changes needed to resolve this issue."
        };
    }

    private CopilotPromptItem GenerateTestingPrompt(string issueTitle, string taskBreakdown)
    {
        return new CopilotPromptItem
        {
            PromptType = "testing",
            Content = $@"GitHub Issue: {issueTitle}

Changes Needed:
{taskBreakdown}

Please:
1. Add unit tests covering the main functionality
2. Add integration tests where applicable
3. Include edge cases and error scenarios
4. Consider regression risks from this change

Provide comprehensive tests to validate this fix/feature."
        };
    }

    private CopilotPromptItem GenerateDbMigrationPrompt(string issueTitle, string executionPlan)
    {
        return new CopilotPromptItem
        {
            PromptType = "db-migration",
            Content = $@"GitHub Issue: {issueTitle}

Execution Plan:
{executionPlan}

Please:
1. Determine if database schema changes are needed
2. If needed, generate EF Core model updates
3. Create migration suggestions (safe, idempotent)
4. Consider data migration if necessary

Provide any database/migration changes required for this issue."
        };
    }

    private CopilotPromptItem GeneratePreventHallucinationPrompt(string issueTitle, string executionPlan)
    {
        return new CopilotPromptItem
        {
            PromptType = "prevent-hallucination",
            Content = $@"GitHub Issue: {issueTitle}

Before implementing:
1. What are your assumptions about the current code structure?
2. What information might be missing or unclear?
3. Which parts of the specification need clarification?
4. What edge cases could cause issues?

Ask clarifying questions if needed, and state your assumptions explicitly before writing code."
        };
    }
}

public class CopilotPromptItem
{
    public string PromptType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
