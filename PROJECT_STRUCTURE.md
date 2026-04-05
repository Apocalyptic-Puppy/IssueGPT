# Project Structure

```
IssueGPT/
│
├── README.md                 # Product description & features
├── QUICKSTART.md            # 5-minute setup
├── SETUP.md                 # Detailed setup guide
├── docker-compose.yml       # MSSQL container config
├── .env.example             # Environment variables template
├── .gitignore               # Git exclusions
│
├── IssueGPT.Api/            # ASP.NET Core Backend
│   ├── IssueGPT.Api.csproj
│   ├── Program.cs           # Startup & DI configuration
│   │
│   ├── Controllers/
│   │   └── IssuesController.cs      # POST /analyze, GET /{id}
│   │
│   ├── Services/
│   │   ├── GitHubService.cs         # Fetch issues via GitHub API
│   │   ├── AnalysisService.cs       # LLM analysis via OpenAI
│   │   └── CopilotPromptService.cs  # Generate 5 prompt types
│   │
│   ├── Models/
│   │   ├── Repository.cs            # GitHub repo reference
│   │   ├── Issue.cs                 # GitHub issue entity
│   │   ├── IssueComment.cs          # Issue comment entity
│   │   ├── Analysis.cs              # LLM analysis result
│   │   └── CopilotPrompt.cs         # Generated prompt
│   │
│   └── Data/
│       └── AppDbContext.cs          # EF Core DbContext
│
├── IssueGPT.Frontend/       # Web UI
│   └── index.html           # Single-page frontend (HTML/CSS/JS)
│
└── IssueGPT.Tests/          # Unit & Integration Tests (future)
    └── [Test files]
```

---

## Data Flow

```
1. User Input (Web UI)
   ↓
2. POST /api/issues/analyze
   ↓
3. GitHubService.FetchIssueAsync()
   ├── Fetch issue metadata
   ├── Fetch comments
   └── Store to MSSQL
   ↓
4. AnalysisService.AnalyzeIssueAsync()
   ├── Build LLM prompt
   ├── Call OpenAI API
   ├── Parse JSON response
   └── Store to MSSQL
   ↓
5. CopilotPromptService.GeneratePrompts()
   ├── Generate 5 prompt templates
   └── Store to MSSQL
   ↓
6. Return JSON response
   ↓
7. Frontend displays:
   - Summary
   - Execution Plan
   - Risks
   - Task Breakdown
   - 5 Copilot Prompts
```

---

## Key Technologies

| Layer      | Tech                  |
| ---------- | --------------------- |
| API        | ASP.NET Core 8.0      |
| ORM        | Entity Framework Core |
| Database   | MSSQL Server          |
| GitHub API | Octokit .NET          |
| LLM API    | OpenAI .NET SDK       |
| Frontend   | HTML/CSS/JavaScript   |
| Container  | Docker                |

---

## Database Schema

### Repositories

- Id (PK)
- Owner, Name, Url
- CreatedAt

### Issues

- Id (PK)
- RepositoryId (FK)
- GitHubIssueNumber
- Title, Body, State
- Author, Labels, Assignee, Url
- CreatedAt, UpdatedAt, SyncedAt

### IssueComments

- Id (PK)
- IssueId (FK)
- GitHubCommentId
- Author, Body
- CreatedAt, UpdatedAt

### Analyses

- Id (PK)
- IssueId (FK)
- Summary, IssueType
- ExecutionPlan, Risks
- ClarifyingQuestions, TaskBreakdown
- RawLlmResponse, ModelName
- CreatedAt, UpdatedAt

### CopilotPrompts

- Id (PK)
- AnalysisId (FK)
- PromptType (understand-code|implement|testing|db-migration|prevent-hallucination)
- Content
- CreatedAt

---

## Environment Variables

Required:

- `GITHUB_TOKEN` - Personal Access Token with repo access
- `OPENAI_API_KEY` - OpenAI API key for GPT-4 access

Database:

- `DB_SERVER` - localhost
- `DB_PORT` - 1433
- `DB_USER` - sa
- `DB_PASSWORD` - IssueGPT@2026
- `DB_NAME` - IssueGptDb

API:

- `API_PORT` - 5000
- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `OPENAI_MODEL` - gpt-4-turbo
