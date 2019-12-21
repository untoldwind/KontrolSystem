---
title: "ksp::debug"
---

Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire.


# Types


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


# Functions


## add_line

```rust
pub sync fn add_line ( start : ksp::math::Vec3,
                       end : ksp::math::Vec3,
                       color : ksp::console::RgbaColor,
                       label : string,
                       width : float ) -> Option<ksp::debug::DebugVector>
```

Draws a line from `start` to `end` with a specified `color` and `width` in the current game scene.
The line may have a `label` at its mid-point.

The result of the function is a `DebugVector` that can be modified or `None` if the current game scene does not support debugging vectors.



## add_vector

```rust
pub sync fn add_vector ( start : ksp::math::Vec3,
                         vector : ksp::math::Vec3,
                         color : ksp::console::RgbaColor,
                         label : string,
                         width : float ) -> Option<ksp::debug::DebugVector>
```

Draws a `vector` positioned at `start` with a specified `color` and `width` in the current game scene.
The vector may have a `label` at its mid-point.

The result of the function is a `DebugVector` that can be modified or `None` if the current game scene does not support debugging vectors.



## clear_markers

```rust
pub sync fn clear_markers ( ) -> Unit
```

Remove all markers from the game-scene.

