#:property PublishAot=false
// Claude Code PreToolUse hook: enforces the TDD phases (docs/workflow.md).
//   spec phase:      tests may change, src/ is locked (write the failing test first)
//   implement phase: src/ is open, existing tests are locked (no weakening the spec)
// Always: the harness itself (.claude/hooks, .claude/settings.json) is human-only.
// Exit code 2 blocks the tool call and shows the message to the agent.
// This is a guard rail, not a security boundary: human review of tests and CI are the real gates.
using System.Text.Json;
using System.Text.RegularExpressions;

var root = Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR") ?? Directory.GetCurrentDirectory();
using var input = JsonDocument.Parse(Console.In.ReadToEnd());
var tool = input.RootElement.GetProperty("tool_name").GetString() ?? string.Empty;
var toolInput = input.RootElement.GetProperty("tool_input");
var phaseFile = Path.Combine(root, ".claude", "state", "phase");
var phase = File.Exists(phaseFile) ? File.ReadAllText(phaseFile).Trim() : "implement";

if (tool == "Bash")
{
    var command = toolInput.TryGetProperty("command", out var c) ? c.GetString() ?? string.Empty : string.Empty;
    var writes = Regex.IsMatch(command, @"(>|\bsed\s+-i|\btee\b|\bmv\b|\brm\b|\bcp\b|\bperl\s+-[a-z]*i)");
    if (Regex.IsMatch(command, @"\bphase\s+(spec|implement)\b") ||
        (writes && Regex.IsMatch(command, @"\.claude/(hooks|settings\.json|state)")))
    {
        return Block("The AI harness and the TDD phase are changed only by a human (./dev phase ...).");
    }

    if (writes && phase == "implement" && Regex.IsMatch(command, @"\btests/"))
    {
        return Block("Shell commands that modify tests/ are blocked in the implement phase. Existing tests are the agreed specification.");
    }

    return 0;
}

var path = toolInput.TryGetProperty("file_path", out var p) ? p.GetString() ?? string.Empty
         : toolInput.TryGetProperty("notebook_path", out var n) ? n.GetString() ?? string.Empty : string.Empty;
if (path.Length == 0)
{
    return 0;
}

var full = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(root, path));
var relative = Path.GetRelativePath(root, full).Replace('\\', '/');

if (relative.StartsWith(".claude/hooks/", StringComparison.Ordinal) ||
    relative is ".claude/settings.json" ||
    relative.StartsWith(".claude/state/", StringComparison.Ordinal))
{
    return Block("The AI harness and the TDD phase are changed only by a human. Propose the change instead.");
}

if (phase == "spec" && relative.StartsWith("src/", StringComparison.Ordinal))
{
    return Block("TDD phase is 'spec': write or adjust the failing test first. src/ is locked until a human " +
                 "has reviewed the tests and run ./dev phase implement.");
}

if (phase == "implement" && relative.StartsWith("tests/", StringComparison.Ordinal) && File.Exists(full))
{
    return Block($"TDD phase is 'implement': '{relative}' is an existing test file and is part of the agreed " +
                 "specification. Make it pass by changing src/. You may add NEW test files. If you believe the " +
                 "test is wrong, stop and explain why to the human; they can switch to ./dev phase spec.");
}

return 0;

static int Block(string reason)
{
    Console.Error.WriteLine(reason);
    return 2;
}
