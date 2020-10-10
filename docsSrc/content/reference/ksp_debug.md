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

## GroundMarker

Represents a ground marker on a given celestial body.


### Fields

Name | Type | Description
--- | --- | ---
color | ksp::console::RgbaColor | The color of the debugging vector 
geo_coordinates | ksp::orbit::GeoCoordinates | 
rotation | float | 
visible | bool | Controls if the ground marker is currently visible (initially `true`) 

# Constants

Name | Type | Description
--- | --- | ---
DEBUG | ksp::debug::Debug | Collection of debug helper 

