// Search.Benchmarks/Program.cs
// Benchmark orchestrator.
//
// Usage:
//   dotnet run -c Release -- --latency     # BenchmarkDotNet single-user matrix (all entity counts)
//   dotnet run -c Release -- --load        # NBomber concurrent-user matrix
//   dotnet run -c Release -- --all         # both (takes many hours)
//   dotnet run -c Release -- --smoke       # quick 10k sanity run: correctness + BDN dry + NBomber 5s
//
// Output: results/results.csv (appended per run; partial runs are not lost)

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Search.Benchmarks;

var mode = args.FirstOrDefault() ?? "--load";
var smoke = mode == "--smoke";
if (smoke) mode = "--all";

// Anchor results path to the exe directory so it always lands in
// Search.Benchmarks/results/ regardless of the working directory.
var resultsPath = Path.Combine(
    AppContext.BaseDirectory, "results", "results.csv");
var writer = new ResultWriter(resultsPath);

// ── Correctness validation ─────────────────────────────────────────────────
// Always runs before any benchmarking when load/all modes are active,
// so a broken variant never pollutes the results CSV.
if (mode is "--load" or "--all")
{
    Console.WriteLine("=== Correctness validation (10k index) ===");
    foreach (var adapter in BenchmarkMatrix.BuildVariants(10_000, writer))
    {
        if (adapter == null) continue;
        CorrectnessValidator.Validate(adapter);
    }
    Console.WriteLine("All variants passed correctness checks. Starting benchmarks.\n");
}

// ── BenchmarkDotNet latency benchmarks ─────────────────────────────────────
if (mode is "--latency" or "--all")
{
    IConfig bdnConfig = smoke
        // Dry job, only EntityCount=10_000 — gives a quick cold-start number.
        // Note: [Params] values are stored as int in attributes, not long.
        ? ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Dry.WithId("Smoke"))
            .AddFilter(new SimpleFilter(bc =>
                bc.Parameters.Items.Any(p =>
                    p.Name == "EntityCount" && Convert.ToInt64(p.Value) == 10_000L)))
        // Full job — all [Params] combinations, .NET 8.0 runtime.
        : ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80).WithId(".NET 8.0"));

    BenchmarkRunner.Run<LatencyBenchmarks>(bdnConfig);
    if (mode is "--latency") return;
}

// ── Correctness validation ────────────────────────────────────────────────
// Build a small 10k index per variant and assert correctness before the
// expensive matrix. A CorrectnessException here exits the process immediately.

// ── NBomber load-test matrix ─────────────────────────────────────────────
var entityCounts  = smoke ? new long[] { 10_000 }  : BenchmarkMatrix.EntityCounts;
var concurrencies = smoke ? new int[]  { 100 }      : BenchmarkMatrix.Concurrencies;
var duration      = smoke ? TimeSpan.FromSeconds(5) : BenchmarkMatrix.TestDuration;

foreach (var entityCount in entityCounts)
{
    Console.WriteLine($"\n=== Entity count: {entityCount:N0} ===");

    // Build each variant's index fresh for this entity count.
    var variants = BenchmarkMatrix.BuildVariants(entityCount, writer);

    foreach (var adapter in variants)
    {
        if (adapter == null) continue;   // was OOM during Build()

        // Warm up JIT before measuring
        for (int w = 0; w < 200; w++) adapter.ScoreOnly(QueryPool.GetRandom());

        foreach (var users in concurrencies)
        {
            Console.WriteLine($"  {adapter.VariantName} | {users:N0} users");
            var stats = LoadTestRunner.Run(adapter, users, duration);
            writer.Append(entityCount, stats);
        }
    }

    BenchmarkMatrix.ForceGc();
}

Console.WriteLine($"\nDone. Results written to {resultsPath}");
