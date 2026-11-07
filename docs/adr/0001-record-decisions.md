# 0001. Record decisions as ADRs

- Status: Accepted
- Date: 2026-11-07

## Context
Decisions made in meetings are forgotten, and new team members and AI agents see rules without reasons.
Rules without reasons get applied wrongly at the edges.

## Decision
Every significant technical decision is recorded as a short ADR in `docs/adr/`. `AGENTS.md` states the
rules; ADRs explain them. Changing a rule means a pull request that changes both.

## Consequences
A little writing per decision. In return, onboarding and agent context improve, and old debates are not reopened
without new information.
