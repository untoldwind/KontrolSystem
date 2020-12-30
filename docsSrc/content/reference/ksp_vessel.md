---
title: "ksp::vessel"
---

Collection of types and functions to get information and control in-game vessels.


# Types


## ActionGroups



### Fields

Name | Type | Description
--- | --- | ---
abort | bool | 
antennas | bool | 
bays | bool | 
brakes | bool | 
chutes | bool | 
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
intakes | bool | 
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



## CrewMember



### Fields

Name | Type | Description
--- | --- | ---
experience | int | 
gender | string | 
name | string | 
part | ksp::vessel::Part | 
trait | string | 

## Defaults



### Fields

Name | Type | Description
--- | --- | ---
steering_pitch_ts | float | 
steering_roll_ts | float | 
steering_yaw_ts | float | 

## DeltaVEngineInfo



### Fields

Name | Type | Description
--- | --- | ---
engine | ksp::vessel::Engine | 
start_burn_stage | int | Number of the stage when engine is supposed to start 

### Methods

#### get_ISP

```rust
deltavengineinfo.get_ISP ( situation : string ) -> float
```

Estimated ISP of the engine in a given `situation`


#### get_thrust

```rust
deltavengineinfo.get_thrust ( situation : string ) -> float
```

Estimated thrust of the engine in a given `situation`


#### get_thrust_vector

```rust
deltavengineinfo.get_thrust_vector ( situation : string ) -> ksp::math::Vec3
```

Estimated thrust vector of the engine in a given `situation`


## DeployablePart



### Fields

Name | Type | Description
--- | --- | ---
as_deployable | Option<ksp::vessel::DeployablePart> | 
as_engine | Option<ksp::vessel::Engine> | 
as_experiment | Option<ksp::vessel::Experiment> | 
class_name | string | 
is_moving | bool | 
module_name | string | 
part | ksp::vessel::Part | 
position | ksp::math::Vec3 | 
tag | string | 
vessel | ksp::vessel::Vessel | 

### Methods

#### do_action

```rust
deployablepart.do_action ( actionName : string,
                           activate : bool ) -> Result<Unit, string>
```



#### do_event

```rust
deployablepart.do_event ( eventName : string ) -> Result<Unit, string>
```



#### extend

```rust
deployablepart.extend ( ) -> Unit
```



#### has_action

```rust
deployablepart.has_action ( actionName : string ) -> bool
```



#### has_event

```rust
deployablepart.has_event ( eventName : string ) -> bool
```



#### has_field

```rust
deployablepart.has_field ( fieldName : string ) -> bool
```



#### retract

```rust
deployablepart.retract ( ) -> Unit
```



## DockingNode



### Fields

Name | Type | Description
--- | --- | ---
as_deployable | Option<ksp::vessel::DeployablePart> | 
as_engine | Option<ksp::vessel::Engine> | 
as_experiment | Option<ksp::vessel::Experiment> | 
class_name | string | 
module_name | string | 
node_position | ksp::math::Vec3 | 
node_type | string | 
part | ksp::vessel::Part | 
port_facing | ksp::math::Direction | 
position | ksp::math::Vec3 | 
state | string | 
tag | string | 
vessel | ksp::vessel::Vessel | 

### Methods

#### control_from

```rust
dockingnode.control_from ( ) -> Unit
```



#### do_action

```rust
dockingnode.do_action ( actionName : string,
                        activate : bool ) -> Result<Unit, string>
```



#### do_event

```rust
dockingnode.do_event ( eventName : string ) -> Result<Unit, string>
```



#### has_action

```rust
dockingnode.has_action ( actionName : string ) -> bool
```



#### has_event

```rust
dockingnode.has_event ( eventName : string ) -> bool
```



#### has_field

```rust
dockingnode.has_field ( fieldName : string ) -> bool
```



#### undock

```rust
dockingnode.undock ( ) -> Unit
```



## Engine



### Fields

Name | Type | Description
--- | --- | ---
as_deployable | Option<ksp::vessel::DeployablePart> | 
as_engine | Option<ksp::vessel::Engine> | 
as_experiment | Option<ksp::vessel::Experiment> | 
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
position | ksp::math::Vec3 | 
tag | string | 
type | string | 
vessel | ksp::vessel::Vessel | 

### Methods

#### activate

```rust
engine.activate ( ) -> Unit
```



#### do_action

