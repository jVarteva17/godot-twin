using Godot;
using Twin.Sim;
using Twin.Sim.Orders;

namespace Twin.Client;

/// <summary>Draws units, their current destinations and the selection. Pure presentation.</summary>
public partial class UnitLayer : Node2D
{
    private readonly Simulation _simulation = null!;
    private readonly CoordinateMapper _mapper = null!;

    /// <summary>Required by Godot. Not used directly.</summary>
    public UnitLayer()
    {
    }

    /// <summary>Creates the layer.</summary>
    public UnitLayer(Simulation simulation, CoordinateMapper mapper)
    {
        _simulation = simulation;
        _mapper = mapper;
    }

    /// <summary>Id of the selected unit, if any.</summary>
    public string? SelectedUnitId { get; set; }

    /// <inheritdoc />
    public override void _Draw()
    {
        var font = ThemeDB.FallbackFont;
        foreach (var unit in _simulation.Units)
        {
            var at = _mapper.ToScreen(unit.Position.Xy);
            var selected = unit.Id == SelectedUnitId;

            if (unit.CurrentOrder is MoveTo move)
            {
                var target = _mapper.ToScreen(move.Destination);
                DrawDashedLine(at, target, new Color(0.1f, 0.2f, 0.6f, 0.8f), 2f, 8f);
                DrawCircle(target, 4f, new Color(0.1f, 0.2f, 0.6f));
            }

            // Placeholder symbol: a friendly-force rectangle (NATO style comes later).
            var box = new Rect2(at - new Vector2(10, 7), new Vector2(20, 14));
            DrawRect(box, new Color(0.5f, 0.75f, 1f));
            DrawRect(box, selected ? Colors.Yellow : Colors.Black, filled: false, width: selected ? 3f : 1.5f);
            DrawString(font, at + new Vector2(13, 5), $"{unit.Id} {unit.Position.Z:0}m", fontSize: 13, modulate: Colors.Black);
        }
    }
}
