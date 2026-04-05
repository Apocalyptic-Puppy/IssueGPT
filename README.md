# IssueGPT

**AI-Powered GitHub Issue Triage & Execution Assistant**

## What is IssueGPT?

IssueGPT reads GitHub issues and automatically generates:

- Clear issue summaries & intent analysis
- Structured execution plans
- Risk assessments & clarifying questions
- Sub-task breakdowns
- **High-quality prompts optimized for GitHub Copilot**

Instead of confusion, you get a ready-to-execute blueprint.

---

## Why IssueGPT?

Most GitHub issues suffer from:

- ❌ Unclear execution direction
- ❌ Hidden risk factors
- ❌ Poor context for code generation
- ❌ Inefficient Copilot prompts

IssueGPT fills this gap by transforming raw GitHub issues into **structured, actionable intelligence**.

---

## MVP Features

### 1. Issue Ingestion

- Paste GitHub issue URL or issue number
- Automatically fetch issue + comments + metadata

### 2. Intelligent Analysis

- Issue classification (bug / feature / refactor / tech debt / etc)
- Probable intent extraction
- Risk & unknown factor identification

### 3. Execution Blueprint

- Suggested execution sequence
- Module/component guidance
- API/DB/UI impact analysis

### 4. Clarifying Questions

- Identify missing information
- List assumptions
- Suggest acceptance criteria

### 5. Task Breakdown

- Auto-decompose into subtasks
- Suggest implementation order

### 6. Copilot Prompt Generator

Generate purpose-built prompts for:

- **Code Understanding** — Understand the codebase & pinpoint change locations
- **Implementation** — Implement the requested changes
- **Testing** — Add unit & integration tests
- **DB/Migration** — Handle schema & EF Core migrations
- **Hallucination Prevention** — Ask for clarification before coding

### 7. Analysis History

- Store all analyses in MSSQL
- Review past GitHub issue decisions
- Track decision patterns

---

## Tech Stack

| Layer             | Technology                  |
| ----------------- | --------------------------- |
| **Backend API**   | ASP.NET Core 8.0            |
| **Database**      | MSSQL (Docker on Mac)       |
| **External APIs** | GitHub API, OpenAI API      |
| **Frontend**      | Simple Web UI (HTML/CSS/JS) |
| **Deployment**    | Docker (future: K8s)        |

---

## Project Structure (Planned)

```
IssueGPT/
├── IssueGPT.Api/              # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── IssueController.cs
│   ├── Services/
│   │   ├── GitHubService.cs
│   │   ├── AnalysisService.cs
│   │   ├── CopilotPromptService.cs
│   │   └── LLMService.cs
│   ├── Models/
│   ├── Dtos/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   └── Program.cs
├── IssueGPT.Frontend/         # Web UI
│   ├── index.html
│   └── app.js
├── IssueGPT.Tests/            # Unit & Integration Tests
├── docker-compose.yml         # MSSQL + API stack
└── README.md
```

---

## Development Roadmap

### Phase 1 (MVP)

- [ ] GitHub issue fetching
- [ ] LLM-based analysis
- [ ] Copilot prompt generation
- [ ] MSSQL schema & persistence
- [ ] Basic Web UI

### Phase 2 (Enhancement)

- [ ] Repository context awareness
- [ ] Analysis history & comparison
- [ ] Prompt templates & customization
- [ ] Rich dashboard

### Phase 3 (Scale)

- [ ] GitHub OAuth integration
- [ ] Auto-comment analysis to issues
- [ ] GitHub Actions integration
- [ ] Semantic search across past analyses

---

## Quick Start (Coming Soon)

```bash
# Clone repo
git clone <repo-url>
cd IssueGPT

# Setup environment
docker-compose up -d

# Configure API keys
# Add .env with GITHUB_TOKEN, OPENAI_API_KEY

# Run tests
dotnet test

# Start API
dotnet run --project IssueGPT.Api

# Open UI
# http://localhost:5000
```

---

## How It Works (Example Flow)

```
User: Paste GitHub issue URL
  ↓
IssueGPT: Fetch issue + comments
  ↓
IssueGPT: Send to LLM for analysis
  ↓
LLM Response:
  - Summary
  - Issue Type: "bug" / "feature"
  - Execution Plan: Step 1, 2, 3
  - Risks: Potential impacts
  - Missing Info: Questions to clarify
  - Sub-tasks: Decomposed work items
  - Copilot Prompts: 5 optimized prompts
  ↓
IssueGPT: Store in MSSQL + Display to user
  ↓
User: Copy Copilot prompt → Paste in GitHub Copilot Chat
  ↓
Result: High-quality code suggestions
```

---

## Database Schema (MVP)

Key tables:

- **Issues** — GitHub issue metadata
- **IssueComments** — Issue discussion context
- **Analyses** — Stored AI analysis results
- **CopilotPrompts** — Generated prompts for reuse

---

## Contributing

This is a personal learning project. Feedback welcome.

---

## License

MIT
