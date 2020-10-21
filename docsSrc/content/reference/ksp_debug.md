---
title: "ksp::debug"
---

Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire.


# Types


## Debug

Collection of debug helper


### Methods

#### add_ground_marker

```rust
debug.add_ground_marker ( geoCoordinates : ksp::orbit::GeoCoordinates,
                          color : ksp::console::RgbaColor,
                          rotation : float ) -> ksp::debug::GroundMarker
```



#### add_line

```rust
debug.add_line ( startProvider : fn() -> ksp::math::Vec3,
                 endProvider : fn() -> ksp::math::Vec3,
                 color : ksp::console::RgbaColor,
                 label : string,
                 width : float ) -> ksp::debug::DebugVector
```

Draws a line from `start` to `end` with a specified `color` and `width` in the current game scene.
The line may have a `label` at its mid-point.



#### add_pixel_path

```rust
debug.add_pixel_path ( path : ksp::math::Vec3[],
                       color : ksp::console::RgbaColor,
                       dashed : bool ) -> ksp::debug::PixelPath
```



#### add_vector

```rust
debug.add_vector ( startProvider : fn() -> ksp::math::Vec3,
                   endProvider : fn() -> ksp::math::Vec3,
                   color : ksp::console::RgbaColor,
                   label : string,
                   width : float ) -> ksp::debug::DebugVector
```

Draws a `vector` positioned at `start` with a specified `color` and `width` in the current game scene.
The vector may have a `label` at its mid-point.



#### clear_markers

```rust
debug.clear_markers ( ) -> Unit
```

Remove all markers from the game-scene.


## DebugVector

Represents a debugging vector in the current scene.


### Fields

Name | Type | Description
--- | --- | ---
color | ksp::console::RgbaColor | The color of the debugging vector 
end | ksp::math::Vec3 | The current end position of the debugging vector. 
pointy | bool | Controls if an arrow should be drawn at the end. 
scale | float | 
start | ksp::math::Vec3 | The current starting position of the debugging vector. 
visible | bool | Controls if the debug-vector is currently visible (initially `true`) 
width | float | The width of the debugging vector 

## GroundMarker

Represents a ground marker on a given celestial body.


### Fields

Name | Type | Description
--- | --- | ---
color | ksp::console::RgbaColor | The color of the ground marker vector 
geo_coordinates | ksp::orbit::GeoCoordinates | 
rotation | float | 
visible | bool | Controls if the ground marker is currently visible (initially `true`) 

## PixelPath

Represents a pixel path.


### Fields

Name | Type | Description
--- | --- | ---
color | ksp::console::RgbaColor | The color of the ground marker vector 
dashed | bool | 
path | ksp::math::Vec3[] | 
visible | bool | Controls if the ground marker is currently visible (initially `true`) 

# Constants

Name | Type | Description
--- | --- | ---
DEBUG | ksp::debug::Debug | Collection of debug helper 

