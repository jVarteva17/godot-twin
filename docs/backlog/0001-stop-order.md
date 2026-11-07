# 0001. Stop order (onboarding validation feature)

**Purpose:** besides adding a feature, this ticket tests whether the walking skeleton works for someone who did
not build it. Implement it using only the docs, the reference feature and the harness. Write every point of
confusion or friction in `docs/onboarding-notes.md` as you go; fixing those is part of the task.

## Story
As an operator, I can order a moving unit to stop, so I can halt it where it is.

## Acceptance criteria
1. A unit moving toward a destination that receives `Stop` is idle after the next tick, with zero velocity.
2. It stops where it is: its position after stopping is within one tick's travel of where it was when the
   order took effect, and on the ground.
3. Stopping an idle unit is accepted and has no effect.
4. A `MoveTo` issued after a `Stop` works normally.
5. `Stop` for an unknown unit is rejected with `OrderRejectedException`.

## Notes
- New order type in `src/Twin.Sim/Orders`; handling belongs in a system, not in the client.
- Optional client part: press `S` to stop the selected unit.

## Done when
Definition of done in AGENTS.md is met, and `docs/onboarding-notes.md` exists with findings and the fixes made.
