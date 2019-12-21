---
title: "ksp::control"
---



# Types


## MovingAverage



### Fields

Name | Type | Description
--- | --- | ---
mean | float | 
sample_limit | int | 
value_count | int | 

### Methods

#### reset

```rust
movingaverage.reset ( ) -> Unit
```



#### update

```rust
movingaverage.update ( value : float ) -> float
```



## PIDLoop



### Fields

Name | Type | Description
--- | --- | ---
change_rate | float | 
d_term | float | 
error | float | 
error_sum | float | 
extra_unwind | bool | 
i_term | float | 
input | float | 
kd | float | 
ki | float | 
kp | float | 
last_sample_time | float | 
max_output | float | 
min_output | float | 
output | float | 
p_term | float | 
setpoint | float | 

### Methods

#### update

```rust
pidloop.update ( sampleTime : float,
                 input : float ) -> float
```



## SteeringManager



### Fields

Name | Type | Description
--- | --- | ---
current_direction | ksp::math::Direction | 
max_stopping_time | float | 
pitch_torque_adjust | float | 
pitch_torque_factor | float | 
roll_control_angle_range | float | 
roll_torque_adjust | float | 
roll_torque_factor | float | 
show_angular_vectors | bool | 
show_facing_vectors | bool | 
show_steering_stats | bool | 
yaw_torque_adjust | float | 
yaw_torque_factor | float | 

### Methods

#### release

```rust
steeringmanager.release ( ) -> Unit
```



#### reset_to_default

```rust
steeringmanager.reset_to_default ( ) -> Unit
```



#### set_direction

```rust
steeringmanager.set_direction ( direction : ksp::math::Direction ) -> Unit
```



#### set_direction_provider

```rust
steeringmanager.set_direction_provider ( _directionProvider : fn() -> ksp::math::Direction ) -> Unit
```



#### set_max_stopping_time

```rust
steeringmanager.set_max_stopping_time ( value : float ) -> Unit
```



#### set_pitch_torque_adjust

```rust
steeringmanager.set_pitch_torque_adjust ( value : float ) -> Unit
```



#### set_pitch_torque_factor

```rust
steeringmanager.set_pitch_torque_factor ( value : float ) -> Unit
```



#### set_roll_control_angle_range

```rust
steeringmanager.set_roll_control_angle_range ( value : float ) -> Unit
```



#### set_roll_torque_adjust

```rust
steeringmanager.set_roll_torque_adjust ( value : float ) -> Unit
```



#### set_roll_torque_factor

```rust
steeringmanager.set_roll_torque_factor ( value : float ) -> Unit
```



#### set_show_angular_vectors

```rust
steeringmanager.set_show_angular_vectors ( value : bool ) -> Unit
```



#### set_show_facing_vectors

```rust
steeringmanager.set_show_facing_vectors ( value : bool ) -> Unit
```



#### set_show_steering_stats

```rust
steeringmanager.set_show_steering_stats ( value : bool ) -> Unit
```



#### set_yaw_torque_adjust

```rust
steeringmanager.set_yaw_torque_adjust ( value : float ) -> Unit
```



#### set_yaw_torque_factor

```rust
steeringmanager.set_yaw_torque_factor ( value : float ) -> Unit
```



## ThrottleManager



### Fields

Name | Type | Description
--- | --- | ---
current_throttle | float | 

### Methods

#### release

```rust
throttlemanager.release ( ) -> Unit
```



#### set_throttle

```rust
throttlemanager.set_throttle ( throttle : float ) -> Unit
```



#### set_throttle_provider

```rust
throttlemanager.set_throttle_provider ( _throttleProvider : fn() -> float ) -> Unit
```



## TorquePI



### Fields

Name | Type | Description
--- | --- | ---
i | float | 
loop | ksp::control::PIDLoop | 
torque_adjust | ksp::control::MovingAverage | 
tr | float | 
ts | float | 

### Methods

#### reset_i

```rust
torquepi.reset_i ( ) -> Unit
```



#### set_torque_adjust

```rust
torquepi.set_torque_adjust ( value : ksp::control::MovingAverage ) -> Unit
```



#### set_ts

```rust
torquepi.set_ts ( value : float ) -> Unit
```



#### update

```rust
torquepi.update ( sampleTime : float,
                  input : float,
                  setpoint : float,
                  momentOfInertia : float,
                  maxOutput : float ) -> float
```


