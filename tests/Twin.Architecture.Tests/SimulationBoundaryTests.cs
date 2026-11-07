using System.Reflection;
using System.Text.RegularExpressions;

namespace Twin.Architecture.Tests;

/// <summary>
/// Turns the most important architecture rules into failing tests, so they hold no matter
/// who (or what) writes the code. Rules: docs/adr/0002-simulation-presentation-separation.md
/// and docs/adr/0005-determinism.md.
/// </summary>
public class SimulationBoundaryTests
{
    private static readonly Assembly SimAssembly = typeof(Twin.Sim.Simulation).Assembly;

    [Fact]
    public void Simulation_references_only_the_dotnet_base_library()
    {
        var offending = SimAssembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(name => !(name.StartsWith("System", StringComparison.Ordinal)
                             || name is "netstandard" or "mscorlib"))
            .ToList();

        if (offending.Count > 0)
        {
            Assert.Fail("Twin.Sim must stay engine-independent, but it references: " + string.Join(", ", offending) +
                        ". Move engine-specific code to Twin.Client (see docs/adr/0002).");
        }
    }

    [Fact]
    public void Simulation_source_uses_no_nondeterministic_apis()
    {
        var violations = new List<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(RepoRoot(), "src", "Twin.Sim"), "*.cs", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            {
                continue;
            }

            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].TrimStart().StartsWith("///", StringComparison.Ordinal) ||
                    lines[i].TrimStart().StartsWith("//", StringComparison.Ordinal) ||
                    lines[i].Contains("determinism-ok", StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var (pattern, reason) in BannedApis)
                {
                    if (pattern.IsMatch(lines[i]))
                    {
                        violations.Add($"{Path.GetRelativePath(RepoRoot(), file)}:{i + 1}: {reason}");
                    }
                }
            }
        }

        if (violations.Count > 0)
        {
            Assert.Fail("Non-deterministic API used in the simulation (docs/adr/0005):" + Environment.NewLine +
                        string.Join(Environment.NewLine, violations));
        }
    }

    private static readonly (Regex Pattern, string Reason)[] BannedApis =
    [
        (new Regex(@"\bDateTime(Offset)?\.(Now|UtcNow|Today)\b"), "wall-clock time; use Simulation.TimeSeconds"),
        (new Regex(@"\bStopwatch\b"), "wall-clock time; use Simulation.TimeSeconds"),
        (new Regex(@"\bEnvironment\.TickCount"), "wall-clock time; use Simulation.TimeSeconds"),
        (new Regex(@"\bnew\s+Random\s*\(|\bRandom\.Shared\b"), "System.Random; use Simulation.Random"),
        (new Regex(@"\bGuid\.NewGuid\b"), "random ids; derive ids from inputs"),
        (new Regex(@"\b(Task\.Run|Parallel\.|ThreadPool\.|new\s+Thread\s*\()"), "threads make ordering non-deterministic"),
        (new Regex(@"\bHashSet<|\bDictionary<"), "unordered iteration; use SortedDictionary/List, or justify with // determinism-ok"),
        (new Regex(@"\busing\s+Godot\b"), "engine dependency"),
    ];

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Twin.sln")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Could not find Twin.sln above " + AppContext.BaseDirectory);
    }
}
