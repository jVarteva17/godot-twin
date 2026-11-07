---
name: scenario-test
description: Use when writing or reading behaviour tests for simulated agents (units moving, stopping, seeing, reacting). Explains the scenario kit in tests/Twin.Testing and the conventions for scenario tests.
---

# Writing scenario tests

Read `tests/Twin.Scenarios.Tests/Movement/MoveToTests.cs` first; copy its style.

## Shape of a test

```csharp
[Fact]
public void Unit_reaches_hilltop_at_the_terrain_height()
{
    // Arrange: describe the world
    var sim = Scenario.Create()
        .WithTerrain(TestTerrain.Hill(peak: new(500, 500), heightMeters: 40))
        .WithUnit("alpha", at: new(100, 100))
        .Build();

    // Act: give orders and advance simulated time
    sim.Order("alpha", new MoveTo(new(500, 500)))
       .RunUntil(s => s.Unit("alpha").IsIdle, TimeSpan.FromMinutes(5), "alpha idle");

    // Assert: describe the expected outcome
    sim.Unit("alpha").Should().BeAt(new Vec3(500, 500, 40));
}
```

## Rules

- One behaviour per test; the name is a sentence describing it.
- Time only advances through `Step`, `RunFor` and `RunUntil`. Always give `RunUntil` a timeout and a description.
- Use explicit tolerances in meters when exact equality is not the point.
- "Has started doing X" and "has finished doing X" are separate tests (e.g. starts moving vs reaches destination).
- Include invalid-input tests: unknown unit, off-map destination, conflicting orders.
- Available assertions: `BeAt(Vec2|Vec3, tol)`, `BeIdle()`, `BeMovingToward(Vec2)`, `BeOnGround(terrain)`,
  `HaveGroundSpeed(mps)`. Add new ones to `tests/Twin.Testing/UnitAssertions.cs` with a clear failure message.
- Keep numbers meaningful: compute expectations from inputs (distance / speed) rather than pasting magic values.
