# IssueGPT - Complete File Index

## 📁 Project Root Files

### Documentation

| File                                                     | Purpose                                                 |
| -------------------------------------------------------- | ------------------------------------------------------- |
| [README.md](README.md)                                   | Product overview, features, tech stack, roadmap         |
| [QUICKSTART.md](QUICKSTART.md)                           | 5-minute setup guide                                    |
| [SETUP.md](SETUP.md)                                     | Detailed setup instructions with troubleshooting        |
| [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md)             | Architecture, data flow, schema, environment variables  |
| [IMPLEMENTATION_COMPLETE.md](IMPLEMENTATION_COMPLETE.md) | Project summary, what's built, technology used          |
| [ARCHITECTURE.md](ARCHITECTURE.md)                       | System diagrams, service relationships, deployment plan |
| [API_EXAMPLES.md](API_EXAMPLES.md)                       | API usage examples, integration patterns, curl commands |
| [CHECKLIST.md](CHECKLIST.md)                             | Implementation checklist, verification, next steps      |

### Configuration

| File                                     | Purpose                                            |
| ---------------------------------------- | -------------------------------------------------- |
| [.env.example](.env.example)             | Environment variables template (copy to .env)      |
| [.gitignore](.gitignore)                 | Git exclusions (bin, obj, .env, node_modules, etc) |
| [docker-compose.yml](docker-compose.yml) | MSSQL Server Docker container configuration        |

---

## 📦 IssueGPT.Api/ - ASP.NET Core Backend

### Project Files

| File                                                    | Purpose                                                |
| ------------------------------------------------------- | ------------------------------------------------------ |
| [IssueGPT.Api.csproj](IssueGPT.Api/IssueGPT.Api.csproj) | NuGet dependencies (EF Core, Octokit, OpenAI, Swagger) |
| [Program.cs](IssueGPT.Api/Program.cs)                   | Startup, DI container, middleware configuration        |

### Controllers

| File                                                                | Purpose              | Endpoints                |
| ------------------------------------------------------------------- | -------------------- | ------------------------ |
| [IssuesController.cs](IssueGPT.Api/Controllers/IssuesController.cs) | API request handling | POST /analyze, GET /{id} |

### Services (Business Logic)

| File                                                                     | Purpose                                                                                             |
| ------------------------------------------------------------------------ | --------------------------------------------------------------------------------------------------- |
| [GitHubService.cs](IssueGPT.Api/Services/GitHubService.cs)               | GitHub API integration (Octokit), fetch issues & comments                                           |
| [AnalysisService.cs](IssueGPT.Api/Services/AnalysisService.cs)           | LLM analysis via OpenAI API, parse JSON responses                                                   |
| [CopilotPromptService.cs](IssueGPT.Api/Services/CopilotPromptService.cs) | Generate 5 types of Copilot prompts (understand, implement, test, migration, prevent-hallucination) |

### Data Access (ORM)

| File                                                 | Purpose                                               |
| ---------------------------------------------------- | ----------------------------------------------------- |
| [AppDbContext.cs](IssueGPT.Api/Data/AppDbContext.cs) | EF Core DbContext, relationships, migrations, indexes |

### Models (Entities)

| File                                                     | Represents                  | Table          |
| -------------------------------------------------------- | --------------------------- | -------------- |
| [Repository.cs](IssueGPT.Api/Models/Repository.cs)       | GitHub repository reference | Repositories   |
| [Issue.cs](IssueGPT.Api/Models/Issue.cs)                 | GitHub issue with metadata  | Issues         |
| [IssueComment.cs](IssueGPT.Api/Models/IssueComment.cs)   | Issue discussion comment    | IssueComments  |
| [Analysis.cs](IssueGPT.Api/Models/Analysis.cs)           | LLM analysis result         | Analyses       |
| [CopilotPrompt.cs](IssueGPT.Api/Models/CopilotPrompt.cs) | Generated Copilot prompt    | CopilotPrompts |

---

## 🎨 IssueGPT.Frontend/

| File                                       | Purpose                                               |
| ------------------------------------------ | ----------------------------------------------------- |
| [index.html](IssueGPT.Frontend/index.html) | Single-page web application (HTML + CSS + JavaScript) |

**Features:**

- Input form: owner, repo, issue number
- API integration with backend
- Loading spinner during analysis
- Results display with formatted sections
- Copy-to-clipboard for Copilot prompts
- Error handling & responsive design

---

## 📊 Database Schema

Created by EF Core migrations, defined in `AppDbContext.cs`:

### Repositories

```sql
Id (PK) | Owner | Name | Url | CreatedAt
```

### Issues

```sql
Id (PK) | RepositoryId (FK) | GitHubIssueNumber | Title | Body | State |
Author | LabelsJson | Assignee | GitHubUrl | CreatedAt | UpdatedAt | SyncedAt
```

### IssueComments

```sql
Id (PK) | IssueId (FK) | GitHubCommentId | Author | Body | CreatedAt | UpdatedAt
```

### Analyses

```sql
Id (PK) | IssueId (FK) | Summary | IssueType | ExecutionPlan | Risks |
ClarifyingQuestions | TaskBreakdown | RawLlmResponse | ModelName | CreatedAt | UpdatedAt
```

### CopilotPrompts

```sql
Id (PK) | AnalysisId (FK) | PromptType | Content | CreatedAt
```

