---
title: "std::utils"
---

Collection of helper functions not fitting anywhere else

# Functions


## angle_to_360

```rust
pub sync fn angle_to_360 ( angle : float ) -> float
```



## remove_all_nodes

```rust
pub sync fn remove_all_nodes ( vessel : ksp::vessel::Vessel ) -> Unit
```

Removes all maneuvering nodes from the flight plan of a `vessel`.

## ship_is_facing

```rust
pub sync fn ship_is_facing ( vessel : ksp::vessel::Vessel,
                             desired_facing : ksp::math::Vec3,
                             max_deviation_degrees : float,
                             max_angular_velocity : float ) -> bool
```

Determine if `vessel` is facing a given direction.

* `max_deviation_degrees` sets a limit how many degrees the angle may differ
* `max_angular_velocity` sets a limit how much the `vessel` may still be turning

## vessel_heading_of

```rust
pub sync fn vessel_heading_of ( vessel : ksp::vessel::Vessel,
                                face : ksp::math::Vec3 ) -> float
```



## vessel_pitch

```rust
pub sync fn vessel_pitch ( vessel : ksp::vessel::Vessel ) -> float
```



## vessel_radar_altimeter

```rust
pub sync fn vessel_radar_altimeter ( vessel : ksp::vessel::Vessel ) -> float
```


