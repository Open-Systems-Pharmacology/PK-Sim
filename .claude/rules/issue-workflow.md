## Issue Workflow

When asked to fix or implement a GitHub issue, follow this workflow:

1. **Branch** — Create a new branch from the current branch. Name it `<issue-number>-<short-description>` (e.g. `3461-event-placeholder-mapping`). Always start with the issue number, no `feature/` prefix.
2. **Implement with tests** — Write the code and corresponding tests, following the testing document (`.claude/docs/testing.md`)
3. **Code review** — Do a thorough code review of what was implemented
4. **Verify quality** — Build and run tests to confirm everything passes
5. **Commit** — Commit the changes
6. **Push and PR** — Push the branch and create a PR targeting the branch it was created from
