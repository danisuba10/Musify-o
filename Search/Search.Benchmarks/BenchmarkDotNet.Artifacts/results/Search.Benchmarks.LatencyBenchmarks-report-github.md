```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2)
Intel Core Ultra 9 285H 2.90GHz, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.103
  [Host] : .NET 8.0.26 (8.0.26, 8.0.2626.16921), X64 RyuJIT x86-64-v3
  Smoke  : .NET 8.0.26 (8.0.26, 8.0.2626.16921), X64 RyuJIT x86-64-v3

Job=Smoke  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method | EntityCount | Variant    | Mean     | Error | Allocated |
|------- |------------ |----------- |---------:|------:|----------:|
| **Search** | **10000**       | **Variant2_1** | **332.4 μs** |    **NA** |   **64.8 KB** |
| **Search** | **10000**       | **Variant2_2** | **300.4 μs** |    **NA** |   **53.1 KB** |
| **Search** | **10000**       | **Variant3_1** | **341.4 μs** |    **NA** |  **70.66 KB** |
| **Search** | **10000**       | **Variant3_2** | **343.8 μs** |    **NA** |   **53.1 KB** |
| **Search** | **10000**       | **Variant3_3** | **302.5 μs** |    **NA** |  **47.25 KB** |
