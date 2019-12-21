---
title: "std::warp"
---

Collection of helper functions to control time warp.

# Functions


## phys_warp

```rust
pub fn phys_warp ( warp : int ) -> Unit
```

Set physics warp.

## rails_warp

```rust
pub fn rails_warp ( warp : int ) -> Unit
```

Set rails warp.

## reset_warp

```rust
pub fn reset_warp ( ) -> Unit
```

Reset warp to its default state.

## warp_seconds

```rust
pub fn warp_seconds ( seconds : float ) -> Unit
```

Warp a given number of seconds into the future.

Will automatically toggle between rails and pythics warping depending how far the future is.
