---
title: "std::maneuvers"
---

Collection of helper functions to plan and execute standard orbital maneuvers

# Functions


## bi_impulsive_transfer

```rust
pub sync fn bi_impulsive_transfer ( start : ksp::orbit::Orbit,
                                    target : ksp::orbit::Orbit,
                                    minUT : float ) -> Result<(delta_v : ksp::math::Vec3, UT : float), string>
```

Calculate delta-v to intercept a `target` orbit starting after `minUT`.
Actual starting and transfer time will be optimized for a minimal delta-v for
acceleration and deacceleration.

Will result in an error if maneuver nodes cannot be created
(e.g. because command or tracking facility has not been sufficiently upgraded)

## bi_impulsive_transfer_near

```rust
pub sync fn bi_impulsive_transfer_near ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Orbit,
                                         UT : float,
                                         TT : float ) -> Result<(delta_v : ksp::math::Vec3, UT : float), string>
```

Calculate delta-v to intercept a `target` orbit starting nearly at time `UT` with
a nearly transfer time of `TT`. `UT` and `TT` will be optimized for a minimal delta-v for
acceleration and deacceleration.

## change_apoapsis

```rust
pub sync fn change_apoapsis ( orbit : ksp::orbit::Orbit,
                              UT : float,
                              apoapsis_radius : float ) -> Result<ksp::math::Vec3, string>
```

Calculate the required delta-v to change the apoapsis of an `orbit`
to `apoapsis_radius` at time `UT`

## change_periapsis

```rust
pub sync fn change_periapsis ( orbit : ksp::orbit::Orbit,
                               UT : float,
                               periapsis_radius : float ) -> Result<ksp::math::Vec3, string>
```

Calculate the required delta-v to change the periapsis of an `orbit`
to `periapsis_radius` at time `UT`

## circularize_orbit

```rust
pub sync fn circularize_orbit ( orbit : ksp::orbit::Orbit ) -> (delta_v : ksp::math::Vec3, UT : float)
```

Calculate the required delta-v and time to change the given `orbit`
to a (mostly) circular orbit at the next apoapsis (if `orbit` is elliplic)
or periapsis (if `orbit` is hyperbolic).

## circularize_orbit_at

```rust
pub sync fn circularize_orbit_at ( orbit : ksp::orbit::Orbit,
                                   UT : float ) -> ksp::math::Vec3
```

Calculate the required delta-v to change the given `orbit`
to a (mostly) circular orbit at a given universal time `UT`.

## ellipticize

```rust
pub sync fn ellipticize ( orbit : ksp::orbit::Orbit,
                          UT : float,
                          periapsis : float,
                          apoapsis : float ) -> ksp::math::Vec3
```

Calculate the required delta-v to change the `apoapsis` and `periapsis` of the given `orbit`
at time `UT`.

## ideal_ejection

```rust
pub sync fn ideal_ejection ( body : ksp::orbit::Body,
                             UT : float,
                             radius : float,
                             normal : ksp::math::Vec3,
                             exit_velocity : ksp::math::Vec3 ) -> ksp::orbit::Orbit
```

Calculate the ideal ejection from a (nearly) circular orbit around a given `body`, `radius` and `normal` vector.
The resulting orbit is choosen so that the vessel will have a given `exit_velocity` on the SOI radius at time `UT`.

## intercept_at

```rust
pub sync fn intercept_at ( start : ksp::orbit::Orbit,
                           start_UT : float,
                           target : ksp::orbit::Orbit,
                           target_UT : float,
                           offset_distance : float ) -> (start_velocity : ksp::math::Vec3, target_velocity : ksp::math::Vec3)
```

Calculate required delta-v to intercept `target` orbit at time `target_UT` from `start` orbit at time `start_UT`.
`offset_distance` may be used to define a desired distance to the target.

## next_closest_approach_time

```rust
pub sync fn next_closest_approach_time ( start : ksp::orbit::Orbit,
                                         target : ksp::orbit::Orbit,
                                         UT : float ) -> float
```


