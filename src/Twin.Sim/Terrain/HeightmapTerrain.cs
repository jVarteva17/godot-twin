using Twin.Sim.Geometry;

namespace Twin.Sim.Terrain;

/// <summary>
/// Terrain stored as a regular grid of heights (a heightmap), sampled with bilinear interpolation.
/// Grid node (i, j) sits at map position (i * CellSize, j * CellSize).
/// </summary>
public sealed class HeightmapTerrain : ITerrain
{
    private readonly double[] _heights;

    /// <summary>Creates terrain from a row-major height array: index = j * NodesX + i.</summary>
    public HeightmapTerrain(int nodesX, int nodesY, double cellSizeMeters, double[] heights)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(nodesX, 2);
        ArgumentOutOfRangeException.ThrowIfLessThan(nodesY, 2);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cellSizeMeters);
        ArgumentNullException.ThrowIfNull(heights);
        if (heights.Length != nodesX * nodesY)
        {
            throw new ArgumentException($"Expected {nodesX * nodesY} heights, got {heights.Length}.", nameof(heights));
        }

        NodesX = nodesX;
        NodesY = nodesY;
        CellSizeMeters = cellSizeMeters;
        _heights = (double[])heights.Clone();
    }

    /// <summary>Number of grid nodes along X.</summary>
    public int NodesX { get; }

    /// <summary>Number of grid nodes along Y.</summary>
    public int NodesY { get; }

    /// <summary>Distance between neighbouring grid nodes, in meters.</summary>
    public double CellSizeMeters { get; }

    /// <inheritdoc />
    public double WidthMeters => (NodesX - 1) * CellSizeMeters;

    /// <inheritdoc />
    public double DepthMeters => (NodesY - 1) * CellSizeMeters;

    /// <summary>Height stored at a grid node.</summary>
    public double NodeHeight(int i, int j) => _heights[(j * NodesX) + i];

    /// <inheritdoc />
    public bool Contains(Vec2 point) =>
        point.X >= 0 && point.Y >= 0 && point.X <= WidthMeters && point.Y <= DepthMeters;

    /// <inheritdoc />
    public double HeightAt(Vec2 point)
    {
        if (!Contains(point))
        {
            throw new ArgumentOutOfRangeException(nameof(point), point, "Point is outside the map.");
        }

        var gx = point.X / CellSizeMeters;
        var gy = point.Y / CellSizeMeters;
        var i0 = Math.Min((int)Math.Floor(gx), NodesX - 2);
        var j0 = Math.Min((int)Math.Floor(gy), NodesY - 2);
        var tx = gx - i0;
        var ty = gy - j0;

        var h00 = NodeHeight(i0, j0);
        var h10 = NodeHeight(i0 + 1, j0);
        var h01 = NodeHeight(i0, j0 + 1);
        var h11 = NodeHeight(i0 + 1, j0 + 1);

        var south = h00 + ((h10 - h00) * tx);
        var north = h01 + ((h11 - h01) * tx);
        return south + ((north - south) * ty);
    }

    /// <summary>Flat terrain at height zero.</summary>
    public static HeightmapTerrain Flat(double widthMeters, double depthMeters, double cellSizeMeters = 5)
        => FromFunction(widthMeters, depthMeters, cellSizeMeters, _ => 0);

    /// <summary>
    /// A single smooth hill on flat ground. Height is <paramref name="peakHeightMeters"/> at
    /// <paramref name="peak"/> and falls to zero at <paramref name="radiusMeters"/> (cosine-squared profile).
    /// </summary>
    public static HeightmapTerrain Hill(
        double widthMeters, double depthMeters, Vec2 peak, double peakHeightMeters, double radiusMeters, double cellSizeMeters = 5)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radiusMeters);
        return FromFunction(widthMeters, depthMeters, cellSizeMeters, p =>
        {
            var d = p.DistanceTo(peak);
            if (d >= radiusMeters)
            {
                return 0;
            }

            var c = Math.Cos(Math.PI / 2 * d / radiusMeters);
            return peakHeightMeters * c * c;
        });
    }

    /// <summary>Builds a heightmap by sampling a height function at every grid node.</summary>
    public static HeightmapTerrain FromFunction(
        double widthMeters, double depthMeters, double cellSizeMeters, Func<Vec2, double> heightAt)
    {
        ArgumentNullException.ThrowIfNull(heightAt);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cellSizeMeters);
        var nodesX = (int)Math.Round(widthMeters / cellSizeMeters) + 1;
        var nodesY = (int)Math.Round(depthMeters / cellSizeMeters) + 1;
        var heights = new double[nodesX * nodesY];
        for (var j = 0; j < nodesY; j++)
        {
            for (var i = 0; i < nodesX; i++)
            {
                heights[(j * nodesX) + i] = heightAt(new Vec2(i * cellSizeMeters, j * cellSizeMeters));
            }
        }

        return new HeightmapTerrain(nodesX, nodesY, cellSizeMeters, heights);
    }
}
