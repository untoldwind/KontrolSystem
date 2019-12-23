---
title: "Benchmark results"
date: 2019-11-16T21:33:45+01:00
draft: true
---

Comparison of Lambert-Solver c# vs. to2

|       Method |             testSet |     Mean |   Error |  StdDev |
|------------- |-------------------- |---------:|--------:|--------:|
| LamberCSharp | new (...)) }, [351] | 414.2 ns | 0.11 ns | 0.10 ns |
|    LamberTO2 | new (...)) }, [351] | 411.4 ns | 0.13 ns | 0.13 ns |
| LamberCSharp | new (...)) }, [347] | 413.8 ns | 0.14 ns | 0.13 ns |
|    LamberTO2 | new (...)) }, [347] | 410.7 ns | 0.09 ns | 0.08 ns |
| LamberCSharp | new (...)) }, [351] | 420.5 ns | 5.53 ns | 4.90 ns |
|    LamberTO2 | new (...)) }, [351] | 415.7 ns | 2.77 ns | 2.31 ns |

(on i7-7700K using mono 6.4.0)