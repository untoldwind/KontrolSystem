---
title: "ksp::debug"
---

Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire.


# Types


## Debug

Collection of debug helper


## DebugVector

Represents a debugging vector in the current scene.


### Fields

Name | Type | Description
--- | --- | ---
color | ksp::console::RgbaColor | The color of the debugging vector 
pointy | bool | Controls if an arrow should be drawn at the end. 
scale | float | 
start | ksp::math::Vec3 | The current starting position of the debugging vector. 
vector | ksp::math::Vec3 | The direction of the debugging vector. 
visible | bool | Controls if the debug-vector is currently visible (initially `true`) 
width | float | The width of the debugging vector 

### Methods

#### set_color

```rust
debugvector.set_color ( value : ksp::console::RgbaColor ) -> Unit
```

The color of the debugging vector


#### set_pointy

```rust
debugvector.set_pointy ( value : bool ) -> Unit
```

Controls if an arrow should be drawn at the end.


#### set_scale

```rust
debugvector.set_scale ( value : float ) -> Unit
```



#### set_start

```rust
debugvector.set_start ( value : ksp::math::Vec3 ) -> Unit
```

The current starting position of the debugging vector.


#### set_vector

```rust
debugvector.set_vector ( value : ksp::math::Vec3 ) -> Unit
```

The direction of the debugging vector.


#### set_visible

```rust
debugvector.set_visible ( value : bool ) -> Unit
```

Controls if the debug-vector is currently visible (initially `true`)


#### set_width

```rust
debugvector.set_width ( value : float ) -> Unit
```

The width of the debugging vector


## GroundMarker

Represents a ground marker on a given celestial body.


### Fields

Name | Type | Description
--- | --- | ---
color | ksp::console::RgbaColor | The color of the debugging vector 
geo_coordinates | ksp::orbit::GeoCoordinates | 
rotation | float | 
visible | bool | Controls if the ground marker is currently visible (initially `true`) 

### Methods

#### set_color

```rust
groundmarker.set_color ( value : ksp::console::RgbaColor ) -> Unit
```

The color of the debugging vector


#### set_geo_coordinates

```rust
groundmarker.set_geo_coordinates ( value : ksp::orbit::GeoCoordinates ) -> Unit
```



#### set_rotation

```rust
groundmarker.set_rotation ( value : float ) -> Unit
```



#### set_visible

```rust
groundmarker.set_visible ( value : bool ) -> Unit
```

Controls if the ground marker is currently visible (initially `true`)


# Constants

Name | Type | Description
--- | --- | ---
DEBUG | ksp::debug::Debug | Collection of debug helper 