```rust
engine.do_action ( actionName : string,
                   activate : bool ) -> Result<Unit, string>
```



#### do_event

```rust
engine.do_event ( eventName : string ) -> Result<Unit, string>
```



#### has_action

```rust
engine.has_action ( actionName : string ) -> bool
```



#### has_event

```rust
engine.has_event ( eventName : string ) -> bool
```



#### has_field

```rust
engine.has_field ( fieldName : string ) -> bool
```



#### shutdown

```rust
engine.shutdown ( ) -> Unit
```



## Experiment



### Fields

Name | Type | Description
--- | --- | ---
as_deployable | Option<ksp::vessel::DeployablePart> | 
as_engine | Option<ksp::vessel::Engine> | 
as_experiment | Option<ksp::vessel::Experiment> | 
class_name | string | 
deployed | bool | 
has_data | bool | 
inoperable | bool | 
module_name | string | 
part | ksp::vessel::Part | 
position | ksp::math::Vec3 | 
rerunnable | bool | 
tag | string | 
vessel | ksp::vessel::Vessel | 

### Methods

#### deploy

```rust
experiment.deploy ( ) -> Unit
```



#### deploy_experiment

```rust
experiment.deploy_experiment ( ) -> Result<Unit, string>
```



#### do_action

```rust
experiment.do_action ( actionName : string,
                       activate : bool ) -> Result<Unit, string>
```



#### do_event

```rust
experiment.do_event ( eventName : string ) -> Result<Unit, string>
```



#### dump_data

```rust
experiment.dump_data ( ) -> Unit
```



#### has_action

```rust
experiment.has_action ( actionName : string ) -> bool
```



#### has_event

```rust
experiment.has_event ( eventName : string ) -> bool
```



#### has_field

```rust
experiment.has_field ( fieldName : string ) -> bool
```



#### transmit_data

```rust
experiment.transmit_data ( ) -> Result<Unit, string>
```



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
maneuver.add ( ut : float,
               radialOut : float,
               normal : float,
               prograde : float ) -> Result<ksp::vessel::ManeuverNode, string>
