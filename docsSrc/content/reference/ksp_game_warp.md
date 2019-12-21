---
title: "ksp::game::warp"
---

Collection of functions to control time warp.


# Constants

Name | Type | Description
--- | --- | ---
PHYSICS | string | Value of `current_warp_mode` if in physics warp. 
RAILS | string | Value of `current_warp_mode` if warp is on rails. 


# Functions


## cancel

```rust
pub sync fn cancel ( ) -> Unit
```

Cancel time warp


## current_index

```rust
pub sync fn current_index ( ) -> int
```

Get the current warp index. Actual factor depends on warp mode.


## current_mode

```rust
pub sync fn current_mode ( ) -> string
```

Get the current warp mode (RAILS/PHYSICS).


## current_rate

```rust
pub sync fn current_rate ( ) -> float
```

Get the current warp rate (i.e. actual time multiplier).


## is_settled

```rust
pub sync fn is_settled ( ) -> bool
```

Check if time warp has settled down


## set_index

```rust
pub sync fn set_index ( warpIndex : int ) -> Unit
```

Set warp index. Actual factor depends on warp mode.


## set_mode

```rust
pub sync fn set_mode ( warpMode : string ) -> Unit
```

Set the warp mode (RAILS/PHYSICS).


## warp_to

```rust
pub sync fn warp_to ( UT : float ) -> Unit
```

Warp forward to a specific universal time.

