using Godot;
using Twin.Sim;
using Twin.Sim.Geometry;
using Twin.Sim.Orders;
using Twin.Sim.Terrain;

namespace Twin.Client;

/// <summary>
/// Owns the simulation and drives it at its fixed tick rate, independent of the frame rate.
/// Translates mouse input into orders. Contains no game rules.
/// Controls: left click selects a unit, right click orders it to move, mouse wheel zooms.
/// </summary>
public partial class SimulationHost : Node2D
{
    private const double MaxFrameCatchUpSeconds = 0.25;
    private const double SelectRadiusMeters = 25;

    private Simulation _simulation = null!;
    private CoordinateMapper _mapper = null!;
    private UnitLayer _units = null!;
    private Camera2D _camera = null!;
    private Label _status = null!;
    private double _accumulator;

    /// <inheritdoc />
    public override void _Ready()
    {
        var terrain = HeightmapTerrain.Hill(1000, 1000, peak: new Vec2(600, 550), peakHeightMeters: 40, radiusMeters: 300);
        _simulation = new Simulation(terrain, new SimulationConfig(TickRateHz: 20, Seed: 1));
        _simulation.AddUnit("alpha", new Vec2(150, 150), maxSpeedMps: 5);
        _simulation.AddUnit("bravo", new Vec2(250, 120), maxSpeedMps: 8);

        _mapper = new CoordinateMapper(terrain.WidthMeters, terrain.DepthMeters, pixelsPerMeter: 0.8f);
        AddChild(new MapView(terrain, _mapper));
        _units = new UnitLayer(_simulation, _mapper) { SelectedUnitId = "alpha" };
        AddChild(_units);

        _camera = new Camera2D { Position = _mapper.ToScreen(new Vec2(500, 500)) };
        AddChild(_camera);

        var hud = new CanvasLayer();
        _status = new Label { Position = new Vector2(12, 8) };
        _status.AddThemeColorOverride("font_color", Colors.Black);
        hud.AddChild(_status);
        AddChild(hud);
    }

    /// <inheritdoc />
    public override void _Process(double delta)
    {
        // Fixed-step loop: the simulation always advances in whole ticks.
        _accumulator += Math.Min(delta, MaxFrameCatchUpSeconds);
        var tick = _simulation.Config.TickSeconds;
        while (_accumulator >= tick)
        {
            _simulation.Step();
            _accumulator -= tick;
        }

        _units.QueueRedraw();
        _status.Text = $"t = {_simulation.TimeSeconds:0.0} s   selected: {_units.SelectedUnitId ?? "none"}\n" +
                       "left click: select   right click: move   wheel: zoom";
    }

    /// <inheritdoc />
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { Pressed: true } click)
        {
            return;
        }

        var mapPosition = _mapper.ToMap(GetGlobalMousePosition());
        switch (click.ButtonIndex)
        {
            case MouseButton.Left:
                _units.SelectedUnitId = _simulation.Units
                    .Where(u => u.Position.Xy.DistanceTo(mapPosition) <= SelectRadiusMeters)
                    .OrderBy(u => u.Position.Xy.DistanceTo(mapPosition))
                    .Select(u => u.Id)
                    .FirstOrDefault();
                break;
            case MouseButton.Right when _units.SelectedUnitId is { } id && _simulation.Terrain.Contains(mapPosition):
                _simulation.Issue(id, new MoveTo(mapPosition));
                break;
            case MouseButton.WheelUp:
                _camera.Zoom *= 1.1f;
                break;
            case MouseButton.WheelDown:
                _camera.Zoom /= 1.1f;
                break;
        }
    }
}
