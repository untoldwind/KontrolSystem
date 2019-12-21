---
title: "ksp::console"
---

Provides functions to interact with the in-game KontrolSystem Console. As of now the console is output- and monochrome-only, this might change in the future.

Additionally there is support for displaying popup messages on the HUD.



# Types


## RgbaColor

Interface color with alpha channel.


# Functions


## clear

```rust
pub sync fn clear ( ) -> Unit
```

Clear the console of all its content and move cursor to (0, 0).


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


## move_cursor

```rust
pub sync fn move_cursor ( row : int,
                          column : int ) -> Unit
```

Move the cursor to a give `row` and `column`.


## print

```rust
pub sync fn print ( message : string ) -> Unit
```

Print a message at the current cursor position (and move cursor forward)


## print_at

```rust
pub sync fn print_at ( row : int,
                       column : int,
                       message : string ) -> Unit
```

Shortcut for `move_cursor(row, col)` followed by `print(message)`


## print_line

```rust
pub sync fn print_line ( message : string ) -> Unit
```

Print a message at the current cursor position and move cursor to the beginning of the next line.

