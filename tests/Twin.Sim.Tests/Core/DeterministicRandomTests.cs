namespace Twin.Sim.Tests.Core;

public class DeterministicRandomTests
{
    [Fact]
    public void Same_seed_gives_same_sequence()
    {
        var a = new DeterministicRandom(7);
        var b = new DeterministicRandom(7);

        for (var i = 0; i < 1000; i++)
        {
            Assert.Equal(a.NextUInt64(), b.NextUInt64());
        }
    }

    [Fact]
    public void Different_seeds_give_different_sequences()
    {
        Assert.NotEqual(new DeterministicRandom(1).NextUInt64(), new DeterministicRandom(2).NextUInt64());
    }

    [Fact]
    public void Known_seed_gives_known_first_value()
    {
        // Pins the algorithm itself: if this changes, saved replays and recorded scenarios break.
        Assert.Equal(0x910A2DEC89025CC1UL, new DeterministicRandom(1).NextUInt64());
    }

    [Fact]
    public void NextDouble_is_in_unit_interval()
    {
        var random = new DeterministicRandom(3);
        for (var i = 0; i < 10_000; i++)
        {
            Assert.InRange(random.NextDouble(), 0.0, 0.9999999999999999);
        }
    }
}
