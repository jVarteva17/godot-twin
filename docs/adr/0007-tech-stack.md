# 0007. Tech stack

- Status: Accepted
- Date: 2026-11-07

## Decision
| Concern | Choice | Pinned in |
|---|---|---|
| Language | C# (latest), nullable enabled | `Directory.Build.props` |
| Runtime / SDK | .NET 10 (LTS) | `global.json` |
| Engine (client only) | Godot 4.7.2 .NET edition, Compatibility renderer | `.godot-version`, `Twin.Client.csproj` |
| Tests | xUnit 2 + our scenario kit | `Directory.Packages.props` |
| Dev tooling | .NET file-based apps (`./dev`, hooks) | `tools/`, `.claude/hooks/` |
| CI | GitHub Actions, Linux and Windows | `.github/workflows/ci.yml` |
| Map pipeline (later) | Python + GDAL/rasterio/geopandas, offline | `tools/` |

## Reasons
- Godot: strong 2D and UI, free MIT licence, light editor. Its weaker high-end 3D does not matter for a top-down view.
- C# for both simulation and client: one language, compiled performance, mature tooling, shared types.
- .NET 10 LTS: supported until late 2028; .NET 8 support ends in November 2026.
- Compatibility renderer: runs on modest and older hardware; a 2D map needs nothing more.
- Tooling as .NET file-based apps: the .NET SDK is the only thing everyone must install; no bash/PowerShell split.

## Alternatives considered
Unity (choose if very large unit counts need DOTS, or for console-first plans), C++ via GDExtension (only for
proven hotspots), Rust/Bevy (immature editor and UI), Unreal (poor fit for 2D/UI-heavy work).

## Consequences
Godot's C# support must be checked when upgrading Godot. Web export of C# projects is limited; if a browser
build becomes a requirement, revisit this ADR.
