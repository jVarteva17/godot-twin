using Twin.Sim.Geometry;
using Twin.Sim.Orders;
using Twin.Testing;

namespace Twin.Scenarios.Tests.Determinism;

/// <summary>
/// Guards docs/adr/0005-determinism.md: identical inputs must give identical state, tick by tick.
/// If this fails, some code reads time, uses unordered collections, or uses unseeded randomness.
/// </summary>
public class DeterminismTests
{
    [Fact]
    public void Same_scenario_and_orders_produce_identical_state_every_tick()
    {
        var first = RecordHashes(seed: 42);
        var second = RecordHashes(seed: 42);

        Assert.Equal(first, second);
    }

    [Fact]
    public void State_hash_changes_when_the_world_changes()
    {
        var hashes = RecordHashes(seed: 42);

        Assert.NotEqual(hashes[0], hashes[^1]);
    }

    private static List<ulong> RecordHashes(ulong seed)
    {
        var sim = Scenario.Create()
            .WithSeed(seed)
            .WithTerrain(TestTerrain.Hill(peak: new(400, 600), heightMeters: 30))
            .WithUnit("alpha", at: new(100, 100))
            .WithUnit("bravo", at: new(800, 200), speedMps: 8)
            .Build();

        sim.Order("alpha", new MoveTo(new Vec2(700, 700)));
        sim.Order("bravo", new MoveTo(new Vec2(200, 800)));

        var hashes = new List<ulong>();
        for (var tick = 0; tick < 600; tick++)
        {
            if (tick == 300)
            {
                sim.Order("alpha", new MoveTo(new Vec2(50, 900)));
            }

            sim.Step();
            hashes.Add(sim.Simulation.ComputeStateHash());
        }

        return hashes;
    }
}
