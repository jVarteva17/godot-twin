# 0002. Simulation separate from presentation

- Status: Accepted
- Date: 2026-11-07

## Context
The digital twin's value is in its rules: terrain, movement, perception, combat. Engines, renderers and UI
change faster than those rules. Tests, replays, servers and batch analysis all need the rules without graphics.

## Decision
`src/Twin.Sim` is a plain .NET library with no engine, UI or rendering dependency. The Godot client reads
simulation state and issues orders; it contains no game rules. A headless runner uses the same library.

Enforced by `tests/Twin.Architecture.Tests` (referenced assemblies and source scan).

## Alternatives considered
- Rules inside Godot nodes: fastest to start, but untestable without the engine, tied to Godot forever, and
  rules spread across the presentation code.
- Godot physics for the world model: convenient raycasts, but non-deterministic and engine-bound (ADR 0005).

## Consequences
Tests run in milliseconds with `dotnet test` and no Godot install. The engine could be replaced. Some data
needs mapping at the boundary (ADR 0003), and the client must not "quickly" add a rule.