---

## 🔌 External API Integrations

### GitHub API (via Octokit)

- **File:** `GitHubService.cs`
- **Requires:** `GITHUB_TOKEN`
- **Endpoints used:**
  - `GET /repos/{owner}/{repo}/issues/{number}`
  - `GET /repos/{owner}/{repo}/issues/{number}/comments`

### OpenAI API

- **File:** `AnalysisService.cs`
- **Requires:** `OPENAI_API_KEY`
- **Model:** `gpt-4-turbo`
- **Endpoint:** `POST /chat/completions`
- **Response format:** Structured JSON with analysis fields

---

## 📐 API Endpoints

### 1. Analyze GitHub Issue

```
POST /api/issues/analyze
Content-Type: application/json

Request:
{
  "owner": "microsoft",
  "repo": "vscode",
  "issueNumber": 12345
}

Response (200):
{
  "issueId": 1,
  "analysisId": 1,
  "summary": "...",
  "issueType": "bug|feature|refactor|tech-debt|unclear",
  "executionPlan": "...",
  "risks": "...",
  "clarifyingQuestions": "...",
  "taskBreakdown": "...",
  "copilotPrompts": [
    {"type": "understand-code", "content": "..."},
    {"type": "implement", "content": "..."},
    {"type": "testing", "content": "..."},
    {"type": "db-migration", "content": "..."},
    {"type": "prevent-hallucination", "content": "..."}
  ]
}
```

### 2. Get Analysis by Issue ID

```
GET /api/issues/{issueId}

Response (200):
{
  "issueId": 1,
  "title": "...",
  "url": "https://github.com/...",
  "analysisId": 1,
  "summary": "...",
  "issueType": "...",
  ...
}
```

---

## 🚀 Quick Start Flow

1. **Read:** [QUICKSTART.md](QUICKSTART.md)
2. **Setup:** Docker + environment variables
3. **Build:** `dotnet restore` + `dotnet ef database update`
4. **Run:** `dotnet run`
5. **Test:** Open frontend, analyze an issue
6. **Copy:** Copilot prompts to clipboard
7. **Use:** Paste in GitHub Copilot Chat

---

## 📚 Deep Dive Documentation

- **Architecture:** See [ARCHITECTURE.md](ARCHITECTURE.md) for diagrams
- **API Details:** See [API_EXAMPLES.md](API_EXAMPLES.md) for usage examples
- **Project Layout:** See [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) for details
- **Troubleshooting:** See [SETUP.md](SETUP.md) for help

---

## 🎯 Key Statistics

| Metric                  | Count  |
| ----------------------- | ------ |
| C# files                | 8      |
| HTML files              | 1      |
| Markdown files          | 10     |
| YAML files              | 1      |
| Database tables         | 5      |
| API endpoints           | 2      |
| Copilot prompt types    | 5      |
| NuGet dependencies      | ~10    |
| Estimated lines of code | ~2,000 |

---

## 🔄 Data Flow Summary

```
GitHub Issue URL
    ↓
GitHubService fetches issue + comments
    ↓
Store in MSSQL (Issues, IssueComments)
    ↓
AnalysisService calls OpenAI LLM
    ↓
Store Analysis in MSSQL
    ↓
CopilotPromptService generates 5 prompts
    ↓
Store Prompts in MSSQL
    ↓
Return to Frontend + Display
    ↓
User copies prompts → Pastes in Copilot Chat
```

---

## ✅ File Checklist

### Essential Files (Must Have)

- [x] Program.cs — API startup
- [x] IssuesController.cs — Request handling
- [x] AppDbContext.cs — Database context
- [x] 5 Model files — Entity definitions
- [x] 3 Service files — Business logic
- [x] docker-compose.yml — Database container
- [x] .env.example — Configuration template
- [x] index.html — Frontend UI

### Documentation Files

- [x] README.md — Product overview
- [x] QUICKSTART.md — Quick setup
- [x] SETUP.md — Detailed setup
- [x] ARCHITECTURE.md — System design
- [x] API_EXAMPLES.md — API documentation
- [x] PROJECT_STRUCTURE.md — Project layout
- [x] IMPLEMENTATION_COMPLETE.md — Status report
- [x] CHECKLIST.md — Tasks & verification

---

## 🔍 File Usage Guide

### If you want to...

| Goal                  | Start here                                                                        |
| --------------------- | --------------------------------------------------------------------------------- |
| Get started quickly   | [QUICKSTART.md](QUICKSTART.md)                                                    |
| Understand the system | [ARCHITECTURE.md](ARCHITECTURE.md)                                                |
| Configure environment | [SETUP.md](SETUP.md) + [.env.example](.env.example)                               |
| Learn API endpoints   | [API_EXAMPLES.md](API_EXAMPLES.md)                                                |
| See project status    | [IMPLEMENTATION_COMPLETE.md](IMPLEMENTATION_COMPLETE.md)                          |
| Understand data model | [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md)                                      |
| Check progress        | [CHECKLIST.md](CHECKLIST.md)                                                      |
| Add a new feature     | [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) → [ARCHITECTURE.md](ARCHITECTURE.md) |
| Deploy                | [SETUP.md](SETUP.md) (Deployment section)                                         |

---

**IssueGPT MVP is fully implemented and ready to use.** 🎉

Last updated: April 5, 2026
