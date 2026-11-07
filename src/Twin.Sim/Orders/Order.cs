using Twin.Sim.Geometry;

namespace Twin.Sim.Orders;

/// <summary>
/// Base type for everything a unit can be told to do. Orders are immutable data.
/// They are validated when issued and take effect at the start of the next tick.
/// </summary>
public abstract record Order;

/// <summary>Move along the ground to a map position. Height follows the terrain.</summary>
/// <param name="Destination">Target map position in meters.</param>
public sealed record MoveTo(Vec2 Destination) : Order;
