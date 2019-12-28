---
title: "ksp::orbit"
---



# Types


## Body

Represents an in-game celestrial body.


### Fields

Name | Type | Description
--- | --- | ---
atmosphere_depth | float | Depth/height of the atmosphere if present. 
grav_parameter | float | Standard gravitation parameter of the body. 
has_atmosphere | bool | `true` if the celestrial body has an atmosphere to deal with. 
name | string | Name of the celestrial body. 
orbit | ksp::orbit::Orbit | The orbit of the celestrial body itself (around the parent body) 
position | ksp::math::Vec3 | 
radius | float | Radius of the body at sea level 
SOI_radius | float | Radius of the sphere of influence of the body 
up | ksp::math::Vec3 | 

### Methods

#### create_orbit

```rust
body.create_orbit ( position : ksp::math::Vec3,
                    velocity : ksp::math::Vec3,
                    UT : float ) -> ksp::orbit::Orbit
```

Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `UT`


#### get_surface_height

```rust
body.get_surface_height ( lat : float,
                          lon : float ) -> float
```



#### get_surface_normal

```rust
body.get_surface_normal ( lat : float,
                          lon : float ) -> ksp::math::Vec3
```



## GeoCoordinates



### Fields

Name | Type | Description
--- | --- | ---
body | ksp::orbit::Body | 
latitude | float | 
longitude | float | 
surface_height | float | 
surface_normal | ksp::math::Vec3 | 

### Methods

#### set_latitude

```rust
geocoordinates.set_latitude ( value : float ) -> Unit
```



#### set_longitude

```rust
geocoordinates.set_longitude ( value : float ) -> Unit
```



## NodeParameters



## Orbit

Represents an in-game orbit.


### Fields

Name | Type | Description
--- | --- | ---
apoapsis | float | Apoapsis of the orbit above sealevel of the `reference_body`. 
apoapsis_r | float | Radius of apoapsis of the orbit (i.e. from the center of the `reference_body') 
argument_of_periapsis | float | Argument of periapsis of the orbit. 
eccentricity | float | Eccentricity of the orbit. 
epoch | float | Orbit epoch. 
inclination | float | Inclination of the orbit in degree. 
LAN | float | Longitude of ascending node of the orbit in degree 
mean_anomaly_at_epoch | float | Mean anomaly of the orbit at `epoch` 
mean_motion | float | Mean motion of the orbit. 
orbit_normal | ksp::math::Vec3 | Normal vector perpendicular to orbital plane. 
patch_end_time | float | Universal time of the end of this orbital patch (if there a planed maneuvering nodes 
periapsis | float | Periapsis of the orbit above sealevel of the `reference_body` 
periapsis_r | float | Radius of periapsis of the orbit (i.e. from the center of the `reference_body') 
period | float | Orbital period. 
reference_body | ksp::orbit::Body | The celestrical body the orbit is referenced on. 
semi_major_axis | float | Semi major axis of the orbit. 

### Methods

#### absolute_position

```rust
orbit.absolute_position ( UT : float ) -> ksp::math::Vec3
```

Get the absolute position at a given univerals time `UT`


#### ascending_node_true_anomaly

```rust
orbit.ascending_node_true_anomaly ( b : ksp::orbit::Orbit ) -> float
```



#### descending_node_true_anomaly

```rust
orbit.descending_node_true_anomaly ( b : ksp::orbit::Orbit ) -> float
```



#### get_eccentric_anomaly_at_true_anomaly

```rust
orbit.get_eccentric_anomaly_at_true_anomaly ( trueAnomaly : float ) -> float
```



#### get_mean_anomaly_at_eccentric_anomaly

```rust
orbit.get_mean_anomaly_at_eccentric_anomaly ( E : float ) -> float
```



#### horizontal

```rust
orbit.horizontal ( UT : float ) -> ksp::math::Vec3
```



#### mean_anomaly_at_u_t

```rust
orbit.mean_anomaly_at_u_t ( UT : float ) -> float
```



#### next_apoapsis_time

```rust
orbit.next_apoapsis_time ( UT : float ) -> float
```



#### next_periapsis_time

```rust
orbit.next_periapsis_time ( UT : float ) -> float
```



#### next_time_of_radius

```rust
orbit.next_time_of_radius ( UT : float,
                            radius : float ) -> float
```



#### normal_plus

```rust
orbit.normal_plus ( UT : float ) -> ksp::math::Vec3
```



#### orbital_velocity

```rust
orbit.orbital_velocity ( UT : float ) -> ksp::math::Vec3
```



#### perturbed_orbit

```rust
orbit.perturbed_orbit ( UT : float,
                        dV : ksp::math::Vec3 ) -> ksp::orbit::Orbit
```



#### prograde

```rust
orbit.prograde ( UT : float ) -> ksp::math::Vec3
```



#### radial_plus

```rust
orbit.radial_plus ( UT : float ) -> ksp::math::Vec3
```



#### radius

```rust
orbit.radius ( UT : float ) -> float
```



#### relative_position

```rust
orbit.relative_position ( UT : float ) -> ksp::math::Vec3
```



#### synodic_period

```rust
orbit.synodic_period ( other : ksp::orbit::Orbit ) -> float
```



#### time_of_ascending_node

```rust
orbit.time_of_ascending_node ( b : ksp::orbit::Orbit,
                               UT : float ) -> float
```



#### time_of_descending_node

```rust
orbit.time_of_descending_node ( b : ksp::orbit::Orbit,
                                UT : float ) -> float
```



#### time_of_true_anomaly

```rust
orbit.time_of_true_anomaly ( trueAnomaly : float,
                             UT : float ) -> float
```



#### true_anomaly_at_radius

```rust
orbit.true_anomaly_at_radius ( radius : float ) -> float
```



#### true_anomaly_from_vector

```rust
orbit.true_anomaly_from_vector ( vec : ksp::math::Vec3 ) -> float
```



#### u_t_at_mean_anomaly

```rust
orbit.u_t_at_mean_anomaly ( meanAnomaly : float,
                            UT : float ) -> float
```



#### up

```rust
orbit.up ( UT : float ) -> ksp::math::Vec3
```



# Functions


## find_body

```rust
pub sync fn find_body ( name : string ) -> Result<ksp::orbit::Body, string>
```


