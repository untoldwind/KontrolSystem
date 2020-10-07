---
title: "std::atmo"
---



# Functions


## atmo_launch

```rust
pub fn atmo_launch ( target_apoapsis : float,
                     heading : float ) -> Result<Unit, string>
```

Automatically launch a rocket from an atmosphere to a circular orbit.

## atmo_launch_ascent

```rust
pub fn atmo_launch_ascent ( vessel : ksp::vessel::Vessel,
                            target_apoapsis : float,
                            heading : float ) -> Unit
```

Perform a rocket launch ascent from an atmosphere.

Note: The rocket will not end up in a stable orbit and most likely crash if no further action
is taken.
