---
title: "ksp::ui"
---

Provides functions to create base UI windows and dialogs.


# Types


## Button



### Methods

#### expand_height

```rust
button.expand_height ( ) -> ksp::ui::Button
```



#### expand_width

```rust
button.expand_width ( ) -> ksp::ui::Button
```



#### height

```rust
button.height ( height : float ) -> ksp::ui::Button
```



#### max_height

```rust
button.max_height ( height : float ) -> ksp::ui::Button
```



#### max_width

```rust
button.max_width ( width : float ) -> ksp::ui::Button
```



#### min_height

```rust
button.min_height ( height : float ) -> ksp::ui::Button
```



#### min_width

```rust
button.min_width ( width : float ) -> ksp::ui::Button
```



#### width

```rust
button.width ( width : float ) -> ksp::ui::Button
```



## Container



### Methods

#### button

```rust
container.button ( label : string,
                   onClick : fn(T) -> T ) -> ksp::ui::Button
```



#### expand_height

```rust
container.expand_height ( ) -> ksp::ui::Container<T>
```



#### expand_width

```rust
container.expand_width ( ) -> ksp::ui::Container<T>
```



#### height

```rust
container.height ( height : float ) -> ksp::ui::Container<T>
```



#### horizontal_layout

```rust
container.horizontal_layout ( ) -> ksp::ui::Container<T>
```



#### label

```rust
container.label ( label : string ) -> ksp::ui::Label
```



#### max_height

```rust
container.max_height ( height : float ) -> ksp::ui::Container<T>
```



#### max_width

```rust
container.max_width ( width : float ) -> ksp::ui::Container<T>
```



#### min_height

```rust
container.min_height ( height : float ) -> ksp::ui::Container<T>
```



#### min_width

```rust
container.min_width ( width : float ) -> ksp::ui::Container<T>
```



#### text_field

```rust
container.text_field ( value : string,
                       onUpdate : fn(T, string) -> T ) -> ksp::ui::TextField
```



#### vertical_layout

```rust
container.vertical_layout ( ) -> ksp::ui::Container<T>
```



#### width

```rust
container.width ( width : float ) -> ksp::ui::Container<T>
```



## Label



### Methods

#### expand_height

```rust
label.expand_height ( ) -> ksp::ui::Label
```



#### expand_width

```rust
label.expand_width ( ) -> ksp::ui::Label
```



#### height

```rust
label.height ( height : float ) -> ksp::ui::Label
```



#### max_height

```rust
label.max_height ( height : float ) -> ksp::ui::Label
```



#### max_width

```rust
label.max_width ( width : float ) -> ksp::ui::Label
```



#### min_height

```rust
label.min_height ( height : float ) -> ksp::ui::Label
```



#### min_width

```rust
label.min_width ( width : float ) -> ksp::ui::Label
```



#### width

```rust
label.width ( width : float ) -> ksp::ui::Label
```



## TextField



### Methods

#### expand_height

```rust
textfield.expand_height ( ) -> ksp::ui::TextField
```



#### expand_width

```rust
textfield.expand_width ( ) -> ksp::ui::TextField
```



#### height

```rust
textfield.height ( height : float ) -> ksp::ui::TextField
```



#### max_height

```rust
textfield.max_height ( height : float ) -> ksp::ui::TextField
```



#### max_length

```rust
textfield.max_length ( maxLength : int ) -> ksp::ui::TextField
```



#### max_width

```rust
textfield.max_width ( width : float ) -> ksp::ui::TextField
```



#### min_height

```rust
textfield.min_height ( height : float ) -> ksp::ui::TextField
```



#### min_width

```rust
textfield.min_width ( width : float ) -> ksp::ui::TextField
```



#### width

```rust
textfield.width ( width : float ) -> ksp::ui::TextField
```



## Window



### Fields

Name | Type | Description
--- | --- | ---
title | string | 

### Methods

#### button

```rust
window.button ( label : string,
                onClick : fn(T) -> T ) -> ksp::ui::Button
```



#### horizontal_layout

```rust
window.horizontal_layout ( ) -> ksp::ui::Container<T>
```



#### label

```rust
window.label ( label : string ) -> ksp::ui::Label
```



#### text_field

```rust
window.text_field ( value : string,
                    onUpdate : fn(T, string) -> T ) -> ksp::ui::TextField
```



#### vertical_layout

```rust
window.vertical_layout ( ) -> ksp::ui::Container<T>
```



## WindowHandle



### Fields

Name | Type | Description
--- | --- | ---
closed | bool | 
state | T | 

### Methods

#### close

```rust
windowhandle.close ( ) -> Unit
```



# Functions


## show_window

```rust
pub sync fn show_window ( initialState : T,
                          isEndState : fn(T) -> bool,
                          render : fn(ksp::ui::Window<T>, T) -> Unit ) -> ksp::ui::WindowHandle<T>
```


