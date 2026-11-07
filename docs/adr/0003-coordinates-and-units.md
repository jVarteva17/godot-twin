# 0003. Coordinates and units

- Status: Accepted
- Date: 2026-11-07

## Context
"Go to X, Y, Z" must mean the same thing to everyone. Godot's 2D space is Y-down in pixels and its 3D space is
Y-up; GIS data and military practice are usually X east, Y north, Z up in meters.

## Decision
- The simulation uses SI units: meters, seconds, meters per second.
- Axes: X east, Y north, Z up. The map spans [0, Width] × [0, Depth]; Z is height above the map datum.
- `Vec2` is a map position, `Vec3` a position in space. Ground units' Z always equals the terrain height.
- Conversion to screen coordinates exists in exactly one place: `src/Twin.Client/CoordinateMapper.cs`.

## Consequences
Test expectations are written in real-world terms. Real map data (projected coordinates such as UTM) can be
mapped with an offset. Anyone touching Godot code must go through the mapper.
