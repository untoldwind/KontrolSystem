---
title: "std::rendezvous::dock"
---



# Functions


## choose_docking_ports

```rust
pub sync fn choose_docking_ports ( vessel : ksp::vessel::Vessel,
                                   target : ksp::vessel::Targetable ) -> Result<(target_port : ksp::vessel::DockingNode, vessel_port : ksp::vessel::DockingNode), string>
```



## dock_approach

```rust
pub fn dock_approach ( vessel : ksp::vessel::Vessel,
                       target_port : ksp::vessel::DockingNode ) -> Result<Unit, string>
```



## dock_move_correct_side

```rust
pub fn dock_move_correct_side ( vessel : ksp::vessel::Vessel,
                                target_port : ksp::vessel::DockingNode ) -> Result<Unit, string>
```



## dock_vessel

```rust
pub fn dock_vessel ( vessel : ksp::vessel::Vessel,
                     target : ksp::vessel::Targetable ) -> Result<Unit, string>
```


