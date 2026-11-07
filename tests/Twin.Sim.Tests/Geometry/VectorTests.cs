using Twin.Sim.Geometry;

namespace Twin.Sim.Tests.Geometry;

public class VectorTests
{
    [Fact]
    public void Distance_is_euclidean()
    {
        Assert.Equal(5, new Vec2(0, 0).DistanceTo(new Vec2(3, 4)));
        Assert.Equal(3, new Vec3(1, 2, 2).Length);
    }

    [Fact]
    public void Normalizing_zero_vector_gives_zero_instead_of_NaN()
    {
        Assert.Equal(Vec2.Zero, Vec2.Zero.Normalized());
    }

    [Fact]
    public void Normalized_vector_has_unit_length()
    {
        Assert.Equal(1, new Vec2(3, -7).Normalized().Length, precision: 12);
    }
}
