---
title: "std::numerics::brent_optimize"
---



# Functions


## brent_optimize

```rust
pub sync fn brent_optimize ( func : fn(float) -> float,
                             a : float,
                             b : float,
                             tolerance : float,
                             max_iterations : int ) -> Result<(fx : float, x : float), string>
```


