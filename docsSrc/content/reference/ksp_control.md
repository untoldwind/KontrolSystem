---
title: "ksp::control"
---



# Types


## MovingAverage



### Fields

Name | Type | Description
--- | --- | ---
mean | float | 
mean_diff | float | 
sample_limit | int | 
value_count | int | 

### Methods

#### reset

```rust
movingaverage.reset ( ) -> Unit
```



#### update

```rust
movingaverage.update ( sampleTime : float,
                       value : float ) -> float
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

#### reset_i

```rust
pidloop.reset_i ( ) -> Unit
```



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
steeringmanager.set_direction_provider ( newDirectionProvider : fn() -> ksp::math::Direction ) -> Unit
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
throttlemanager.set_throttle_provider ( newThrottleProvider : fn() -> float ) -> Unit
```



## TorquePI



### Fields

Name | Type | Description
--- | --- | ---
i | float | 
loop | ksp::control::PIDLoop | 
tr | float | 
ts | float | 

### Methods

#### reset_i

```rust
torquepi.reset_i ( ) -> Unit
```



#### update

```rust
torquepi.update ( sampleTime : float,
                  input : float,
                  setpoint : float,
                  momentOfInertia : float,
                  maxOutput : float ) -> float
```



## WheelSteeringManager



### Methods

#### release

```rust
wheelsteeringmanager.release ( ) -> Unit
```



#### set_bearing

```rust
wheelsteeringmanager.set_bearing ( bearing : float ) -> Unit
```



#### set_bearing_provider

```rust
wheelsteeringmanager.set_bearing_provider ( newBearingProvider : fn() -> float ) -> Unit
```



## WheelThrottleManager



### Fields

Name | Type | Description
--- | --- | ---
current_throttle | float | 

### Methods

#### release

```rust
wheelthrottlemanager.release ( ) -> Unit
```



#### set_throttle

```rust
wheelthrottlemanager.set_throttle ( throttle : float ) -> Unit
```



#### set_throttle_provider

```rust
wheelthrottlemanager.set_throttle_provider ( newThrottleProvider : fn() -> float ) -> Unit
```



# Functions


## moving_average

```rust
pub sync fn moving_average ( sampleLimit : int ) -> ksp::control::MovingAverage
```

Create a new MovingAverage with given sample limit.


## pid_loop

```rust
pub sync fn pid_loop ( kp : float,
                       ki : float,
                       kd : float,
                       minOutput : float,
                       maxOutput : float ) -> ksp::control::PIDLoop
```

Create a new PIDLoop with given parameters.

