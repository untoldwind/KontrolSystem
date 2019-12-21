---
title: "ksp::vessel"
---



# Types


## ActionGroups



### Fields

Name | Type | Description
--- | --- | ---
abort | bool | 
antennas | bool | 
breaks | bool | 
custom1 | bool | 
custom10 | bool | 
custom2 | bool | 
custom3 | bool | 
custom4 | bool | 
custom5 | bool | 
custom6 | bool | 
custom7 | bool | 
custom8 | bool | 
custom9 | bool | 
gear | bool | 
light | bool | 
panels | bool | 
radiators | bool | 
rcs | bool | 
sas | bool | 

### Methods

#### deploy_fairings

```rust
actiongroups.deploy_fairings ( ) -> Unit
```



#### set_abort

```rust
actiongroups.set_abort ( value : bool ) -> Unit
```



#### set_antennas

```rust
actiongroups.set_antennas ( value : bool ) -> Unit
```



#### set_breaks

```rust
actiongroups.set_breaks ( value : bool ) -> Unit
```



#### set_custom1

```rust
actiongroups.set_custom1 ( value : bool ) -> Unit
```



#### set_custom10

```rust
actiongroups.set_custom10 ( value : bool ) -> Unit
```



#### set_custom2

```rust
actiongroups.set_custom2 ( value : bool ) -> Unit
```



#### set_custom3

```rust
actiongroups.set_custom3 ( value : bool ) -> Unit
```



#### set_custom4

```rust
actiongroups.set_custom4 ( value : bool ) -> Unit
```



#### set_custom5

```rust
actiongroups.set_custom5 ( value : bool ) -> Unit
```



#### set_custom6

```rust
actiongroups.set_custom6 ( value : bool ) -> Unit
```



#### set_custom7

```rust
actiongroups.set_custom7 ( value : bool ) -> Unit
```



#### set_custom8

```rust
actiongroups.set_custom8 ( value : bool ) -> Unit
```



#### set_custom9

```rust
actiongroups.set_custom9 ( value : bool ) -> Unit
```



#### set_gear

```rust
actiongroups.set_gear ( value : bool ) -> Unit
```



#### set_light

```rust
actiongroups.set_light ( value : bool ) -> Unit
```



#### set_panels

```rust
actiongroups.set_panels ( value : bool ) -> Unit
```



#### set_radiators

```rust
actiongroups.set_radiators ( value : bool ) -> Unit
```



#### set_rcs

```rust
actiongroups.set_rcs ( value : bool ) -> Unit
```



#### set_sas

```rust
actiongroups.set_sas ( value : bool ) -> Unit
```



## DeltaVEngineInfo



### Fields

Name | Type | Description
--- | --- | ---
engine | ksp::vessel::ModuleEngines | 
i_s_p_a_s_l | float | 
i_s_p_actual | float | 
i_s_p_vac | float | 
start_burn_stage | int | 

## Maneuver



### Fields

Name | Type | Description
--- | --- | ---
available | bool | 
nodes | ksp::vessel::ManeuverNode[] | 
patch_limit | int | 

### Methods

#### add

```rust
maneuver.add ( UT : float,
               radialOut : float,
               normal : float,
               prograde : float ) -> Result<ksp::vessel::ManeuverNode, string>
```



#### add_burn_vector

```rust
maneuver.add_burn_vector ( UT : float,
                           burnVector : ksp::math::Vec3 ) -> Result<ksp::vessel::ManeuverNode, string>
```



#### next_node

```rust
maneuver.next_node ( ) -> Result<ksp::vessel::ManeuverNode, string>
```



## ManeuverNode



### Fields

Name | Type | Description
--- | --- | ---
burn_vector | ksp::math::Vec3 | 
eta | float | 
normal | float | 
prograde | float | 
radial_out | float | 
time | float | 

### Methods

#### remove

```rust
maneuvernode.remove ( ) -> Unit
```



#### set_burn_vector

```rust
maneuvernode.set_burn_vector ( value : ksp::math::Vec3 ) -> Unit
```



#### set_eta

```rust
maneuvernode.set_eta ( value : float ) -> Unit
```



#### set_normal

```rust
maneuvernode.set_normal ( value : float ) -> Unit
```



#### set_prograde

