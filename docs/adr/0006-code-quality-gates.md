# 0006. Code quality gates and dependency policy

- Status: Accepted
- Date: 2026-11-07

## Decision
- Compiler warnings are errors in all projects except the Godot client, whose generated code we do not control.
- Code style is defined in `.editorconfig` and applied by `dotnet format`; CI verifies it. Style is never a review topic.
- Every NuGet version is pinned exactly in `Directory.Packages.props` (central package management).
- A package version must have been public for at least 14 days before we adopt it (supply-chain safety),
  and each upgrade is its own pull request.
- Prefer few dependencies. The scenario kit has its own assertions instead of an assertion library; this also
  avoids licence surprises (FluentAssertions 8, for example, moved to a commercial licence).
- NuGet vulnerability auditing is on; a vulnerable package fails the build.

## Consequences
Builds are reproducible and stricter. Occasionally a warning must be fixed before a change can merge.
