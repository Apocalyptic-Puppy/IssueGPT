# System Architecture Diagram

## High-Level Data Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                        USER BROWSER                             │
│                  (IssueGPT.Frontend/index.html)                 │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │  Input Fields: Owner / Repo / Issue #                   │  │
│  │  → Click "Analyze"                                      │  │
│  └────────────────────┬────────────────────────────────────┘  │
└─────────────────────────┼───────────────────────────────────────┘
                          │
                          │ HTTP POST /api/issues/analyze
                          │ {"owner": "...", "repo": "...", "issueNumber": ...}
                          ↓
┌─────────────────────────────────────────────────────────────────┐
│              ASP.NET CORE WEB API (Port 5000)                  │
│                  IssueGPT.Api/Program.cs                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌────────────────────────────────────────────────────────┐   │
│  │           IssuesController                            │   │
│  │  ├─ AnalyzeIssue()  [POST /analyze]                   │   │
│  │  └─ GetAnalysis()   [GET /{id}]                       │   │
│  └────────────────────────────────────────────────────────┘   │
│                          │                                      │
│         ┌────────────────┼────────────────┐                     │
│         ↓                ↓                ↓                     │
│    ┌─────────────┐ ┌──────────────┐ ┌────────────────┐        │
│    │  GitHub     │ │  Analysis    │ │  Copilot       │        │
│    │  Service    │ │  Service     │ │  Prompt Service│        │
│    │             │ │              │ │                │        │
│    │ - Fetch     │ │ - LLM        │ │ - Generate 5   │        │
│    │   issue     │ │   analysis   │ │   prompt types │        │
│    │ - Fetch     │ │ - Parse JSON │ │ - Understand   │        │
│    │   comments  │ │ - Structure  │ │ - Implement    │        │
│    │ - Get labels│ │   insights   │ │ - Testing      │        │
│    └──────┬──────┘ └──────┬───────┘ └────────┬───────┘        │
│           │                │                   │                │
└───────────┼────────────────┼───────────────────┼────────────────┘
            │                │                   │
            │                │                   │
            ↓                ↓                   │
  ┌─────────────────┐ ┌─────────────────┐      │
  │ GitHub API      │ │ OpenAI API      │      │
  │ (octokit)       │ │ (gpt-4-turbo)   │      │
  │                 │ │                 │      │
  │ GET /repos/{}/  │ │ POST /chat/     │      │
  │   issues/{}/    │ │ completions     │      │
  │ GET /repos/{}/  │ │                 │      │
  │   issues/{}/    │ │ Returns JSON:   │      │
  │   comments      │ │ {               │      │
  │                 │ │   summary,      │      │
  │ Needs:          │ │   issueType,    │      │
  │ GITHUB_TOKEN    │ │   executionPlan,│      │
  │                 │ │   risks,        │      │
  │                 │ │   tasks...      │      │
  │                 │ │ }               │      │
  │                 │ │                 │      │
  │ Returns:        │ │ Needs:          │      │
  │ {               │ │ OPENAI_API_KEY  │      │
  │   issue,        │ │                 │      │
  │   comments,     │ │                 │      │
  │   labels        │ │                 │      │
  │ }               │ │                 │      │
  └────────┬────────┘ └────────┬────────┘      │
           │                   │               │
           └────────┬──────────┘               │
                    │                         │
                    │ Store Analysis Results  │
                    │ + Copilot Prompts       │
                    ↓                         │
