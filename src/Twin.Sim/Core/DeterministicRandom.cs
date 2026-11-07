namespace Twin.Sim;

/// <summary>
/// Seeded pseudo-random generator (SplitMix64). Used instead of <see cref="System.Random"/>,
/// whose algorithm is not guaranteed to stay the same across .NET versions.
/// Same seed, same sequence, on every machine.
/// </summary>
public sealed class DeterministicRandom
{
    private ulong _state;

    /// <summary>Creates a generator from a seed.</summary>
    public DeterministicRandom(ulong seed) => _state = seed;

    /// <summary>Next 64-bit value.</summary>
    public ulong NextUInt64()
    {
        unchecked
        {
            _state += 0x9E3779B97F4A7C15UL;
            var z = _state;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
    }

    /// <summary>Uniform value in [0, 1).</summary>
    public double NextDouble() => (NextUInt64() >> 11) * (1.0 / (1UL << 53));

    /// <summary>Uniform integer in [0, maxExclusive).</summary>
    public int NextInt(int maxExclusive)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxExclusive);
        return (int)(NextUInt64() % (ulong)maxExclusive);
    }
}
