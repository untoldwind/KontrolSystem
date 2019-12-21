---
title: "Benchmark results"
date: 2019-11-16T21:33:45+01:00
draft: true
---

Comparison of Lambert-Solver c# vs. to2

|       Method |             testSet |     Mean |   Error |  StdDev |
|------------- |-------------------- |---------:|--------:|--------:|
| LamberCSharp | new (...)) }, [351] | 412.6 ns | 0.81 ns | 0.76 ns |
|    LamberTO2 | new (...)) }, [351] | 415.4 ns | 0.16 ns | 0.15 ns |
| LamberCSharp | new (...)) }, [347] | 412.8 ns | 0.93 ns | 0.87 ns |
|    LamberTO2 | new (...)) }, [347] | 415.7 ns | 0.23 ns | 0.21 ns |
| LamberCSharp | new (...)) }, [351] | 412.6 ns | 0.45 ns | 0.42 ns |
|    LamberTO2 | new (...)) }, [351] | 415.9 ns | 0.57 ns | 0.53 ns |