┌─────────────────────────────────────────────┼──────────────────┐
│            MSSQL DATABASE (Docker)          │                  │
│            (Port 1433)                      │                  │
├─────────────────────────────────────────────┼──────────────────┤
│                                             │                  │
│  ┌──────────────────────────────────────┐  │                  │
│  │ Repositories Table                   │  │                  │
│  │ - Id, Owner, Name, Url, CreatedAt   │  │                  │
│  └──────────────────────────────────────┘  │                  │
│                    ↑                        │                  │
│                    │ FK                     │                  │
│  ┌──────────────────────────────────────┐  │                  │
│  │ Issues Table                         │  │                  │
│  │ - Id, RepoId, Number, Title, Body   │  │                  │
│  │ - State, Author, Labels, Assignee   │  │                  │
│  └──────────────────────────────────────┘  │                  │
│         ↑                  ↑                │                  │
│         │ FK              │ FK             │                  │
│  ┌──────────────┐   ┌─────────────────┐   │                  │
│  │ IssueComments│   │ Analyses        │   │                  │
│  │ - Id, Body   │   │ - Summary       │   │                  │
│  │ - Author     │   │ - Type          │   │                  │
│  │ - CreatedAt  │   │ - ExecutionPlan │   │                  │
│  └──────────────┘   │ - Risks         │   │                  │
│                     │ - Questions     │   │                  │
│                     │ - Tasks         │   │                  │
│                     │ - LLMResponse   │───┼──┐               │
│                     │ - CreatedAt     │   │  │               │
│                     └─────────────────┘   │  │               │
│                            ↑              │  │               │
│                            │ FK           │  │               │
│                     ┌───────────────────┐ │  │               │
│                     │ CopilotPrompts    │ │  │               │
│                     │ - Type:           │ │  │               │
│                     │   understand-code │←┴──┘               │
│                     │   implement       │                    │
│                     │   testing         │                    │
│                     │   db-migration    │                    │
│                     │   prevent-        │                    │
│                     │   hallucination   │                    │
│                     │ - Content         │                    │
│                     │ - CreatedAt       │                    │
│                     └───────────────────┘                    │
└─────────────────────────────────────────────────────────────┘
```

---

## Service Layer Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                    IssuesController                          │
│  Receives: POST /api/issues/analyze                          │
│  - Validates input (owner, repo, issueNumber)                │
│  - Orchestrates service calls                                │
│  - Returns formatted JSON response                           │
└──────────────────────────────────────────────────────────────┘
          │
          ├─ Call 1: GitHubService.FetchIssueAsync()
          │   └─ Returns: GitHubIssueData
          │       ├─ Title, Body, State
          │       ├─ Author, Labels, Assignee
          │       ├─ Comments (List<GitHubCommentData>)
          │       └─ URLs, timestamps
          │
          ├─ Store Issue + Comments in MSSQL
          │
          ├─ Call 2: AnalysisService.AnalyzeIssueAsync()
          │   ├─ Build LLM prompt with issue context
          │   ├─ Call OpenAI ChatGPT-4 API
          │   ├─ Parse JSON response
          │   └─ Returns: AnalysisResult
          │       ├─ Summary
          │       ├─ IssueType
          │       ├─ ExecutionPlan
          │       ├─ Risks
          │       ├─ ClarifyingQuestions
          │       └─ TaskBreakdown
          │
          ├─ Store Analysis in MSSQL
          │
          ├─ Call 3: CopilotPromptService.GeneratePrompts()
          │   ├─ GenerateUnderstandCodePrompt()
          │   ├─ GenerateImplementationPrompt()
          │   ├─ GenerateTestingPrompt()
          │   ├─ GenerateDbMigrationPrompt()
          │   └─ GeneratePreventHallucinationPrompt()
          │   Returns: List<CopilotPromptItem>
          │
          └─ Store CopilotPrompts in MSSQL
              Then Return Complete Response to Frontend
```

---

## Database Entity Relationships

```
┌────────────────────┐
│  Repository        │
│  ─────────────────│
│  ○ Id (PK)         │
│  - Owner           │
│  - Name            │
│  - Url             │
│  - CreatedAt       │
└────────────────────┘
          │ 1
          │
          ├─────────────────────────────────┐
          │ N                               │ N
          ↓                                 ↓
┌────────────────────┐          ┌────────────────────┐
│  Issue             │          │ (future: other)    │
│  ─────────────────│          │                    │
│  ○ Id (PK)         │          └────────────────────┘
│  ● RepoId (FK)     │
│  - Number          │
│  - Title           │
│  - Body            │
│  - State           │
│  - Author          │
│  - Labels          │
│  - Assignee        │
│  - Url             │
│  - CreatedAt       │
│  - UpdatedAt       │
│  - SyncedAt        │
└────────────────────┘
          │ 1
          │
    ┌─────┴─────────┐
    │ N             │ N
    ↓               ↓
┌──────────────┐  ┌──────────────┐
│ IssueComment │  │ Analysis     │
│ ────────────│  │ ────────────│
│ ○ Id (PK)    │  │ ○ Id (PK)    │
│ ● IssueId    │  │ ● IssueId    │
│ - CommentId  │  │ - Summary    │
│ - Author     │  │ - Type       │
│ - Body       │  │ - Plan       │
│ - CreatedAt  │  │ - Risks      │
│ - UpdatedAt  │  │ - Questions  │
│              │  │ - Tasks      │
│              │  │ - RawLLM     │
│              │  │ - Model      │
│              │  │ - CreatedAt  │
│              │  │ - UpdatedAt  │
│              │  └──────────────┘
│              │        │ 1
│              │        │
│              │        │ N
│              │        ↓
│              │  ┌──────────────────┐
│              │  │ CopilotPrompt    │
│              │  │ ────────────────│
│              │  │ ○ Id (PK)        │
│              │  │ ● AnalysisId     │
│              │  │ - Type           │
│              │  │ - Content        │
│              │  │ - CreatedAt      │
│              │  └──────────────────┘
└──────────────┘
```

Legend:

