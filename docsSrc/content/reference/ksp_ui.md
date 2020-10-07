---
title: "ksp::ui"
---

Provides functions to create base UI windows and dialogs.


# Types


## Button



## Container



### Methods

#### button

```rust
container.button ( label : string,
                   onClick : fn(T) -> T ) -> ksp::ui::Button
```



#### horizontal_layout

```rust
container.horizontal_layout ( ) -> ksp::ui::Container<T>
```



#### input

```rust
container.input ( value : string,
                  onUpdate : fn(T, string) -> T ) -> ksp::ui::Input
```



#### label

```rust
container.label ( label : string ) -> ksp::ui::Label
```



#### vertical_layout

```rust
container.vertical_layout ( ) -> ksp::ui::Container<T>
```



## Input



## Label



## Window



### Fields

Name | Type | Description
--- | --- | ---
closed | bool | 
state | T | 

### Methods

#### close

```rust
window.close ( ) -> Unit
```



#### set_state

```rust
window.set_state ( value : T ) -> Unit
```



# Functions


## show_window

```rust
pub sync fn show_window ( initialState : T,
                          isEndState : fn(T) -> bool,
                          render : fn(ksp::ui::Container<T>, T) -> Unit ) -> ksp::ui::Window<T>
```


