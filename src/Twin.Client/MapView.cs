using Godot;
using Twin.Sim.Geometry;
using Twin.Sim.Terrain;

namespace Twin.Client;

/// <summary>
/// Draws the terrain once as a shaded relief texture with contour lines, so height is visible
/// on a flat map. Pure presentation: it only reads <see cref="ITerrain"/>.
/// </summary>
public partial class MapView : Sprite2D
{
    private const double SampleSpacingMeters = 2.0;
    private const double ContourIntervalMeters = 5.0;

    private readonly ITerrain _terrain = null!;
    private readonly CoordinateMapper _mapper = null!;

    /// <summary>Required by Godot. Not used directly.</summary>
    public MapView()
    {
    }

    /// <summary>Creates the view for a terrain.</summary>
    public MapView(ITerrain terrain, CoordinateMapper mapper)
    {
        _terrain = terrain;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public override void _Ready()
    {
        Centered = false;
        Texture = ImageTexture.CreateFromImage(RenderRelief());
        var scale = (float)(SampleSpacingMeters * _mapper.PixelsPerMeter);
        Scale = new Vector2(scale, scale);
    }

    private Image RenderRelief()
    {
        var width = (int)(_terrain.WidthMeters / SampleSpacingMeters);
        var height = (int)(_terrain.DepthMeters / SampleSpacingMeters);
        var image = Image.CreateEmpty(width, height, false, Image.Format.Rgb8);

        for (var py = 0; py < height; py++)
        {
            for (var px = 0; px < width; px++)
            {
                var map = new Vec2(px * SampleSpacingMeters, _terrain.DepthMeters - (py * SampleSpacingMeters));
                image.SetPixel(px, py, ShadeAt(map));
            }
        }

        return image;
    }

    private Color ShadeAt(Vec2 p)
    {
        var h = _terrain.HeightAt(p);
        var east = _terrain.HeightAt(new Vec2(Math.Min(p.X + SampleSpacingMeters, _terrain.WidthMeters), p.Y));
        var north = _terrain.HeightAt(new Vec2(p.X, Math.Min(p.Y + SampleSpacingMeters, _terrain.DepthMeters)));

        // Light from the north-west, the cartographic convention.
        var slopeShade = 1.0 - (((east - h) - (north - h)) * 0.15);
        var tint = Mathf.Clamp((float)(h / 60.0), 0f, 1f);
        var baseColor = new Color(0.55f, 0.68f, 0.45f).Lerp(new Color(0.78f, 0.70f, 0.52f), tint);
        var shaded = baseColor * Mathf.Clamp((float)slopeShade, 0.6f, 1.3f);

        var onContour = Math.Floor(h / ContourIntervalMeters) != Math.Floor(Math.Max(east, north) / ContourIntervalMeters);
        return onContour ? shaded.Darkened(0.35f) : shaded;
    }
}
