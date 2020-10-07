---
title: "std::staging"
---

Collection of helper functions to control staging of a vessel

# Functions


## has_flameout

```rust
pub sync fn has_flameout ( vessel : ksp::vessel::Vessel ) -> bool
```



## trigger_staging

```rust
pub fn trigger_staging ( vessel : ksp::vessel::Vessel ) -> bool
```

Helper function to automatically trigger staging during a burn.

This function is just checking if one of the ignited engines has has a flameout,
which in most cases means that the current stage has burned out.

Will return `true` if stating has been triggered.
