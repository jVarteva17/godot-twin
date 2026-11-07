using Twin.Sim.Geometry;
using Twin.Sim.Orders;
using Twin.Testing;

namespace Twin.Scenarios.Tests.Movement;

/// <summary>
/// Behaviour of the MoveTo order. This file is the reference example for scenario tests:
/// one behaviour per test, named as a sentence, written with the scenario kit.
/// </summary>
public class MoveToTests
{
    private static readonly Vec2 Start = new(100, 100);
    private static readonly Vec2 Destination = new(500, 500);
    private static readonly TimeSpan Generous = TimeSpan.FromMinutes(5);

    [Fact]
    public void Order_has_no_effect_until_the_next_tick()
    {
        var sim = Scenario.Create().WithUnit("alpha", at: Start).Build();

        sim.Order("alpha", new MoveTo(Destination));

        sim.Unit("alpha").Should().BeIdle().BeAt(Start);
    }

    [Fact]
    public void Unit_ordered_to_move_starts_moving_toward_destination_on_next_tick()
    {
        var sim = Scenario.Create().WithUnit("alpha", at: Start).Build();

        sim.Order("alpha", new MoveTo(Destination)).Step();

        sim.Unit("alpha").Should().BeMovingToward(Destination);
        Assert.True(sim.Unit("alpha").Position.Xy.DistanceTo(Destination) < Start.DistanceTo(Destination));
    }

    [Fact]
    public void Moving_unit_travels_at_its_maximum_ground_speed()
    {
        var sim = Scenario.Create().WithUnit("alpha", at: Start, speedMps: 4).Build();

        sim.Order("alpha", new MoveTo(Destination)).RunFor(TimeSpan.FromSeconds(10));

        sim.Unit("alpha").Should().HaveGroundSpeed(4);
        Assert.Equal(40, sim.Unit("alpha").Position.Xy.DistanceTo(Start), precision: 6);
    }

    [Fact]
    public void Unit_reaches_destination_and_becomes_idle()
    {
        var sim = Scenario.Create().WithUnit("alpha", at: Start).Build();

        sim.Order("alpha", new MoveTo(Destination))
           .RunUntil(s => s.Unit("alpha").IsIdle, Generous, "alpha idle");

        sim.Unit("alpha").Should().BeAt(Destination).BeIdle();
    }

    [Fact]
    public void Arrival_time_equals_distance_divided_by_speed()
    {
        var sim = Scenario.Create().WithUnit("alpha", at: Start, speedMps: 5).Build();
        var expectedSeconds = Start.DistanceTo(Destination) / 5;

        sim.Order("alpha", new MoveTo(Destination))
           .RunUntil(s => s.Unit("alpha").IsIdle, Generous, "alpha idle");

        var oneTick = sim.Simulation.Config.TickSeconds;
        Assert.InRange(sim.TimeSeconds, expectedSeconds, expectedSeconds + oneTick);
    }

    [Fact]
    public void Unit_reaches_hilltop_at_the_terrain_height()
    {
        var sim = Scenario.Create()
            .WithTerrain(TestTerrain.Hill(peak: Destination, heightMeters: 40))
            .WithUnit("alpha", at: Start)
            .Build();

        sim.Order("alpha", new MoveTo(Destination))
           .RunUntil(s => s.Unit("alpha").IsIdle, Generous, "alpha idle");

        sim.Unit("alpha").Should().BeAt(new Vec3(500, 500, 40));
    }

    [Fact]
    public void Unit_stays_on_the_ground_while_crossing_a_hill()
    {
        var sim = Scenario.Create()
            .WithTerrain(TestTerrain.Hill(peak: new(300, 300), heightMeters: 25))
            .WithUnit("alpha", at: Start)
            .Build();
        sim.Order("alpha", new MoveTo(Destination));

        while (sim.Step().Unit("alpha").Activity == Twin.Sim.Units.UnitActivity.Moving)
        {
            sim.Unit("alpha").Should().BeOnGround(sim.Terrain);
        }

        sim.Unit("alpha").Should().BeAt(Destination);
    }

    [Fact]
    public void New_move_order_replaces_the_current_one()
    {
        var sim = Scenario.Create().WithUnit("alpha", at: Start).Build();
        var newDestination = new Vec2(100, 400);

        sim.Order("alpha", new MoveTo(Destination)).RunFor(TimeSpan.FromSeconds(5));
        sim.Order("alpha", new MoveTo(newDestination)).Step();

        sim.Unit("alpha").Should().BeMovingToward(newDestination);
        Assert.Equal(new MoveTo(newDestination), sim.Unit("alpha").CurrentOrder);
    }

    [Fact]
    public void Units_execute_their_orders_independently()
    {
        var sim = Scenario.Create()
            .WithUnit("alpha", at: Start)
            .WithUnit("bravo", at: new(900, 900))
            .Build();

        sim.Order("alpha", new MoveTo(Destination)).RunFor(TimeSpan.FromSeconds(10));

        sim.Unit("alpha").Should().BeMovingToward(Destination);
        sim.Unit("bravo").Should().BeIdle().BeAt(new Vec2(900, 900));
    }

    [Fact]
    public void Order_for_unknown_unit_is_rejected()
    {
        var sim = Scenario.Create().Build();

        Assert.Throws<OrderRejectedException>(() => sim.Order("ghost", new MoveTo(Destination)));
    }

    [Theory]
    [InlineData(-1, 500)]
    [InlineData(500, 1000.5)]
    public void Destination_outside_the_map_is_rejected(double x, double y)
    {
        var sim = Scenario.Create().WithUnit("alpha", at: Start).Build();

        Assert.Throws<OrderRejectedException>(() => sim.Order("alpha", new MoveTo(new Vec2(x, y))));
    }
}
