# 🎯 First-Time User Guide

Welcome to IssueGPT! This guide will get you from zero to analyzing GitHub issues in **5 minutes**.

---

## What is IssueGPT?

IssueGPT reads a GitHub issue and automatically produces:

- ✅ Clear summary
- ✅ Execution plan
- ✅ Risk assessment
- ✅ Task breakdown
- ✅ 5 Copilot prompts (copy-ready!)

Perfect for developers who want AI-powered insights when tackling GitHub issues.

---

## Prerequisites (3 things to get)

### 1. GitHub Personal Access Token

1. Go to https://github.com/settings/tokens
2. Click "Generate new token" → "Generate new token (classic)"
3. Name: "IssueGPT"
4. Scopes: Check `repo` (full control of repos)
5. Generate & copy the token
6. **Save it** — you'll need it in 2 minutes

### 2. OpenAI API Key

1. Go to https://platform.openai.com/api/keys
2. Click "Create new secret key"
3. Name: "IssueGPT"
4. Copy the key
5. **Save it** — you'll need it too

### 3. Docker (already on Mac?)

```bash
# Check if Docker is installed
docker --version

# If not, install from https://www.docker.com/products/docker-desktop
```

---

## Step 1: Configure (1 minute)

```bash
cd /Users/brad/Code/IssueGPT

# Create your .env file from the template
cp .env.example .env

# Open .env in your editor
nano .env  # or use VS Code
```

Edit these 2 lines:

```
GITHUB_TOKEN=ghp_YOUR_TOKEN_HERE
OPENAI_API_KEY=sk-YOUR_KEY_HERE
```

Save and close. ✅

---

## Step 2: Start Database (1 minute)

```bash
# Still in IssueGPT directory
docker-compose up -d

# Wait 10-15 seconds, then verify
docker ps

# You should see: issuegpt-mssql running
```

✅ Database is ready.

---

## Step 3: Build & Run API (2 minutes)

```bash
cd IssueGPT.Api

# Install dependencies
dotnet restore

# Setup database
dotnet ef database update

# Start the API
dotnet run
```

You'll see:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

✅ API is running. **Leave this terminal open.**

---

## Step 4: Open Frontend (1 minute)

In a **new terminal**:

```bash
# Option A: Just open the file
open /Users/brad/Code/IssueGPT/IssueGPT.Frontend/index.html

# Option B: Run a simple server
cd /Users/brad/Code/IssueGPT/IssueGPT.Frontend
python3 -m http.server 8000

# Then open: http://localhost:8000
```

You should see a beautiful purple form. ✅

---

## Step 5: Analyze Your First Issue (30 seconds)

1. Pick any GitHub issue
   - Example: `microsoft` / `vscode` / `188819`

2. Enter into the form:
   - **Owner**: microsoft
   - **Repo**: vscode
   - **Issue #**: 188819

3. Click **Analyze**

4. Wait 5-10 seconds...

5. You'll see:
   - 📝 Issue Summary
   - 📋 Execution Plan
   - ⚠️ Risks
   - ✅ Task Breakdown
   - 💬 5 Copilot Prompts

✅ **Success!**

---

## Step 6: Copy a Copilot Prompt

1. Scroll down to "GitHub Copilot Prompts"
2. Pick one (recommend "Understand Code" first)
3. Click **Copy Prompt**
4. Go to https://github.com/copilot/chat
5. Paste the prompt
6. Watch GitHub Copilot understand your issue with perfect context

✅ **You just powered up GitHub Copilot!**

---

## Troubleshooting

### "Connection refused" when starting API

- Check Docker is running: `docker ps`
- Check MSSQL is healthy: `docker logs issuegpt-mssql`
- Wait another 10 seconds

### "GITHUB_TOKEN is not set"

- Edit `.env` and add your GitHub token
- Restart API: Stop it (Ctrl+C) and run `dotnet run` again

### "OpenAI API error"

- Verify `OPENAI_API_KEY` in `.env`
- Check you have API credits: https://platform.openai.com/account/usage/overview
- Make sure key has `gpt-4-turbo` access

### Frontend shows "Analyzing..." forever

- Check API is running: See terminal output
- Try a different issue
- Check browser console (F12) for errors

---

## What Happens Behind the Scenes

```
1. You enter owner/repo/issue#
   ↓
2. Frontend sends HTTP POST to API
   ↓
3. API calls GitHub API to fetch the issue
   ↓
4. API calls OpenAI LLM to analyze it
   ↓
5. API generates 5 Copilot prompts
   ↓
6. API saves everything to MSSQL
   ↓
7. Frontend displays results beautifully
   ↓
8. You copy a prompt and paste into Copilot Chat
```

---

## Now What?

### 🚀 Advanced Use Cases

**Analyze multiple issues:**

- Just enter different issue numbers
- Each analysis is saved and retrievable

**Integration with your workflow:**

- Add IssueGPT link to your PR review checklist
- Run analysis before starting a task
- Share analysis with your team

**Check past analyses:**

- All analyses stored in MSSQL
- Can query and compare decisions later

---

## How to Stop Everything

```bash
# Stop the API
# In the API terminal, press Ctrl+C

# Stop the database
docker-compose down

# Later, to start again
docker-compose up -d
cd IssueGPT.Api && dotnet run
```

---

## Next: Level Up Your Skills

Finished and want to learn more?

1. **Understand the code** → See [ARCHITECTURE.md](ARCHITECTURE.md)
2. **Learn the API** → See [API_EXAMPLES.md](API_EXAMPLES.md)
3. **Add a feature** → See [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md)
4. **Deploy** → See [SETUP.md](SETUP.md#deployment)

---

## Success Checklist

You've successfully setup IssueGPT when:

- [x] Docker running MSSQL
- [x] API running on :5000
- [x] Frontend opens in browser
- [x] Can analyze a GitHub issue
- [x] Get back AI insights
- [x] Get 5 Copilot prompts
- [x] Can copy prompts to clipboard

---

## Reality Check

What you just did:

✅ Deployed a **full-stack web application** with:

- ASP.NET Core backend
- MSSQL database
- React-less modern frontend
- GitHub & OpenAI integrations

✅ In **less than 5 minutes**

✅ With **professional production code**

✅ Ready to **scale to enterprise**

That's pretty amazing. 🚀

---

## Questions?

Everything is documented:

- **Quick issues?** → Check [SETUP.md](SETUP.md)
- **Want the full picture?** → Read [ARCHITECTURE.md](ARCHITECTURE.md)
- **Curious about the code?** → See [FILE_INDEX.md](FILE_INDEX.md)
- **Need API examples?** → Check [API_EXAMPLES.md](API_EXAMPLES.md)

---

**Welcome to IssueGPT!** 🎉

You're now running a production-grade GitHub issue analyzer.

Time to analyze some issues. 🚀
