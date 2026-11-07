---
name: tdd-feature
description: Use for any change to simulation behaviour or any new agent capability (orders, movement, perception, combat), and whenever a ticket has acceptance criteria. Walks through the spec / review / implement cycle this repository requires.
---

# Test-driven feature workflow

Check the phase first: `./dev phase`.

## If the phase is `spec`

1. Restate the ticket's acceptance criteria as a numbered list. Ask the human about anything ambiguous
   (tolerances, timing, what happens on invalid input) before writing tests.
2. For each criterion write one scenario test (see the `scenario-test` skill). Put them in
   `tests/Twin.Scenarios.Tests/<Area>/<Feature>Tests.cs`.
3. If the kit lacks vocabulary, extend `tests/Twin.Testing` with a small, documented addition.
4. Run `./dev test`. Every new test must fail, and for the right reason: an assertion failure or a
   missing type/member, not an unrelated crash. Fix the test if it fails for the wrong reason.
5. Finish by giving the human: the list of tests, which criterion each covers, and the failure output.
   Ask them to review and run `./dev phase implement`. Do not touch `src/`.

## If the phase is `implement`

1. Read the failing tests. They are the specification. Do not edit existing test files.
2. Make the smallest change in `src/` that turns them green. Follow AGENTS.md: new behaviour in a
   system, orders as records, validation in `Simulation.Issue`, no non-deterministic APIs.
3. Run `./dev test` after each meaningful change.
4. If you think a test is wrong, stop and explain the problem. Do not work around it.
5. When green, refactor for clarity, then run `./dev check`.
6. Ask the `reviewer` subagent to review the diff, address its findings, and summarise the change.
