using Twin.Sim.Geometry;
using Twin.Sim.Terrain;

namespace Twin.Testing;

/// <summary>Ready-made terrains for tests. Add new shapes here rather than inside individual tests.</summary>
public static class TestTerrain
{
    /// <summary>Square, flat map at height 0.</summary>
    public static ITerrain Flat(double sizeMeters) => HeightmapTerrain.Flat(sizeMeters, sizeMeters);

    /// <summary>Square map with one smooth hill. Peak height is exact at <paramref name="peak"/>.</summary>
    public static ITerrain Hill(Vec2 peak, double heightMeters, double radiusMeters = 300, double sizeMeters = 1000)
        => HeightmapTerrain.Hill(sizeMeters, sizeMeters, peak, heightMeters, radiusMeters);
}
