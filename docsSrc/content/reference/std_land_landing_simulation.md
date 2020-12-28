---
title: "std::land::landing_simulation"
---



# Types


## BodyParameters



### Fields

Name | Type | Description
--- | --- | ---
aerobraked_radius | float | 
angular_velocity | ksp::math::Vec3 | 
decel_radius | float | 
grav_parameter | float | 
speed_policy | fn(ksp::math::Vec3, ksp::math::Vec3) -> float | 

### Methods

#### find_freefall_end_time

```rust
bodyparameters.find_freefall_end_time ( orbit : ksp::orbit::Orbit,
                                        ut : float ) -> float
```



#### freefall_ended

```rust
bodyparameters.freefall_ended ( orbit : ksp::orbit::Orbit,
                                ut : float ) -> bool
```



#### grav_accel

```rust
bodyparameters.grav_accel ( pos : ksp::math::Vec3 ) -> ksp::math::Vec3
```



#### surface_velocity

```rust
bodyparameters.surface_velocity ( pos : ksp::math::Vec3,
                                  vel : ksp::math::Vec3 ) -> ksp::math::Vec3
```



#### total_accel

```rust
bodyparameters.total_accel ( pos : ksp::math::Vec3,
                             vel : ksp::math::Vec3 ) -> ksp::math::Vec3
```



## SimulationState



### Fields

Name | Type | Description
--- | --- | ---
body | std::land::landing_simulation::BodyParameters | 
deltav_expended | float | 
dt | float | 
max_thrust_accel | float | 
min_dt | float | 
t | float | 
v | ksp::math::Vec3 | 
x | ksp::math::Vec3 | 

### Methods

#### bs34_step

```rust
simulationstate.bs34_step ( ) -> Unit
```



#### limit_speed

```rust
simulationstate.limit_speed ( ) -> Unit
```



# Functions


## init_simulation

```rust
pub sync fn init_simulation ( vessel : ksp::vessel::Vessel,
                              start_ut : float,
                              start_dt : float,
                              min_dt : float,
                              max_thrust_accel : float,
                              decel_end_altitude_asl : float,
                              speed_policy : fn(ksp::math::Vec3, ksp::math::Vec3) -> float ) -> std::land::landing_simulation::SimulationState
```


