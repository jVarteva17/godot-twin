@AGENTS.md

## Claude Code specifics

- Hooks in `.claude/settings.json` enforce the TDD phase (`guard.cs`) and block finishing while tests
  are red (`verify.cs`). If a hook blocks you, read its message and follow it; do not try to work around it.
- For any behaviour change, use the `tdd-feature` skill. For writing scenario tests, use `scenario-test`.
- Before proposing a pull request, ask the `reviewer` subagent to check the change.
- You cannot change the TDD phase or the harness. Ask the human.
