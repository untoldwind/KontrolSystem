---
title: "Benchmark results"
date: 2019-11-16T21:33:45+01:00
draft: true
---

Since TO2 is supposed to run as a Unity-Coroutine it has a built-in time-out check to prevent endless loops which would block the main Unity thread and essentially crash the game.

In most cases the impact on numerical performance should be almost unnoticeable.

Comparison of Lambert-Solver C# vs. to2 with timeout checking

|       Method |             testSet |     Mean |   Error |  StdDev |
|------------- |-------------------- |---------:|--------:|--------:|
| LamberCSharp | new (...)) }, [351] | 472.6 ns | 0.62 ns | 0.58 ns |
|    LamberTO2 | new (...)) }, [351] | 492.8 ns | 0.17 ns | 0.15 ns |
| LamberCSharp | new (...)) }, [347] | 473.9 ns | 0.70 ns | 0.66 ns |
|    LamberTO2 | new (...)) }, [347] | 486.7 ns | 0.62 ns | 0.58 ns |
| LamberCSharp | new (...)) }, [351] | 484.3 ns | 0.67 ns | 0.63 ns |
|    LamberTO2 | new (...)) }, [351] | 473.7 ns | 1.16 ns | 0.91 ns |

(on AMD Ryzen 9 3900X using mono 6.10.0)