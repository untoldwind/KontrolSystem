---
title: "Build on Linux"
date: 2019-11-16T21:33:45+01:00
draft: true
---

# Requirements

* `mono` development environment
* `msbuild` (if not already shiped with mono)
* `nuget` (if not already shiped with mono)

Note: The `dotnet` SDK most like will not work since Unity is using .NET-Framework 4.6.2 which is not an available target for linux. The next best thing would be .NET-Core 2.0, which may or may not be compatible.
(Welcome to the wonderful world of .NET version chaos ... if you figure out how to untangle this mess give me a hint.)

# Setup KSP path

Building requires an install (base) game. More precisely there is a dependency to the DLL files the game ships with. For the build scripts to work one has to set the `KSP_BASE_DIR` environment variable accordingly.

```
export KSP_BASE_DIR=<gamedir>/KSP_linux/KSP_Data
```

