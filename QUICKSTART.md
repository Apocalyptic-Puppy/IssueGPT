# Quick Start (5 minutes)

## 1. Setup Environment

```bash
cp .env.example .env
# Edit .env with your GitHub Token & OpenAI API Key
```

## 2. Start MSSQL

```bash
docker-compose up -d
```

## 3. Build & Run API

```bash
cd IssueGPT.Api
dotnet restore
dotnet ef database update
dotnet run
```

## 4. Open Frontend

- Open `IssueGPT.Frontend/index.html` in browser
- Or: `python3 -m http.server 8000` in that folder

## 5. Analyze an Issue

- Enter: owner, repo, issue #
- Click Analyze
- Copy Copilot prompts → Paste into GitHub Copilot Chat

---

## What You Get

✅ AI-analyzed GitHub issue
✅ Execution plan
✅ Risk assessment  
✅ Task breakdown
✅ 5 ready-to-use Copilot prompts
✅ Saved to MSSQL for future reference

See [SETUP.md](SETUP.md) for detailed setup.
