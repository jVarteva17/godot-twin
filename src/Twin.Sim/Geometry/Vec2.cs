namespace Twin.Sim.Geometry;

/// <summary>
/// A horizontal map position or direction in meters.
/// X points east, Y points north. See docs/adr/0003-coordinates-and-units.md.
/// </summary>
public readonly record struct Vec2(double X, double Y)
{
    /// <summary>The origin / zero vector.</summary>
    public static Vec2 Zero => new(0, 0);

    /// <summary>Length of the vector in meters.</summary>
    public double Length => Math.Sqrt((X * X) + (Y * Y));

    /// <summary>Distance to another point in meters.</summary>
    public double DistanceTo(Vec2 other) => (other - this).Length;

    /// <summary>Unit vector in the same direction, or zero for a zero vector.</summary>
    public Vec2 Normalized()
    {
        var length = Length;
        return length == 0 ? Zero : new Vec2(X / length, Y / length);
    }

    /// <summary>Dot product.</summary>
    public double Dot(Vec2 other) => (X * other.X) + (Y * other.Y);

    /// <summary>Adds a height to make a 3D position.</summary>
    public Vec3 WithZ(double z) => new(X, Y, z);

    /// <summary>Vector addition.</summary>
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>Vector subtraction.</summary>
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>Scales a vector.</summary>
    public static Vec2 operator *(Vec2 v, double s) => new(v.X * s, v.Y * s);

    /// <inheritdoc />
    public override string ToString() => $"({X:0.###}, {Y:0.###})";
}
