namespace Twin.Sim;

/// <summary>Fixed parameters of a simulation run. See docs/adr/0005-determinism.md.</summary>
/// <param name="TickRateHz">Simulation updates per simulated second. Independent of frame rate.</param>
/// <param name="Seed">Seed for all randomness in the run.</param>
public sealed record SimulationConfig(int TickRateHz = 20, ulong Seed = 1)
{
    /// <summary>Duration of one tick in seconds.</summary>
    public double TickSeconds => 1.0 / TickRateHz;
}
