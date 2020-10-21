---
title: "ksp::resource"
---

Provides functions to handle resources (like fuel, ore, etc)


# Types


## ResourceDefinition



### Fields

Name | Type | Description
--- | --- | ---
density | float | 
display_name | string | 
name | string | 
transfer_mode | string | 
unit_cost | float | 
volume | float | 

### Methods

#### start_resource_transfer

```rust
resourcedefinition.start_resource_transfer ( transferFrom : ksp::vessel::Part[],
                                             transferTo : ksp::vessel::Part[],
                                             amount : float ) -> ksp::resource::ResourceTransfer
```



## ResourceTransfer



### Fields

Name | Type | Description
--- | --- | ---
goal | float | 
resource | ksp::resource::ResourceDefinition | 
status | string | 
status_message | string | 
transferred | float | 

### Methods

#### abort

```rust
resourcetransfer.abort ( ) -> Unit
```



# Functions


## find_resource

```rust
pub sync fn find_resource ( resourceName : string ) -> Result<ksp::resource::ResourceDefinition, string>
```