```



#### add_burn_vector

```rust
maneuver.add_burn_vector ( ut : float,
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
ETA | float | 
normal | float | 
prograde | float | 
radial_out | float | 
time | float | 

### Methods

#### remove

```rust
maneuvernode.remove ( ) -> Unit
```



## Part



### Fields

Name | Type | Description
--- | --- | ---
dry_mass | float | 
has_physics | bool | 
mass | float | 
modules | ksp::vessel::PartModule[] | 
part_id | string | 
part_name | string | 
tag | string | 
vessel | ksp::vessel::Vessel | 

### Methods

#### get_module

```rust
part.get_module ( moduleName : string ) -> Option<ksp::vessel::PartModule>
```



#### has_module

```rust
part.has_module ( moduleName : string ) -> bool
```



## PartModule



### Fields

Name | Type | Description
--- | --- | ---
as_deployable | Option<ksp::vessel::DeployablePart> | 
as_engine | Option<ksp::vessel::Engine> | 
as_experiment | Option<ksp::vessel::Experiment> | 
class_name | string | 
module_name | string | 
part | ksp::vessel::Part | 
position | ksp::math::Vec3 | 
tag | string | 
vessel | ksp::vessel::Vessel | 

### Methods

#### do_action

```rust
partmodule.do_action ( actionName : string,
                       activate : bool ) -> Result<Unit, string>
```



#### do_event

```rust
partmodule.do_event ( eventName : string ) -> Result<Unit, string>
```



#### has_action

```rust
partmodule.has_action ( actionName : string ) -> bool
```



#### has_event

```rust
partmodule.has_event ( eventName : string ) -> bool
```



#### has_field

```rust
partmodule.has_field ( fieldName : string ) -> bool
```



## SimulatedVessel



### Methods

#### drag

```rust
simulatedvessel.drag ( localVelocity : ksp::math::Vec3,
                       dynamicPressurekPa : float,
                       mach : float ) -> ksp::math::Vec3
```



#### lift

```rust
simulatedvessel.lift ( localVelocity : ksp::math::Vec3,
                       dynamicPressurekPa : float,
                       mach : float ) -> ksp::math::Vec3
```



#### simulate

```rust
simulatedvessel.simulate ( altATGL : float,
                           altASL : float,
                           endASL : float,
                           pressure : float,
                           shockTemp : float,
                           time : float,
                           semiDeployMultiplier : float ) -> bool
```



#### will_chutes_deploy

```rust
simulatedvessel.will_chutes_deploy ( altAGL : float,
                                     altASL : float,
                                     probableLandingSiteASL : float,
                                     pressure : float,
                                     shockTemp : float,
                                     t : float,
                                     parachuteSemiDeployMultiplier : float ) -> bool
```



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



## Targetable



### Fields

Name | Type | Description
--- | --- | ---
body | Option<ksp::orbit::Body> | 
docking_port | Option<ksp::vessel::DockingNode> | 
name | string | 
orbit | ksp::orbit::Orbit | 
vessel | Option<ksp::vessel::Vessel> | 

## Vessel

Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.


### Fields

Name | Type | Description
--- | --- | ---
actions | ksp::vessel::ActionGroups | 
air_speed | float | 
altitude | float | 
angular_momentum | ksp::math::Vec3 | 
angular_velocity | ksp::math::Vec3 | 
available_thrust | float | 
can_separate | bool | 
CoM | ksp::math::Vec3 | 
control_level | string | 
crew | ksp::vessel::CrewMember[] | 
crew_capacity | int | 
defaults | ksp::vessel::Defaults | 
docking_ports | ksp::vessel::DockingNode[] | 
east | ksp::math::Vec3 | 
engines | ksp::vessel::Engine[] | 
facing | ksp::math::Direction | 
geo_coordinates | ksp::orbit::GeoCoordinates | 
ground_speed | float | 
heading | float | 
horizontal_surface_speed | float | 
is_active | bool | 
is_commandable | bool | 
is_eva | bool | 
main_body | ksp::orbit::Body | 
maneuver | ksp::vessel::Maneuver | 
mass | float | 
name | string | The name of the vessel. 
north | ksp::math::Vec3 | 
orbit | ksp::orbit::Orbit | 
orbital_velocity | ksp::math::Vec3 | 
parts | ksp::vessel::Part[] | 
position | ksp::math::Vec3 | 
prograde | ksp::math::Direction | 
retrograde | ksp::math::Direction | 
sample_time | float | 
stage | ksp::vessel::Stage | 
status | string | 
surface_velocity | ksp::math::Vec3 | 
target | Option<ksp::vessel::Targetable> | 
up | ksp::math::Vec3 | 
velocity_heading | float | 
vertical_speed | float | 
vessel_type | string | 
volumes | ksp::vessel::Volume[] | 

### Methods

#### heading_direction

```rust
vessel.heading_direction ( degreesFromNorth : float,
                           pitchAboveHorizon : float,
                           roll : float ) -> ksp::math::Direction
```



#### heading_to

```rust
vessel.heading_to ( geoCoordinates : ksp::orbit::GeoCoordinates ) -> float
```



#### manage_rcs_translate

```rust
vessel.manage_rcs_translate ( translateProvider : fn() -> ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



#### manage_steering

```rust
vessel.manage_steering ( directionProvider : fn() -> ksp::math::Direction ) -> ksp::control::SteeringManager
```



#### manage_throttle

```rust
vessel.manage_throttle ( throttleProvider : fn() -> float ) -> ksp::control::ThrottleManager
```



#### manage_wheel_steering

```rust
vessel.manage_wheel_steering ( bearingProvider : fn() -> float ) -> ksp::control::WheelSteeringManager
```



#### manage_wheel_throttle

```rust
vessel.manage_wheel_throttle ( throttleProvider : fn() -> float ) -> ksp::control::WheelThrottleManager
```



#### release_control

```rust
vessel.release_control ( ) -> Unit
```



#### set_rcs_translate

```rust
vessel.set_rcs_translate ( translate : ksp::math::Vec3 ) -> ksp::control::RCSTranslateManager
```



#### set_steering

```rust
vessel.set_steering ( direction : ksp::math::Direction ) -> ksp::control::SteeringManager
```



#### set_throttle

```rust
vessel.set_throttle ( throttle : float ) -> ksp::control::ThrottleManager
```



#### set_wheel_steering

```rust
vessel.set_wheel_steering ( bearing : float ) -> ksp::control::WheelSteeringManager
```



#### set_wheel_throttle

```rust
vessel.set_wheel_throttle ( throttle : float ) -> ksp::control::WheelThrottleManager
```



#### simulated

```rust
vessel.simulated ( limitStage : int ) -> ksp::vessel::SimulatedVessel
```



#### stage_deltav

```rust
vessel.stage_deltav ( stage : int ) -> Option<ksp::vessel::VesselDeltaV>
```

Get delta-v information for a specific `stage` of the vessel, if existent.


## VesselDeltaV



### Fields

Name | Type | Description
--- | --- | ---
active_engines | ksp::vessel::DeltaVEngineInfo[] | 
burn_time | float | Estimated burn time of the stage. 
dry_mass | float | Dry mass of the stage. 
end_mass | float | End mass of the stage. 
engines | ksp::vessel::DeltaVEngineInfo[] | 
fuel_mass | float | Mass of the fuel in the stage. 
stage | int | The stage number. 
start_mass | float | Start mass of the stage. 

### Methods

#### get_deltav

```rust
vesseldeltav.get_deltav ( situation : string ) -> float
```

Estimated delta-v of the stage in a given `situation`


#### get_ISP

```rust
vesseldeltav.get_ISP ( situation : string ) -> float
```

Estimated ISP of the stage in a given `situation`


#### get_thrust

```rust
vesseldeltav.get_thrust ( situation : string ) -> float
```

Estimated thrust of the stage in a given `situation`


#### get_TWR

```rust
vesseldeltav.get_TWR ( situation : string ) -> float
```

Estimated TWR of the stage in a given `situation`


## Volume



### Methods

#### get_bool

```rust
volume.get_bool ( key : string,
                  defaultValue : bool ) -> bool
```



#### get_float

```rust
volume.get_float ( key : string,
                   defaultValue : float ) -> float
```



#### get_int

```rust
volume.get_int ( key : string,
                 defaultValue : int ) -> int
```



#### get_string

```rust
volume.get_string ( key : string,
                    defaultValue : string ) -> string
```



#### has_bool

```rust
volume.has_bool ( key : string ) -> bool
```



#### has_float

```rust
volume.has_float ( key : string ) -> bool
```



#### has_int

```rust
volume.has_int ( key : string ) -> bool
```



#### has_string

```rust
volume.has_string ( key : string ) -> bool
```



#### set_bool

```rust
volume.set_bool ( key : string,
                  value : bool ) -> Unit
```



#### set_float

```rust
volume.set_float ( key : string,
                   value : float ) -> Unit
```



#### set_int

```rust
volume.set_int ( key : string,
                 value : int ) -> Unit
```



#### set_string

```rust
volume.set_string ( key : string,
                    value : string ) -> Unit
```



# Constants

Name | Type | Description
--- | --- | ---
SITUATION_ALTITUDE | string | Used for delta-v calculation at the current altitude. 
SITUATION_SEALEVEL | string | Used for delta-v calculation at sea level of the current body. 
SITUATION_VACUUM | string | Used for delta-v calculation in vacuum. 
TYPE_BASE | string | Value of `vessel.type` if vessel is a planetary base. 
TYPE_DEBIRS | string | Value of `vessel.type` if vessel is some space debris. 
TYPE_EVA | string | Value of `vessel.type` if vessel is a Kerbal in EVA. 
TYPE_FLAG | string | Value of `vessel.type` if vessel is a flag. 
TYPE_LANDER | string | Value of `vessel.type` if vessel is a lander. 
TYPE_PLANE | string | Value of `vessel.type` if vessel is a plane. 
TYPE_PROBE | string | Value of `vessel.type` if vessel is a space probe. 
TYPE_RELAY | string | Value of `vessel.type` if vessel is a communication relay satelite. 
TYPE_SCIENCE_CONTROLLER | string | Value of `vessel.type` if vessel is a deployed science controller. 
TYPE_SCIENCE_PART | string | Value of `vessel.type` if vessel is a deployed science part. 
TYPE_SHIP | string | Value of `vessel.type` if vessel is a space ship. 
TYPE_SPACEOBJECT | string | Value of `vessel.type` if vessel is some unspecified space object. 
TYPE_UNKOWN | string | Value of `vessel.type` if the type of the vessel is unknown/undefined. 


# Functions


## active_vessel

```rust
pub sync fn active_vessel ( ) -> Result<ksp::vessel::Vessel, string>
```

Try to get the currently active vessel. Will result in an error if there is none.


## find_vessel

```rust
pub sync fn find_vessel ( vesselName : string ) -> Result<ksp::vessel::Vessel, string>
```


