# 0005. Deterministic simulation

- Status: Accepted
- Date: 2026-11-07

## Context
TDD needs reproducible results: a test that sometimes fails is worthless. Replays, debugging and any future
lockstep multiplayer need the same.

## Decision
- Time advances only through `Simulation.Step()`, a fixed tick (default 20 Hz) independent of frame rate.
- Orders are validated when issued and applied at the start of the next tick, in issue order.
- All randomness comes from `Simulation.Random` (SplitMix64, seeded from `SimulationConfig.Seed`).
  `System.Random` is banned because its algorithm may change between .NET versions.
- Units are iterated in id order. No wall-clock time, threads, or unordered collections in the simulation.
- `Simulation.ComputeStateHash()` fingerprints the full state; the determinism tests compare runs tick by tick.

## Open question
Floating-point results are reproducible on one platform and runtime, but not guaranteed across CPUs and
operating systems. That matters only for lockstep multiplayer. Decide fixed-point vs server-authoritative
before starting networking.

## Consequences
Every bug is reproducible from seed plus orders. Systems must be written as single-threaded, ordered code;
parallelism later needs a deliberate, deterministic design.
