---
title: "std::land::speed_policy"
---



# Types


## SpeedPolicy



# Functions


## gravity_turn_speed_policy

```rust
pub sync fn gravity_turn_speed_policy ( terrain_radius : float,
                                        g : float,
                                        thrust : float ) -> fn(ksp::math::Vec3, ksp::math::Vec3) -> float
```



## powered_coast_speed_policy

```rust
pub sync fn powered_coast_speed_policy ( terrain_radius : float,
                                         g : float,
                                         thrust : float ) -> fn(ksp::math::Vec3, ksp::math::Vec3) -> float
```



## safe_speed_policy

```rust
pub sync fn safe_speed_policy ( terrain_radius : float,
                                g : float,
                                thrust : float ) -> fn(ksp::math::Vec3, ksp::math::Vec3) -> float
```