```rust
maneuvernode.set_prograde ( value : float ) -> Unit
```



#### set_radial_out

```rust
maneuvernode.set_radial_out ( value : float ) -> Unit
```



#### set_time

```rust
maneuvernode.set_time ( value : float ) -> Unit
```



## ModuleDeployablePart



### Fields

Name | Type | Description
--- | --- | ---
class_name | string | 
is_moving | bool | 
module_name | string | 
part | ksp::vessel::Part | 
vessel | ksp::vessel::Vessel | 

### Methods

#### extend

```rust
moduledeployablepart.extend ( ) -> Unit
```



#### retract

```rust
moduledeployablepart.retract ( ) -> Unit
```



## ModuleEngines



### Fields

Name | Type | Description
--- | --- | ---
class_name | string | 
has_ignited | bool | 
id | string | 
is_flameout | bool | 
is_shutdown | bool | 
is_staged | bool | 
max_thrust | float | 
min_thrust | float | 
module_name | string | 
name | string | 
part | ksp::vessel::Part | 
type | string | 
vessel | ksp::vessel::Vessel | 

### Methods

#### activate

```rust
moduleengines.activate ( ) -> Unit
```



#### shutdown

```rust
moduleengines.shutdown ( ) -> Unit
```



## Part



### Fields

Name | Type | Description
--- | --- | ---
modules | ksp::vessel::PartModule[] | 
part_name | string | 

## PartModule



### Fields

Name | Type | Description
--- | --- | ---
class_name | string | 
module_name | string | 
part | ksp::vessel::Part | 
vessel | ksp::vessel::Vessel | 

## Stage



### Fields

Name | Type | Description
--- | --- | ---
number | int | 
ready | bool | 

### Methods

#### next

```rust
stage.next ( ) -> bool
```



## Vessel



### Fields

Name | Type | Description
--- | --- | ---
actions | ksp::vessel::ActionGroups | 
air_speed | float | 
altitude | float | 
angular_momentum | ksp::math::Vec3 | 
angular_velocity | ksp::math::Vec3 | 
can_separate | bool | 
co_m | ksp::math::Vec3 | 
engines | ksp::vessel::ModuleEngines[] | 
facing | ksp::math::Direction | 
ground_speed | float | 
heading | float | 
is_active | bool | 
is_commandable | bool | 
is_e_v_a | bool | 
main_body | ksp::orbit::Body | 
maneuver | ksp::vessel::Maneuver | 
mass | float | 
max_thrust | float | 
name | string | 
north_vector | ksp::math::Vec3 | 
orbit | ksp::orbit::Orbit | 
orbital_velocity | ksp::math::Vec3 | 
parts | ksp::vessel::Part[] | 
prograde | ksp::math::Direction | 
retrograde | ksp::math::Direction | 
sample_time | float | 
stage | ksp::vessel::Stage | 
status | string | 
surface_velocity | ksp::math::Vec3 | 
vertical_speed | float | 
vessel_type | string | 
vessel_up | ksp::math::Vec3 | 

### Methods

#### heading_direction

```rust
vessel.heading_direction ( degreesFromNorth : float,
                           pitchAboveHorizon : float,
                           roll : float ) -> ksp::math::Direction
```



#### manage_steering

```rust
vessel.manage_steering ( directionProvider : fn() -> ksp::math::Direction ) -> ksp::control::SteeringManager
```



#### manage_throttle

```rust
vessel.manage_throttle ( throttleProvider : fn() -> float ) -> ksp::control::ThrottleManager
```



#### stage_delta_v

```rust
vessel.stage_delta_v ( stage : int ) -> Option<ksp::vessel::VesselDeltaV>
```



## VesselDeltaV



### Fields

Name | Type | Description
--- | --- | ---
active_engines | ksp::vessel::DeltaVEngineInfo[] | 
burn_time | float | 
delta_v_in_a_s_l | float | 
delta_v_in_vac | float | 
dry_mass | float | 
end_mass | float | 
engines | ksp::vessel::DeltaVEngineInfo[] | 
fuel_mass | float | 
stage | int | 
start_mass | float | 

# Functions


## active_vessel

```rust
pub sync fn active_vessel ( ) -> Result<ksp::vessel::Vessel, string>
```

Try to get the currently active vessel. Will result in an error if there is none.

