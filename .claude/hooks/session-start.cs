#:property PublishAot=false
// Claude Code SessionStart hook: everything printed here is added to the agent's context.
var root = Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR") ?? Directory.GetCurrentDirectory();
_ = Console.In.ReadToEnd();
var phaseFile = Path.Combine(root, ".claude", "state", "phase");
var phase = File.Exists(phaseFile) ? File.ReadAllText(phaseFile).Trim() : "implement";
Console.WriteLine($"Current TDD phase: {phase}. " + (phase == "spec"
    ? "Only tests may be written now; src/ is locked."
    : "Implement against the existing tests; do not modify existing test files."));
Console.WriteLine("Follow AGENTS.md. Use the tdd-feature skill for any behaviour change. Run tests with ./dev test.");
return 0;
