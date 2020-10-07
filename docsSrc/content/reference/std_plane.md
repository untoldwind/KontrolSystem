---
title: "std::plane"
---



# Functions


## plane_fly_to

```rust
pub fn plane_fly_to ( vessel : ksp::vessel::Vessel,
                      target_vec : ksp::math::Vec3,
                      target_speed : float ) -> Result<Unit, string>
```



## plane_heading_of

```rust
pub sync fn plane_heading_of ( vessel : ksp::vessel::Vessel,
                               face : ksp::math::Vec3 ) -> float
```



## plane_launch_ssto

```rust
pub fn plane_launch_ssto ( target_apoapsis : float,
                           heading : float ) -> Result<Unit, string>
```



## plane_radar_altimeter

```rust
pub sync fn plane_radar_altimeter ( vessel : ksp::vessel::Vessel ) -> float
```



## plane_rel_vec

```rust
pub sync fn plane_rel_vec ( vessel : ksp::vessel::Vessel,
                            face : ksp::math::Vec3 ) -> ksp::math::Vec3
```



## plane_ssto_atmo_ascent

```rust
pub fn plane_ssto_atmo_ascent ( vessel : ksp::vessel::Vessel,
                                heading : float ) -> Result<Unit, string>
```



## plane_ssto_leave_atmo

```rust
pub fn plane_ssto_leave_atmo ( vessel : ksp::vessel::Vessel,
                               target_apoapsis : float,
                               heading : float ) -> Result<Unit, string>
```



## plane_takeoff

```rust
pub fn plane_takeoff ( vessel : ksp::vessel::Vessel,
                       takeoff_speed : float,
                       heading : float ) -> Result<Unit, string>
```

Perform a takeoff of a plane.

The function will end once the plane has reached an altitude of 500 above ground.
The plane will crash if no further action is taken.
