namespace Twin.Sim.Geometry;

/// <summary>
/// A 3D position or vector in meters. X east, Y north, Z up (height above the map datum).
/// See docs/adr/0003-coordinates-and-units.md.
/// </summary>
public readonly record struct Vec3(double X, double Y, double Z)
{
    /// <summary>The zero vector.</summary>
    public static Vec3 Zero => new(0, 0, 0);

    /// <summary>The horizontal (map) part of this vector.</summary>
    public Vec2 Xy => new(X, Y);

    /// <summary>Length of the vector in meters.</summary>
    public double Length => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

    /// <summary>Straight-line 3D distance in meters.</summary>
    public double DistanceTo(Vec3 other) => (other - this).Length;

    /// <summary>Vector subtraction.</summary>
    public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    /// <summary>Vector addition.</summary>
    public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>Scales a vector.</summary>
    public static Vec3 operator *(Vec3 v, double s) => new(v.X * s, v.Y * s, v.Z * s);

    /// <inheritdoc />
    public override string ToString() => $"({X:0.###}, {Y:0.###}, {Z:0.###})";
}
