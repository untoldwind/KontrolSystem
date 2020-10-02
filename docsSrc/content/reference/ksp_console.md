---
title: "ksp::console"
---

Provides functions to interact with the in-game KontrolSystem Console. As of now the console is output- and monochrome-only, this might change in the future.

Additionally there is support for displaying popup messages on the HUD.



# Types


## Console

Representation of a console


### Fields

Name | Type | Description
--- | --- | ---
cursor_col | int | 
cursor_row | int | 

### Methods

#### clear

```rust
console.clear ( ) -> Unit
```

Clear the console of all its content and move cursor to (0, 0).


#### clear_line

```rust
console.clear_line ( row : int ) -> Unit
```

Clear a line


#### move_cursor

```rust
console.move_cursor ( row : int,
                      column : int ) -> Unit
```

Move the cursor to a give `row` and `column`.


#### print

```rust
console.print ( message : string ) -> Unit
```

Print a message at the current cursor position (and move cursor forward)


#### print_at

```rust
console.print_at ( row : int,
                   column : int,
                   message : string ) -> Unit
```

Moves the cursor to the specified position, prints the message and restores the previous cursor position


#### print_line

```rust
console.print_line ( message : string ) -> Unit
```

Print a message at the current cursor position and move cursor to the beginning of the next line.


## RgbaColor

Interface color with alpha channel.


### Fields

Name | Type | Description
--- | --- | ---
alpha | float | 
blue | float | 
green | float | 
red | float | 

# Constants

Name | Type | Description
--- | --- | ---
BLUE | ksp::console::RgbaColor | Color blue 
CONSOLE | ksp::console::Console | Main console 
CYAN | ksp::console::RgbaColor | Color cyan 
GREEN | ksp::console::RgbaColor | Color green 
RED | ksp::console::RgbaColor | Color red 
YELLOW | ksp::console::RgbaColor | Color yellow 


# Functions


## color

```rust
pub sync fn color ( red : float,
                    green : float,
                    blue : float,
                    alpha : float ) -> ksp::console::RgbaColor
```

Create a new color from `red`, `green`, `blue` and `alpha` (0.0 - 1.0).


## hud_text

```rust
pub sync fn hud_text ( message : string,
                       seconds : int,
                       size : int,
                       styleSelect : int,
                       color : ksp::console::RgbaColor ) -> Unit
```

Show a message on the HUD to inform the player that something extremely cool (or extremely uncool) has happed.

