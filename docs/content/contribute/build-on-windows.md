---
title: "Build on Windows"
date: 2019-11-16T21:33:49+01:00
draft: true
---

# Requirements

There might be other options but the simplest option seems to be to install VCode 2019 (Community Edition is sufficent) with command-line tools enabled.
Check if `msbuild` and `nuget` is avaible in `cmd` or Power-Shell.

# Setup KSP path

Building requires an install (base) game. More precisely there is a dependency to the DLL files the game ships with. For the build scripts to work one has to set the `KSP_BASE_DIR` environment variable accordingly.

```
set KSP_BASE_DIR=C:\<gamedir>\KSP_x64_Data
```

E.g. if you install the version from https://www.kerbalspaceprogram.com this would be

```
set KSP_BASE_DIR=C:\Kerbal Space Program\KSP_x64_Data
```

Alternatively you might want to setup this globally so that everything works in the IDE of your choice as well: https://superuser.com/questions/949560/how-do-i-set-system-environment-variables-in-windows-10
