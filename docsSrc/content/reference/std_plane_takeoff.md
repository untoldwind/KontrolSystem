---
title: "std::plane::takeoff"
---



# Functions


## plane_takeoff

```rust
pub fn plane_takeoff ( vessel : ksp::vessel::Vessel,
                       takeoff_speed : float,
                       heading : float ) -> Result<Unit, string>
```

Perform a takeoff of a plane.

The function will end once the plane has reached an altitude of 500 above ground.
The plane will crash if no further action is taken.
