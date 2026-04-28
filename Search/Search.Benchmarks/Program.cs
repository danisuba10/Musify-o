// Search.Benchmarks/Program.cs
// Benchmark orchestrator.
//
// Usage:
//   dotnet run -c Release -- --latency     # BenchmarkDotNet single-user matrix
//   dotnet run -c Release -- --load        # NBomber concurrent-user matrix
//   dotnet run -c Release -- --all         # both (takes many hours)
//
// Output: results/results.csv (appended per run; partial runs are not lost)

using BenchmarkDotNet.Running;
using Search.Benchmarks;

var mode = args.FirstOrDefault() ?? "--load";

if (mode == "--latency" || mode == "--all")
{
    // BenchmarkDotNet handles the [Params] matrix automatically.
    // Must be compiled in Release mode — will refuse to run in Debug.
    BenchmarkRunner.Run<LatencyBenchmarks>();
    if (mode == "--latency") return;
}

// ── Correctness validation ────────────────────────────────────────────────
// Build a small 10k index per variant and assert correctness before the
// expensive matrix. A CorrectnessException here exits the process immediately.

var writer = new ResultWriter("results/results.csv");

Console.WriteLine("=== Correctness validation (10k index) ===");
foreach (var adapter in BenchmarkMatrix.BuildVariants(10_000, writer))
{
    if (adapter == null) continue;
    CorrectnessValidator.Validate(adapter);
}
Console.WriteLine("All variants passed correctness checks. Starting benchmark matrix.\n");

// ── Load-test matrix ──────────────────────────────────────────────────────
foreach (var entityCount in BenchmarkMatrix.EntityCounts)
{
    Console.WriteLine($"\n=== Entity count: {entityCount:N0} ===");

    // Build each variant's index fresh for this entity count.
    var variants = BenchmarkMatrix.BuildVariants(entityCount, writer);

    foreach (var adapter in variants)
    {
        if (adapter == null) continue;   // was OOM during Build()

        // Warm up JIT before measuring
        for (int w = 0; w < 200; w++) adapter.ScoreOnly(QueryPool.GetRandom());

        foreach (var users in BenchmarkMatrix.Concurrencies)
        {
            Console.WriteLine($"  {adapter.VariantName} | {users:N0} users");
            var stats = LoadTestRunner.Run(adapter, users, BenchmarkMatrix.TestDuration);
            writer.Append(entityCount, stats);
        }
    }

    BenchmarkMatrix.ForceGc();
}

Console.WriteLine("\nDone. Results written to results/results.csv");
