using Twin.Sim;
using Twin.Sim.Orders;
using Twin.Sim.Terrain;
using Twin.Sim.Units;

namespace Twin.Testing;

/// <summary>
/// A running scenario. Time advances only through <see cref="Step"/>, <see cref="RunFor"/>
/// and <see cref="RunUntil"/>, so every test is reproducible.
/// </summary>
public sealed class ScenarioRun
{
    internal ScenarioRun(Simulation simulation) => Simulation = simulation;

    /// <summary>The underlying simulation, for checks the kit does not cover yet.</summary>
    public Simulation Simulation { get; }

    /// <summary>The scenario's terrain.</summary>
    public ITerrain Terrain => Simulation.Terrain;

    /// <summary>Simulated seconds elapsed.</summary>
    public double TimeSeconds => Simulation.TimeSeconds;

    /// <summary>Looks up a unit.</summary>
    public Unit Unit(string id) => Simulation.GetUnit(id);

    /// <summary>Issues an order; it takes effect on the next tick.</summary>
    public ScenarioRun Order(string unitId, Order order)
    {
        Simulation.Issue(unitId, order);
        return this;
    }

    /// <summary>Runs exactly <paramref name="ticks"/> ticks.</summary>
    public ScenarioRun Step(int ticks = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(ticks);
        for (var i = 0; i < ticks; i++)
        {
            Simulation.Step();
        }

        return this;
    }

    /// <summary>Runs for a span of simulated time (rounded up to whole ticks).</summary>
    public ScenarioRun RunFor(TimeSpan duration) =>
        Step((int)Math.Ceiling(duration.TotalSeconds * Simulation.Config.TickRateHz));

    /// <summary>
    /// Runs until <paramref name="condition"/> holds, checking after every tick.
    /// Fails with a diagnostic message if simulated <paramref name="timeout"/> passes first,
    /// so a broken behaviour never hangs the test suite.
    /// </summary>
    public ScenarioRun RunUntil(Func<ScenarioRun, bool> condition, TimeSpan timeout, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(condition);
        var maxTicks = (long)Math.Ceiling(timeout.TotalSeconds * Simulation.Config.TickRateHz);
        for (long i = 0; i < maxTicks; i++)
        {
            Simulation.Step();
            if (condition(this))
            {
                return this;
            }
        }

        throw new ScenarioAssertionException(
            $"Condition {(description is null ? string.Empty : $"'{description}' ")}was not met within {timeout.TotalSeconds:0.##} s " +
            $"of simulated time (tick {Simulation.Tick}).{Environment.NewLine}{Describe()}");
    }

    /// <summary>Every unit's state as text. Included in failure messages.</summary>
    public string Describe() =>
        string.Join(Environment.NewLine, Simulation.Units.Select(UnitAssertions.DescribeUnit));
}
