* [x] Check declared type for variables
* [x] Check declared return type for functions
* [x] Consider type inflection for local variables
* [x] Consider type inflection for function return (considered, but nope at this point)
* [x] Module level `const` -ants
* [x] Support tuples + deconstructing tuples
* [x] `if` expression (simple case)
* [x] `return` expression
* [x] `loop`/`while` expression with `break`/`continue`
* Fill up core lib
  * [x] `core::math`
  * [x] `core::logging`
* [x] Decide if Vector2, Vector3 should become base types or part of `ksp::math`
* [x] Add a test runner
* [x] Split generator into define types/methods -> emit (so that strick ordering of dependencies is no longer necessary)
* [x] Import/call functions from other modules
* [x] Define common entrypoints for KSP
  * `main_ksc()`
  * `main_flight()`
  * `main_editor()`
  * `main_trackingstation()`
* [x] Migrate mock Orbit to `ksp::testing` + define common interface
* [x] Get rid of IKontrolType in favour of `BoundType implements TO2Type`
  * TO2Type probably need `UnderlyingType(context)` for TypeRef resolving
  * Will enable type aliases as well
* [x] List available modules in Toolbar window (based on gamemode)
* [x] Fix/optimization: Bool operators should skip right operant if possible
* [x] Support `Future<T>`
  * Should be part of a subscription model for game hooks/callbacks
  * [x] `async` functions
  * [x] Refactory `async` should become the default case
* [x] Concept for `ArrayBuilder<T>` (ranges will do that)
* [x] Assign operators `+=`, `-=` etc.
* [x] Read-only variables with `const`
* [x] Support lambdas (interop with c# back and forth)
* [x] User defined structs/records (will be just type aliases)
* [x] Type aliasing in modules
* [x] User defined methods for structs (out of scope for now)
* [x] Consider traits (out of scope for now)
* [x] Add `to_string` converters to int, float, bool with formatting
* [ ] Optimization: Call by ref for value types
* [ ] Optimization: Return by ref for value type (potentially generate to method variants)
* [x] `for ... in` loops
  * [x] Deconstruct variables from loop elements
* [x] Support `Result<T, E>` on language level
  * [x] Support `?` suffix
  * [ ] Basic `match`
  * [ ] Collect stacktrace in error case
  * [x] Auto-Cast to `Ok`
* [x] `Option<T>` as synonym for `Result<T, Unit>`
  * [ ] Support `??` operator
* [x] Simple `if` should give an `option`
* [x] Range expression
  * [ ] ... with `map` to create arrays
  * [x] ... as source for `for .. in`
  * [ ] ... in index should create a slice
* [x] Internal yield in loops
  * [x] in sync function have a time in context to prevent stall (i.e. hard timeout of functions)
  * [x] in async actually yield a "not ready" and wait for next poll
  * .... all of this now handled with common timeout mechanic
* [ ] Have `Channel<T>` to link lambdas
* [ ] `... as ...` cast resulting in an option (`PartModule` casting)
* [ ] User defined `const` -ants (in module)
* [x] bit operations
* [ ] Consider optional parameters with defaults
* [ ] add basic immutable collections (list/array). Potentially support basic generics
* [ ] Tailcall optimization
* [ ] Literal simplify optimization
* [ ] Condition emit optimization (i.e. use `blt`, `bgt` etc)
* [ ] Refactor error collection in submodules
* [x] Record-like struct mapping
* [x] Named function arguments (not really necessary atm, can be simulated with a record parameter)
* [x] Record updating/chaining
* [x] Support multiple operator-emitters bound to same operator (example Quaternion * (Vec or Quaternion))
* Plugin extensions:
  * [x] Show compilation errors in UI
  * [x] .... parsing errors as well
  * [ ] Support source watch
  * [x] Running/In progress marker for async entry points (with abort)
  * [ ] Test runner with UI
  * [x] Run KsModules as Unity coroutines
    * Only async entrypoints are allowed
* Bootstrap `ksp` library
  * [ ] `ksp::orbit` common things to do with orbits
  * [x] `ksp::vessel` get current vessel + basic interactions with its parts/actions
  * [x] `ksp::planets` get information about the solar system(s) (will be part of `ksp::orbit`)
  * [x] `ksp::console` text-console
  * [ ] `ksp::ui` ...
* Autopiloting
  * [ ] `OnPreAutopilot` callback on vessel
    * Exception should lead to an automatic disengage
  * [x] `Wait until` construct
  * [x] `Sleep` construct
  * [x] `SteeringManager` (at best implement in to2 itself)
* Add helpful struff
  * [x] PIDLoop
  * [x] Lambert-Solver
  * [ ] Root-Solver
  * [ ] ODE-Solver
* ... multi-threading and the great beyond

* BUGS/Quriks:
* [x] `result_tests::test_unwrap_unit_result` fails on windows
* [x] `lambert_tests::test_testsets` has illegal IL on windows (deconstruct with placeholder does not work)
* [x] Support for async methods missing
* [x] Hidden vars of `for ... in` not stored in async state
