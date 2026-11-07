namespace Twin.Sim.Systems;

/// <summary>
/// One step of simulation logic, run once per tick in a fixed order.
/// Systems must be deterministic: same state in, same state out.
/// </summary>
public interface ISimSystem
{
    /// <summary>Advances this system by one tick of <paramref name="dtSeconds"/>.</summary>
    void Update(Simulation simulation, double dtSeconds);
}
