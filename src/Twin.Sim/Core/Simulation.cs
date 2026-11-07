using Twin.Sim.Geometry;
using Twin.Sim.Orders;
using Twin.Sim.Systems;
using Twin.Sim.Terrain;
using Twin.Sim.Units;

namespace Twin.Sim;

/// <summary>
/// The digital twin's simulation: world state plus the fixed-order systems that advance it.
/// Time only moves when <see cref="Step"/> is called, so callers (tests, the Godot client,
/// the headless runner) control time explicitly.
/// </summary>
public sealed class Simulation
{
    private readonly SortedDictionary<string, Unit> _units = new(StringComparer.Ordinal);
    private readonly List<(string UnitId, Order Order)> _pendingOrders = [];
    private readonly IReadOnlyList<ISimSystem> _systems;

    /// <summary>Creates a simulation over the given terrain.</summary>
    public Simulation(ITerrain terrain, SimulationConfig? config = null)
    {
        ArgumentNullException.ThrowIfNull(terrain);
        Terrain = terrain;
        Config = config ?? new SimulationConfig();
        Random = new DeterministicRandom(Config.Seed);

        // System order is part of the simulation's behaviour. Change it deliberately.
        _systems = [new MovementSystem()];
    }

    /// <summary>The ground surface.</summary>
    public ITerrain Terrain { get; }

    /// <summary>Run parameters.</summary>
    public SimulationConfig Config { get; }

    /// <summary>The only source of randomness systems may use.</summary>
    public DeterministicRandom Random { get; }

    /// <summary>Number of ticks completed.</summary>
    public long Tick { get; private set; }

    /// <summary>Simulated time in seconds.</summary>
    public double TimeSeconds => Tick * Config.TickSeconds;

    /// <summary>All units, ordered by id (stable iteration order is required for determinism).</summary>
    public IReadOnlyCollection<Unit> Units => _units.Values;

    /// <summary>Places a ground unit on the map. Its height is taken from the terrain.</summary>
    public Unit AddUnit(string id, Vec2 position, double maxSpeedMps)
    {
        if (_units.ContainsKey(id))
        {
            throw new ArgumentException($"A unit with id '{id}' already exists.", nameof(id));
        }

        if (!Terrain.Contains(position))
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, "Unit must be placed on the map.");
        }

        var unit = new Unit(id, position.WithZ(Terrain.HeightAt(position)), maxSpeedMps);
        _units.Add(id, unit);
        return unit;
    }

    /// <summary>Gets a unit by id.</summary>
    public Unit GetUnit(string id) =>
        _units.TryGetValue(id, out var unit) ? unit : throw new KeyNotFoundException($"No unit with id '{id}'.");

    /// <summary>
    /// Issues an order. It is validated immediately and takes effect at the start of the next tick.
    /// A new order replaces the unit's current order.
    /// </summary>
    /// <exception cref="OrderRejectedException">The unit does not exist or the order is invalid.</exception>
    public void Issue(string unitId, Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (!_units.ContainsKey(unitId))
        {
            throw new OrderRejectedException($"No unit with id '{unitId}'.");
        }

        if (order is MoveTo move && !Terrain.Contains(move.Destination))
        {
            throw new OrderRejectedException($"Destination {move.Destination} is outside the map.");
        }

        _pendingOrders.Add((unitId, order));
    }

    /// <summary>Advances the simulation by exactly one tick.</summary>
    public void Step()
    {
        Tick++;
        foreach (var (unitId, order) in _pendingOrders)
        {
            _units[unitId].CurrentOrder = order;
        }

        _pendingOrders.Clear();
        foreach (var system in _systems)
        {
            system.Update(this, Config.TickSeconds);
        }
    }

    /// <summary>
    /// A fingerprint of the complete simulation state. Two runs with the same inputs must
    /// produce the same hash at every tick (checked by the determinism tests).
    /// </summary>
    public ulong ComputeStateHash()
    {
        var hash = new StateHasher();
        hash.Add(Tick);
        foreach (var unit in _units.Values)
        {
            hash.Add(unit.Id);
            hash.Add(unit.Position);
            hash.Add(unit.Velocity);
            hash.Add((long)unit.Activity);
            hash.Add(unit.CurrentOrder?.ToString() ?? "none");
        }

        return hash.Value;
    }

    /// <summary>FNV-1a over the exact bit patterns of the state.</summary>
    private sealed class StateHasher
    {
        public ulong Value { get; private set; } = 14695981039346656037UL;

        public void Add(long value)
        {
            for (var i = 0; i < 8; i++)
            {
                AddByte((byte)(value >> (i * 8)));
            }
        }

        public void Add(double value) => Add(BitConverter.DoubleToInt64Bits(value));

        public void Add(Vec3 v)
        {
            Add(v.X);
            Add(v.Y);
            Add(v.Z);
        }

        public void Add(string value)
        {
            foreach (var c in value)
            {
                Add(c);
            }

            AddByte(0);
        }

        private void AddByte(byte b)
        {
            unchecked
            {
                Value = (Value ^ b) * 1099511628211UL;
            }
        }
    }
}