- ○ = Primary Key (PK)
- ● = Foreign Key (FK)
- N-1 = Many-to-One
- 1-N = One-to-Many

---

## Request-Response Lifecycle

```
1. USER SUBMITS FORM
   owner = "microsoft"
   repo = "vscode"
   issueNumber = 12345
        │
        ↓
2. BROWSER SENDS HTTP POST
   POST /api/issues/analyze
   Content-Type: application/json
   {"owner": "...", "repo": "...", "issueNumber": ...}
        │
        ↓
3. ISSUESCONTROLLER RECEIVES
   ├─ Validate inputs
   ├─ Parse owner, repo, issueNumber
   └─ Find or create Repository in DB
        │
        ├─────────────────────────────────────────┐
        │                                         │
        ↓                                         ↓
4A. GITHUBSERVICE.FETCHISSUE    4B. STORE IN MSSQL
   ├─ Call GitHub API            ├─ Create Issue
   ├─ Get issue metadata          ├─ Add Comments
   └─ Get comments                └─ Set SyncedAt
        │
        ├─────────────────────────────────────────┐
        │                                         │
        ↓                                         ↓
5A. ANALYSISSERVICE.ANALYZE     5B. STORE IN MSSQL
   ├─ Build LLM prompt            ├─ Create Analysis
   ├─ Call OpenAI API             ├─ Save Summary
   ├─ Parse response              ├─ Save Type, Plan, etc
   └─ Validate JSON               └─ Store RawResponse
        │
        ├─────────────────────────────────────────┐
        │                                         │
        ↓                                         ↓
6A. COPILOT SERVICE           6B. STORE IN MSSQL
   ├─ Generate 5 prompts         ├─ Create Prompts[5]
   ├─ Type 1: Understand         ├─ Save each content
   ├─ Type 2: Implement          └─ Link to Analysis
   ├─ Type 3: Testing
   ├─ Type 4: DB Migration
   └─ Type 5: Hallucination
        │
        └─────────────────────────────────────────┐
                                                  │
                                                  ↓
7. BUILD JSON RESPONSE
   {
     issueId: 1,
     analysisId: 1,
     summary: "...",
     issueType: "feature",
     executionPlan: "...",
     risks: "...",
     clarifyingQuestions: "...",
     taskBreakdown: "...",
     copilotPrompts: [
       { type: "understand-code", content: "..." },
       { type: "implement", content: "..." },
       ...
     ]
   }
        │
        ↓
8. HTTP 200 OK
   Send JSON response to frontend
        │
        ↓
9. FRONTEND DISPLAYS
   ├─ Issue Summary
   ├─ Execution Plan
   ├─ Risks
   ├─ Questions
   ├─ Tasks
   └─ 5 Copy-able Prompts
```

---

## Component Dependencies

```
                         IssuesController
                         ╱   ╱   ╱   ╲
                        ╱    ╱    ╱    ╲
                       ╱     ╱     ╱     ╲
                      ╱      ╱      ╱      ╲
                     ╱       ╱       ╱       ╲
        GitHubService  AnalysisService  CopilotPromptService
             │              │              │
             │              │              │
        GitHub API      OpenAI API    (No external deps)
             │              │
             │              │
             └──────┬───────┘
                    │
                    ↓
            AppDbContext
            (EF Core)
                    │
                    ├─ Repository
                    ├─ Issue
                    ├─ IssueComment
                    ├─ Analysis
                    └─ CopilotPrompt
                    │
                    ↓
            MSSQL Server
            (Docker)
```

---

## Deployment Architecture (Future)

```
┌─────────────────────────────────────────────────────────────┐
│                       KUBERNETES CLUSTER                   │
│  (future deployment, not in MVP)                           │
│                                                             │
│  ┌──────────────┐         ┌──────────────┐               │
│  │  Frontend    │         │  API Service │               │
│  │  Replicas: 2 │─────────│  Replicas: 3 │               │
│  └──────────────┘         └──────┬───────┘               │
│                                  │                       │
│                    ┌─────────────┼─────────────┐         │
│                    │             │             │         │
│                    ↓             ↓             ↓         │
│        ┌──────────────────────────────────────────┐     │
│        │      MSSQL Database                      │     │
│        │      (Azure SQL Database)                │     │
│        │      (managed, replicated)               │     │
│        └──────────────────────────────────────────┘     │
│                                                         │
│  ┌────────────────────────────────────────────────┐    │
│  │  Service Mesh (Istio)                         │    │
│  │  - Traffic routing                            │    │
│  │  - Circuit breaking                           │    │
│  │  - Rate limiting                              │    │
│  └────────────────────────────────────────────────┘    │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

**Current: MVP with Docker + Local MSSQL**
**Future: Kubernetes in Azure + managed databases**
