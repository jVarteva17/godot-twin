# Testing guide

## Test layers

| Project | Tests | Speed | Share |
|---|---|---|---|
| `Twin.Scenarios.Tests` | Behaviour of agents in a simulated world | ms each | most tests |
| `Twin.Sim.Tests` | Pure logic: terrain sampling, math, random | µs each | many |
| `Twin.Architecture.Tests` | Structural rules (no engine in sim, no non-determinism) | fast | few, stable |
| Godot client | Manual play-testing for now; GdUnit4 when the client grows logic | slow | very few |

All of `./dev test` should run in seconds. If it gets slow, the tests or the simulation need attention.

## The scenario kit

`tests/Twin.Testing` is a small language for describing situations and expectations. It has no dependency on
a test framework and lives in the test tree, so it is changed in the spec phase like any test code.

```csharp
var sim = Scenario.Create()                     // 1 km flat map, 20 Hz, seed 1 by default
    .WithTerrain(TestTerrain.Hill(peak: new(500, 500), heightMeters: 40))
    .WithUnit("alpha", at: new(100, 100), speedMps: 5)
    .Build();

sim.Order("alpha", new MoveTo(new(500, 500)));  // takes effect on the next tick
sim.Step();                                      // exactly one tick
sim.RunFor(TimeSpan.FromSeconds(10));            // simulated time, rounded up to whole ticks
sim.RunUntil(s => s.Unit("alpha").IsIdle,        // stops as soon as the condition holds...
             TimeSpan.FromMinutes(5), "alpha idle"); // ...or fails with a state dump at the timeout

sim.Unit("alpha").Should().BeAt(new Vec3(500, 500, 40), toleranceMeters: 0.5).BeIdle();
```

Failure messages always include the unit's full state, for example:

```
Expected unit 'alpha' to be at (500, 500, 40) (±0.01 m), but it is 40 m away.
  unit 'alpha': position (500, 500, 0), velocity (0, 0, 0), activity Idle, order none
```

## Patterns

- **Started vs finished.** "Starts moving toward X after the next tick" and "reaches X" are separate tests.
  The first catches orders that are ignored; the second catches orders that never complete.
- **Expectations from inputs.** Write `Start.DistanceTo(Destination) / speed`, not `113.137`.
- **Tolerances.** Exact positions are fine where the model is exact (arrival snaps to the destination).
  Use explicit tolerances elsewhere, and justify unusual ones in a comment.
- **Determinism.** Every scenario is reproducible. A test that passes only sometimes is a bug in the
  simulation (ADR 0005), never something to retry.

## Extending the kit

Add vocabulary when two tests would otherwise duplicate setup or assertion logic. Each addition gets a doc
comment and a failure message that says what was expected and what happened. Keep it small: the kit is
read far more often than it is written.
