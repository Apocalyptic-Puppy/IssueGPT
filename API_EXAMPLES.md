# API Usage Examples

## Endpoint 1: Analyze GitHub Issue

### Request

```http
POST /api/issues/analyze
Content-Type: application/json

{
  "owner": "microsoft",
  "repo": "vscode",
  "issueNumber": 12345
}
```

### Response (200 OK)

```json
{
  "issueId": 1,
  "analysisId": 1,
  "summary": "Add support for multi-line region folding to improve code navigation in large files with complex structure.",
  "issueType": "feature",
  "executionPlan": "1. Examine current folding provider implementation\n2. Check region syntax parser\n3. Modify folding logic for nested regions\n4. Add configuration options\n5. Test edge cases with deeply nested structures",
  "risks": "Performance impact on large files with many nested regions. May break existing folding behavior if not careful.",
  "clarifyingQuestions": "- Should nested regions support unlimited depth?\n- What's the maximum practical nesting level?\n- Should this be behind a feature flag?",
  "taskBreakdown": "- [ ] Research current folding architecture\n- [ ] Update folding provider service\n- [ ] Modify region parser\n- [ ] Add unit tests\n- [ ] Add integration tests\n- [ ] Update documentation\n- [ ] Test with real-world complex files",
  "copilotPrompts": [
    {
      "type": "understand-code",
      "content": "GitHub Issue: Add support for multi-line region folding...\n\nPlease:\n1. Identify the modules and components...\n2. Explain the current execution flow...\n3. Point out where changes should be implemented...\n4. List any related files or patterns..."
    },
    {
      "type": "implement",
      "content": "GitHub Issue: Add support for multi-line region folding...\n\nPlease:\n1. Implement the requested changes...\n2. Keep the scope focused on this issue...\n3. Explain what each modified file does...\n4. Ensure backward compatibility..."
    },
    {
      "type": "testing",
      "content": "GitHub Issue: Add support for multi-line region folding...\n\nPlease:\n1. Add unit tests covering the main functionality...\n2. Add integration tests where applicable...\n3. Include edge cases and error scenarios...\n4. Consider regression risks from this change..."
    },
    {
      "type": "db-migration",
      "content": "GitHub Issue: Add support for multi-line region folding...\n\nPlease:\n1. Determine if database schema changes are needed...\n2. If needed, generate EF Core model updates...\n3. Create migration suggestions...\n4. Consider data migration if necessary..."
    },
    {
      "type": "prevent-hallucination",
      "content": "Before implementing:\n1. What are your assumptions about the current code structure?\n2. What information might be missing or unclear?\n3. Which parts of the specification need clarification?\n4. What edge cases could cause issues?"
    }
  ]
}
```

---

## Endpoint 2: Get Analysis by Issue ID

### Request

```http
GET /api/issues/1
```

### Response (200 OK)

```json
{
  "issueId": 1,
  "title": "Add multi-line region folding support",
  "url": "https://github.com/microsoft/vscode/issues/12345",
  "analysisId": 1,
  "summary": "Add support for multi-line region folding...",
  "issueType": "feature",
  "executionPlan": "1. Examine current folding provider...",
  "risks": "Performance impact on large files...",
  "clarifyingQuestions": "- Should nested regions support unlimited depth?...",
  "taskBreakdown": "- [ ] Research current folding architecture...",
  "copilotPrompts": [
    {
      "type": "understand-code",
      "content": "..."
    },
    ...
  ],
  "createdAt": "2026-04-05T10:30:00Z"
}
```

---

## Using Copilot Prompts

### Example Workflow

1. **Get Analysis**

   ```bash
   curl -X POST http://localhost:5000/api/issues/analyze \
     -H "Content-Type: application/json" \
     -d '{
       "owner": "microsoft",
       "repo": "vscode",
       "issueNumber": 12345
     }'
   ```

2. **Copy Prompt** (from response)

   ```
   GitHub Issue: Add support for multi-line region folding...

   Task Breakdown:
   - [ ] Research current folding architecture
   - [ ] Update folding provider service
   ...

   Please:
   1. Implement the requested changes following existing code conventions
   2. Keep the scope focused on this issue - avoid unrelated refactors
   3. Explain what each modified file does
   4. Ensure backward compatibility where applicable
   ```

3. **Paste into GitHub Copilot Chat**
   - GitHub Copilot will understand the issue context
   - Generate relevant code suggestions
   - Follow the structured execution plan

---

## Error Responses

### 400 Bad Request

```json
{
  "error": "Please fill in all fields"
}
```

### 404 Not Found

```json
{
  "error": "Issue not found"
}
```

### 500 Internal Server Error

```json
{
  "error": "Error fetching GitHub issue: Authentication failed"
}
```

---

## Database Query Examples

### Find all analyses for a specific issue

```sql
SELECT a.* FROM Analyses a
JOIN Issues i ON a.IssueId = i.Id
WHERE i.GitHubIssueNumber = 12345
ORDER BY a.CreatedAt DESC
```

### Find all issues for a repository

```sql
SELECT i.* FROM Issues i
JOIN Repositories r ON i.RepositoryId = r.Id
WHERE r.Owner = 'microsoft' AND r.Name = 'vscode'
ORDER BY i.CreatedAt DESC
```

### Get latest analysis with prompts

```sql
SELECT a.*, cp.*
FROM Analyses a
LEFT JOIN CopilotPrompts cp ON a.Id = cp.AnalysisId
WHERE a.IssueId = 1
ORDER BY cp.PromptType
```

---

## Performance Considerations

### Caching (Future)

- Cache analyses for 24 hours per issue
- Reduce API calls to GitHub & OpenAI
- Implement Redis layer

### Pagination (Future)

- Add `skip`/`take` parameters
- Handle large result sets efficiently

### Batch Operations

```json
POST /api/issues/analyze-batch
{
  "issues": [
    {"owner": "microsoft", "repo": "vscode", "issueNumber": 123},
    {"owner": "microsoft", "repo": "vscode", "issueNumber": 456}
  ]
}
```

---

## Integration with External Tools

### GitHub Actions

```yaml
- name: Analyze PR issue
  run: |
    curl -X POST http://localhost:5000/api/issues/analyze \
      -H "Content-Type: application/json" \
      -d '{"owner": "...", "repo": "...", "issueNumber": ${{ github.event.issue.number }}}'
```

### Slack Notification

```bash
ANALYSIS=$(curl -s -X GET http://localhost:5000/api/issues/1)
curl -X POST $SLACK_WEBHOOK \
  -d "{\"text\": \"Issue Analysis: $(echo $ANALYSIS | jq -r .summary)\"}"
```

---

## Testing the API

### Using curl

```bash
curl -X POST http://localhost:5000/api/issues/analyze \
  -H "Content-Type: application/json" \
  -d '{
    "owner": "microsoft",
    "repo": "vscode",
    "issueNumber": 12345
  }' | jq .
```

### Using Postman

1. Import: `http://localhost:5000/swagger`
2. Create new request to `POST /api/issues/analyze`
3. Add JSON body with owner/repo/issueNumber
4. Send & review response

### Using the Web UI

1. Open `IssueGPT.Frontend/index.html`
2. Enter GitHub repo details
3. Click Analyze
4. Review results & copy prompts
