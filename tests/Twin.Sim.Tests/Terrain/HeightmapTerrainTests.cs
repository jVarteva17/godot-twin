using Twin.Sim.Geometry;
using Twin.Sim.Terrain;

namespace Twin.Sim.Tests.Terrain;

public class HeightmapTerrainTests
{
    [Fact]
    public void Flat_terrain_is_zero_everywhere()
    {
        var terrain = HeightmapTerrain.Flat(1000, 500);

        Assert.Equal(0, terrain.HeightAt(new Vec2(0, 0)));
        Assert.Equal(0, terrain.HeightAt(new Vec2(1000, 500)));
        Assert.Equal(0, terrain.HeightAt(new Vec2(333.3, 123.4)));
    }

    [Fact]
    public void Map_extent_matches_requested_size()
    {
        var terrain = HeightmapTerrain.Flat(1000, 500, cellSizeMeters: 10);

        Assert.Equal(1000, terrain.WidthMeters);
        Assert.Equal(500, terrain.DepthMeters);
    }

    [Fact]
    public void Hill_has_its_peak_height_at_the_peak()
    {
        var terrain = HeightmapTerrain.Hill(1000, 1000, peak: new(500, 500), peakHeightMeters: 40, radiusMeters: 300);

        Assert.Equal(40, terrain.HeightAt(new Vec2(500, 500)), precision: 9);
    }

    [Fact]
    public void Hill_is_flat_beyond_its_radius()
    {
        var terrain = HeightmapTerrain.Hill(1000, 1000, peak: new(500, 500), peakHeightMeters: 40, radiusMeters: 300);

        Assert.Equal(0, terrain.HeightAt(new Vec2(100, 100)));
    }

    [Fact]
    public void Height_between_grid_nodes_is_interpolated_linearly()
    {
        // 2 x 2 nodes, 10 m cells: heights 0, 10 (south) and 20, 30 (north).
        var terrain = new HeightmapTerrain(2, 2, 10, [0, 10, 20, 30]);

        Assert.Equal(5, terrain.HeightAt(new Vec2(5, 0)), precision: 9);
        Assert.Equal(15, terrain.HeightAt(new Vec2(5, 5)), precision: 9);
        Assert.Equal(30, terrain.HeightAt(new Vec2(10, 10)), precision: 9);
    }

    [Theory]
    [InlineData(-0.1, 10)]
    [InlineData(10, 100.1)]
    public void Height_outside_the_map_is_an_error(double x, double y)
    {
        var terrain = HeightmapTerrain.Flat(100, 100);

        Assert.False(terrain.Contains(new Vec2(x, y)));
        Assert.Throws<ArgumentOutOfRangeException>(() => terrain.HeightAt(new Vec2(x, y)));
    }

    [Fact]
    public void Height_array_must_match_grid_size()
    {
        Assert.Throws<ArgumentException>(() => new HeightmapTerrain(3, 3, 10, new double[8]));
    }
}
