# IssueGPT - Setup Guide

## Prerequisites

- .NET 8.0 SDK
- Docker & Docker Compose
- GitHub Personal Access Token (PAT)
- OpenAI API Key

---

## Step 1: Clone & Environment Setup

```bash
cd IssueGPT
cp .env.example .env
```

Edit `.env` with your credentials:

```
GITHUB_TOKEN=ghp_your_token_here
OPENAI_API_KEY=sk-your_key_here
```

---

## Step 2: Start MSSQL Docker Container

```bash
docker-compose up -d
```

Verify MSSQL is running:

```bash
docker-compose logs mssql
```

Connection details:

- Server: localhost
- Port: 1433
- User: sa
- Password: IssueGPT@2026

---

## Step 3: Restore NuGet Packages & Create Migration

```bash
cd IssueGPT.Api

# Restore packages
dotnet restore

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

---

## Step 4: Run the API

```bash
dotnet run
```

The API will start on `http://localhost:5000`

Swagger docs available at: `http://localhost:5000/swagger`

---

## Step 5: Open the Web UI

Open `IssueGPT.Frontend/index.html` in your browser, or:

```bash
# Simple HTTP server (Python 3)
cd IssueGPT.Frontend
python3 -m http.server 8000
```

Then open: `http://localhost:8000`

---

## How to Use

1. Enter GitHub repository info:
   - Owner: e.g., `microsoft`
   - Repo: e.g., `vscode`
   - Issue #: e.g., `12345`

2. Click **Analyze**

3. View:
   - Issue summary
   - Execution plan
   - Risks & concerns
   - Task breakdown
   - **5 GitHub Copilot prompts** → Copy & paste into Copilot Chat

---

## API Endpoints

### Analyze Issue

```
POST /api/issues/analyze
Body: {
  "owner": "microsoft",
  "repo": "vscode",
  "issueNumber": 12345
}
```

### Get Analysis by Issue ID

```
GET /api/issues/{issueId}
```

---

## Troubleshooting

### MSSQL connection fails

- Check Docker is running: `docker ps`
- Check credentials in `.env`
- Wait 10-15 seconds for MSSQL to fully initialize

### Migration errors

```bash
# Reset database (WARNING: deletes all data)
dotnet ef database drop
dotnet ef database update
```

### API won't start

- Check port 5000 is available
- Check .env variables are set
- Review error logs in terminal

---

## Development Tips

- **Swagger UI**: Explore API at `http://localhost:5000/swagger`
- **Logs**: Check terminal output for detailed errors
- **Database**: Use SQL Server Management Studio or Azure Data Studio to browse data
- **Debugging**: Use VS Code debugger with breakpoints

---

## Next Steps

- Add GitHub OAuth (skip manual token entry)
- Add web dashboard with analysis history
- Implement PR description generator
- Add support for PR reviews
