using Twin.Sim.Geometry;
using Twin.Sim.Orders;

namespace Twin.Sim.Units;

/// <summary>What a unit is doing right now.</summary>
public enum UnitActivity
{
    /// <summary>Not executing any order.</summary>
    Idle,

    /// <summary>Moving toward a destination.</summary>
    Moving,
}

/// <summary>
/// A simulated agent on the map. State is changed only by the simulation's systems;
/// everything outside the simulation sees it read-only.
/// </summary>
public sealed class Unit
{
    internal Unit(string id, Vec3 position, double maxSpeedMps)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxSpeedMps);
        Id = id;
        Position = position;
        MaxSpeedMps = maxSpeedMps;
    }

    /// <summary>Unique, stable identifier.</summary>
    public string Id { get; }

    /// <summary>Position in meters; Z is the ground height for ground units.</summary>
    public Vec3 Position { get; internal set; }

    /// <summary>Velocity in meters per second at the end of the last tick.</summary>
    public Vec3 Velocity { get; internal set; }

    /// <summary>Maximum ground speed in meters per second.</summary>
    public double MaxSpeedMps { get; }

    /// <summary>The order being executed, or null when idle.</summary>
    public Order? CurrentOrder { get; internal set; }

    /// <summary>What the unit is currently doing.</summary>
    public UnitActivity Activity { get; internal set; } = UnitActivity.Idle;

    /// <summary>Convenience: true when <see cref="Activity"/> is <see cref="UnitActivity.Idle"/>.</summary>
    public bool IsIdle => Activity == UnitActivity.Idle;
}
