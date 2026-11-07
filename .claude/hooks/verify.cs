#:property PublishAot=false
// Claude Code Stop hook: the agent may not finish while the build or tests are red.
//   implement phase: formats the code, runs the fast test suite, blocks finishing on failure.
//   spec phase:      red tests are expected; reminds the agent to hand the tests to a human.
// Gives up after 3 consecutive blocks so a stuck agent hands the problem to the human instead of looping.
using System.Diagnostics;

const int MaxConsecutiveBlocks = 3;
var root = Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR") ?? Directory.GetCurrentDirectory();
_ = Console.In.ReadToEnd();
var stateDir = Path.Combine(root, ".claude", "state");
var counterFile = Path.Combine(stateDir, "stop-blocks");
var phaseFile = Path.Combine(stateDir, "phase");
var phase = File.Exists(phaseFile) ? File.ReadAllText(phaseFile).Trim() : "implement";

// Nothing changed in the working tree: a conversation, not a code change. Nothing to verify.
if (string.IsNullOrWhiteSpace(Run("git", "status --porcelain -- src tests tools").Output))
{
    return 0;
}

if (phase == "spec")
{
    Console.Error.WriteLine("Spec phase: before finishing, list the new or changed tests, run ./dev test to show they " +
                            "fail for the expected reason, and ask the human to review them and run ./dev phase implement.");
    return ResetAnd(0);
}

Run("dotnet", "format Twin.Core.slnf");
var test = Run("dotnet", "test Twin.Core.slnf --nologo");
if (test.ExitCode == 0)
{
    return ResetAnd(0);
}

var blocks = (File.Exists(counterFile) && int.TryParse(File.ReadAllText(counterFile), out var b) ? b : 0) + 1;
if (blocks > MaxConsecutiveBlocks)
{
    Console.Error.WriteLine($"Tests are still failing after {MaxConsecutiveBlocks} attempts. Stopping: explain to the " +
                            "human what fails and why, instead of trying again.");
    return ResetAnd(0);
}

Directory.CreateDirectory(stateDir);
File.WriteAllText(counterFile, blocks.ToString(System.Globalization.CultureInfo.InvariantCulture));
var tail = string.Join('\n', test.Output.Split('\n').Where(l => l.Length > 0).TakeLast(60));
Console.Error.WriteLine($"Tests or build are red (attempt {blocks}/{MaxConsecutiveBlocks}). Do not finish yet. " +
                        $"Fix src/ (not the tests) until ./dev test passes.\n{tail}");
return 2;

int ResetAnd(int code)
{
    if (File.Exists(counterFile))
    {
        File.Delete(counterFile);
    }

    return code;
}

(int ExitCode, string Output) Run(string file, string arguments)
{
    var info = new ProcessStartInfo(file, arguments)
    {
        WorkingDirectory = root,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
    };
    using var process = Process.Start(info)!;
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    process.WaitForExit();
    return (process.ExitCode, stdout.Result + stderr.Result);
}
