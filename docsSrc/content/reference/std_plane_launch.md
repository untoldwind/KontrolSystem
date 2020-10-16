---
title: "std::plane::launch"
---



# Constants

Name | Type | Description
--- | --- | ---
AirBreathingAlt | float | 
ClimbDefaultPitch | float | 
GTAltitude | float | 
TGTAirSpeed | float | 


# Functions


## plane_launch_ssto

```rust
pub fn plane_launch_ssto ( vessel : ksp::vessel::Vessel,
                           target_apoapsis : float,
                           heading : float ) -> Result<Unit, string>
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


