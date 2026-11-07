# 0004. 2.5D world model

- Status: Accepted
- Date: 2026-11-07

## Context
Terrain height drives tactics (observation, cover, dead ground), so a flat 2D model is not enough. A full 3D
world (meshes plus physics) costs more per query, is harder to keep deterministic, and is not needed for a
top-down operational view.

## Decision
The world is a heightmap (`ITerrain`, `HeightmapTerrain`): one ground height per map position, bilinearly
interpolated. Features such as buildings, vegetation and bridges will be added as objects with footprints and
heights, designed when the first feature needs them.

## Consequences
Fast, predictable queries. No overhangs, tunnels or stacked levels without explicit extra modelling: before
adding features such as bridges or multi-storey buildings, write an ADR on how they are represented.
