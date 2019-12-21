---
title: "std::maneuvers"
---

Collection of helper functions to plan and execute standard orbital maneuvers

# Functions


## circularize_orbit

```rust
pub sync fn circularize_orbit ( vessel : ksp::vessel::Vessel ) -> Result<ksp::vessel::ManeuverNode, string>
```

Create a maneuver node to change to a (mostly) circular orbit at then next
apoapsis (from elliplic orbit) or periapsis (from hyperbolic orbit).

Will result in an error if maneuver nodes cannot be created
(e.g. because command or tracking facility has not been sufficiently upgraded)

## circularize_orbit_at

```rust
pub sync fn circularize_orbit_at ( vessel : ksp::vessel::Vessel,
                                   UT : float ) -> Result<ksp::vessel::ManeuverNode, string>
```

Create a maneuver node to change to a (mostly) circular orbit at a given universal time `UT`.

Will result in an error if maneuver nodes cannot be created
(e.g. because command or tracking facility has not been sufficiently upgraded)

## exec_next_node

```rust
pub fn exec_next_node ( vessel : ksp::vessel::Vessel ) -> Result<Unit, string>
```

Execute the next planed maneuver node.

Will result in an error if there are no planed maneuver nodes.
