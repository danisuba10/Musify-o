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

// ── Thread-pool warm-up ────────────────────────────────────────────────────
// Default min worker threads = #cores. Under high concurrent NBomber load the
// thread-pool grows new workers at ~1 per ~500 ms, which alone produces
// multi-second tail latencies during the first 30 s of every scenario.
// Pre-allocate enough workers to cover the largest concurrency level we run.
{
    int desiredMin = Math.Max(BenchmarkMatrix.Concurrencies.Max(), 256);
    ThreadPool.GetMinThreads(out var oldWorker, out var oldIo);
    ThreadPool.SetMinThreads(Math.Max(oldWorker, desiredMin),
                             Math.Max(oldIo,     desiredMin));
}

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

    // Build each variant one at a time so only one index lives in RAM per run.
    var factories = BenchmarkMatrix.GetVariantFactories(entityCount, writer);

    foreach (var factory in factories)
    {
        var adapter = factory();
        if (adapter == null) continue;   // was OOM during Build()

        // Warm up JIT before measuring
        for (int w = 0; w < 200; w++) adapter.ScoreOnly(QueryPool.GetRandom());

        foreach (var users in concurrencies)
        {
            Console.WriteLine($"  {adapter.VariantName} | {entityCount:N0} entities | {users:N0} users");
            var stats = LoadTestRunner.Run(adapter, entityCount, users, duration);
            writer.Append(entityCount, stats);
        }

        // Release this variant's index before building the next one.
        BenchmarkMatrix.ForceGc();
    }
}

Console.WriteLine($"\nDone. Results written to {resultsPath}");
