---
title: "std::maneuvers"
---

Collection of helper functions to plan and execute standard orbital maneuvers

# Functions


## bi_impulsive_transfer

```rust
pub sync fn bi_impulsive_transfer ( vessel : ksp::vessel::Vessel,
                                    target : ksp::orbit::Orbit,
                                    UT : float ) -> Result<ksp::vessel::ManeuverNode, string>
```



## bi_impulsive_transfer_near

```rust
pub sync fn bi_impulsive_transfer_near ( vessel : ksp::vessel::Vessel,
                                         target : ksp::orbit::Orbit,
                                         UT : float,
                                         TT : float ) -> Result<ksp::vessel::ManeuverNode, string>
```



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

## deltav_to_intercept_at

```rust
pub sync fn deltav_to_intercept_at ( start : ksp::orbit::Orbit,
                                     start_UT : float,
                                     target : ksp::orbit::Orbit,
                                     target_UT : float,
                                     offset_distance : float ) -> (start_velocity : ksp::math::Vec3, target_velocity : ksp::math::Vec3)
```



## ellipticize

```rust
pub sync fn ellipticize ( vessel : ksp::vessel::Vessel,
                          UT : float,
                          periapsis : float,
                          apoapsis : float ) -> Result<ksp::vessel::ManeuverNode, string>
```

Create a maneuver node at `UT` to change the current orbit of `vessel` to an elliptic orbit with
given `apoapsis` and `periapsis` (above sea level).

Will result in an error if maneuver nodes cannot be created
(e.g. because command or tracking facility has not been sufficiently upgraded)

## estimate_burn_time

```rust
pub sync fn estimate_burn_time ( vessel : ksp::vessel::Vessel,
                                 delta_v : float,
                                 stage_delay : float,
                                 throttle_limit : float ) -> (burn_time : float, half_burn_time : float)
```

Estimate the required burn time for a desired `delta_v` in vacuum.

* `stage_delay` is the assumed amount of seconds required for staging
* `throttle_limit` is a limit for the throttle to be considered

## exec_next_node

```rust
pub fn exec_next_node ( vessel : ksp::vessel::Vessel ) -> Result<Unit, string>
```

Execute the next planed maneuver node.

Will result in an error if there are no planed maneuver nodes.

## intercept_at

```rust
pub sync fn intercept_at ( vessel : ksp::vessel::Vessel,
                           start_UT : float,
                           target : ksp::orbit::Orbit,
                           target_UT : float,
                           offset_distance : float ) -> Result<ksp::vessel::ManeuverNode, string>
```

Create a maneuver node at `start_UT` to transfer from the current orbit of the `vessel` to
intercept an other `orbit` at `target_UT`

Will result in an error if maneuver nodes cannot be created
(e.g. because command or tracking facility has not been sufficiently upgraded)
