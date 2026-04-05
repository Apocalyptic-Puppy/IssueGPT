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

| Layer             | Technology                |
| ----------------- | ------------------------- |
| **Backend API**   | ASP.NET Core 8.0          |
| **Database**      | MSSQL Server 2022         |
| **External APIs** | GitHub API, OpenAI API    |
| **Frontend**      | HTML5 / CSS3 / JavaScript |
| **Deployment**    | Docker + Kubernetes       |

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

### Phase 3 (K8s & Scale)

- [x] Kubernetes support (NEW!)
- [ ] GitHub OAuth integration
- [ ] Auto-comment analysis to issues
- [ ] Semantic search across past analyses
- [ ] Multi-environment deployments (dev/staging/prod)

---

## 📖 Documentation

| Document                                     | Purpose                                      |
| -------------------------------------------- | -------------------------------------------- |
| **[GETTING_STARTED.md](GETTING_STARTED.md)** | 👈 **Start here** — 5-minute Docker setup    |
| [K8S_QUICK_START.md](K8S_QUICK_START.md)     | **New: Deploy to Kubernetes** on Mac (3 min) |
| [QUICKSTART.md](QUICKSTART.md)               | Fast setup (experienced developers)          |
| [SETUP.md](SETUP.md)                         | Detailed Docker step-by-step guide           |
| [K8S_DEPLOYMENT.md](K8S_DEPLOYMENT.md)       | Complete Kubernetes deployment guide         |
| [ARCHITECTURE.md](ARCHITECTURE.md)           | System design & data flow diagrams           |
| [API_EXAMPLES.md](API_EXAMPLES.md)           | API usage & integration examples             |
| [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) | Project layout & schema design               |
| [FILE_INDEX.md](FILE_INDEX.md)               | Complete file guide                          |
| [CHECKLIST.md](CHECKLIST.md)                 | Implementation status                        |
| [SUMMARY.md](SUMMARY.md)                     | Project overview & statistics                |

---

## 🚀 Get Started

### Option 1: Docker (Traditional)

See [GETTING_STARTED.md](GETTING_STARTED.md) — 5 minutes

### Option 2: Kubernetes (NEW!)

See [K8S_QUICK_START.md](K8S_QUICK_START.md) — 3 minutes + 3 port-forwards

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

## 📖 Documentation

| Document                                     | Purpose                                  |
| -------------------------------------------- | ---------------------------------------- |
| **[GETTING_STARTED.md](GETTING_STARTED.md)** | 👈 **Start here** — 5-minute setup guide |
| [QUICKSTART.md](QUICKSTART.md)               | Fast setup (experienced developers)      |
| [SETUP.md](SETUP.md)                         | Detailed step-by-step guide              |
| [ARCHITECTURE.md](ARCHITECTURE.md)           | System design & data flow diagrams       |
| [API_EXAMPLES.md](API_EXAMPLES.md)           | API usage & integration examples         |
| [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) | Project layout & schema design           |
| [FILE_INDEX.md](FILE_INDEX.md)               | Complete file guide & descriptions       |
| [CHECKLIST.md](CHECKLIST.md)                 | Implementation status & next steps       |
| [SUMMARY.md](SUMMARY.md)                     | Project overview & statistics            |

---

## 🏃 Get Started in 5 Minutes

```bash
# 1. Configure
cp .env.example .env
# Edit with your GITHUB_TOKEN and OPENAI_API_KEY

# 2. Start database
docker-compose up -d

# 3. Run API
cd IssueGPT.Api
dotnet restore
dotnet ef database update
dotnet run

# 4. Open frontend
open IssueGPT.Frontend/index.html

# 5. Analyze!
# Enter: microsoft / vscode / 12345
# Click Analyze
# Copy Copilot prompts
```

👉 [Full Getting Started Guide](GETTING_STARTED.md)

---

## 📊 Project Stats

- **1,117** lines of production C# code
- **2,129** lines of documentation
- **26** total files (code + docs)
- **5** database tables
- **3** microservices
- **2** external API integrations
- **5** Copilot prompt types

---

## 🎯 Use Cases

**For Developers:**

- Analyze GitHub issues faster
- Get structured insights before coding
- Generate Copilot prompts automatically
- Build execution plans instantly

**For Teams:**

- Share issue analysis with team members
- Build knowledge base of past analyses
- Standardize issue triage process
- Accelerate GitHub Copilot effectiveness

---

## License

MIT
