using Twin.Sim.Geometry;

namespace Twin.Sim.Terrain;

/// <summary>
/// Read-only access to the ground surface. This is the 2.5D world model:
/// exactly one ground height per map position (docs/adr/0004-world-model-2-5d.md).
/// </summary>
public interface ITerrain
{
    /// <summary>Extent of the map along X (east), in meters. The map spans [0, WidthMeters].</summary>
    double WidthMeters { get; }

    /// <summary>Extent of the map along Y (north), in meters. The map spans [0, DepthMeters].</summary>
    double DepthMeters { get; }

    /// <summary>True if the point lies on the map (edges included).</summary>
    bool Contains(Vec2 point);

    /// <summary>Ground height in meters at a map position. Throws if the point is off the map.</summary>
    double HeightAt(Vec2 point);
}
