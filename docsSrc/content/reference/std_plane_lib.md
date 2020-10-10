---
title: "std::plane::lib"
---



# Functions


## ksc_runway

```rust
pub sync fn ksc_runway ( ) -> Result<(ksp::orbit::GeoCoordinates, ksp::orbit::GeoCoordinates), string>
```



## plane_has_multi_mode_engine

```rust
pub sync fn plane_has_multi_mode_engine ( vessel : ksp::vessel::Vessel ) -> bool
```



## plane_heading_of

```rust
pub sync fn plane_heading_of ( vessel : ksp::vessel::Vessel,
                               face : ksp::math::Vec3 ) -> float
```



## plane_radar_altimeter

```rust
pub sync fn plane_radar_altimeter ( vessel : ksp::vessel::Vessel ) -> float
```



## plane_rel_vec

```rust
pub sync fn plane_rel_vec ( vessel : ksp::vessel::Vessel,
                            face : ksp::math::Vec3 ) -> ksp::math::Vec3
```



## plane_switch_atmo

```rust
pub sync fn plane_switch_atmo ( vessel : ksp::vessel::Vessel ) -> Unit
```


