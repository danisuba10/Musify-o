// Search.Benchmarks/Program.cs
// Benchmark orchestrator.
//
// Usage:
//   dotnet run -c Release -- --latency     # BenchmarkDotNet single-user matrix (all entity counts)
//   dotnet run -c Release -- --load        # NBomber concurrent-user matrix
//   dotnet run -c Release -- --all         # both (takes many hours)
//   dotnet run -c Release -- --smoke       # quick 10k sanity run: correctness + BDN dry + NBomber 5s
//
//   --variant=<id>   Limit to a single variant. id ∈ { 2_1, 2_2, 3_1, 3_2, 3_3, 4_1, 4_2, all }.
//                    Aliases: 2.1/v2_1/V21 etc. Default: all.
//                    Example: dotnet run -c Release --no-build -- --load --variant=2_1
//
// Output: results/results.csv (appended per run; partial runs are not lost)

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Search.Benchmarks;

// ── Argument parsing ───────────────────────────────────────────────────────
// First non-flag-prefixed positional is the mode; --variant=<id> is optional.
string mode = "--load";
string variantFilter = "all";
foreach (var raw in args)
{
    if (raw.StartsWith("--variant=", StringComparison.OrdinalIgnoreCase))
        variantFilter = raw.Substring("--variant=".Length);
    else if (raw.StartsWith("--variant:", StringComparison.OrdinalIgnoreCase))
        variantFilter = raw.Substring("--variant:".Length);
    else
        mode = raw;
}
var smoke = mode == "--smoke";
if (smoke) mode = "--all";

// Normalise variant filter to canonical form ("2_1", "2_2", "3_1", "3_2", "3_3", "4_1", "4_2", or "all").
static string NormaliseVariant(string raw)
{
    var s = raw.Trim().ToLowerInvariant().TrimStart('v').Replace(".", "_");
    if (s is "all" or "") return "all";
    // Accept "21" → "2_1"
    if (s.Length == 2 && char.IsDigit(s[0]) && char.IsDigit(s[1]))
        s = $"{s[0]}_{s[1]}";
    if (s is "2_1" or "2_2" or "3_1" or "3_2" or "3_3" or "4_1" or "4_2") return s;
    throw new ArgumentException(
        $"Unknown --variant '{raw}'. Expected one of: 2_1, 2_2, 3_1, 3_2, 3_3, 4_1, 4_2, all.");
}
variantFilter = NormaliseVariant(variantFilter);
bool VariantSelected(string variantName) =>
    variantFilter == "all" ||
    variantName.StartsWith($"Variant{variantFilter}", StringComparison.OrdinalIgnoreCase);

if (variantFilter != "all")
    Console.WriteLine($"[filter] Limiting run to Variant{variantFilter}.\n");

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

// Anchor results to the project source directory (Search.Benchmarks/results/)
// rather than the bin output, so results survive a `dotnet clean` and are
// easy to find without navigating into bin/Release/net8.0/.
// AppContext.BaseDirectory = bin/{Config}/net8.0/ → go up 3 levels.
var projectDir  = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
var resultsPath = Path.Combine(projectDir, "results", "results.csv");
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
        if (!VariantSelected(adapter.VariantName)) continue;
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

    if (variantFilter != "all")
    {
        // BenchmarkDotNet method names follow the SearchAsync_VariantX_Y pattern.
        var needle = $"Variant{variantFilter}";
        bdnConfig = bdnConfig.AddFilter(new SimpleFilter(bc =>
            bc.Descriptor.WorkloadMethod.Name.Contains(needle, StringComparison.OrdinalIgnoreCase)));
    }

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

// Resume support: skip (variant, entityCount, concurrentUsers) combos that
// already have a row in the CSV so an interrupted run can be continued.
var completedKeys = writer.LoadCompletedKeys();
bool AlreadyDone(string variantName, long ec, int users) =>
    completedKeys.Contains($"{variantName}|{ec}|{users}");

foreach (var entityCount in entityCounts)
{
    Console.WriteLine($"\n=== Entity count: {entityCount:N0} ===");

    // Build each variant one at a time so only one index lives in RAM per run.
    var factories = BenchmarkMatrix.GetVariantFactories(entityCount, writer);

    foreach (var factory in factories)
    {
        // Cheap pre-check: each factory's name is encoded in its OomGuard label,
        // but the adapter exposes VariantName only after build. To avoid building
        // an index we will discard, peek at the factory index position.
        int factoryIndex = factories.IndexOf(factory);
        string variantTag = factoryIndex switch
        {
            0 => "2_1", 1 => "2_2", 2 => "3_1", 3 => "3_2", 4 => "3_3",
            5 => "4_1", 6 => "4_2",
            _ => ""
        };
        if (variantFilter != "all" && variantTag != variantFilter)
            continue;

        var adapter = factory();
        if (adapter == null) continue;   // was OOM during Build()

        // Warm up JIT before measuring
        for (int w = 0; w < 200; w++) adapter.ScoreOnly(QueryPool.GetRandom());

        foreach (var users in concurrencies)
        {
            if (AlreadyDone(adapter.VariantName, entityCount, users))
            {
                Console.WriteLine($"  [SKIP] {adapter.VariantName} | {entityCount:N0} entities | {users:N0} users — already in CSV");
                continue;
            }
            Console.WriteLine($"  {adapter.VariantName} | {entityCount:N0} entities | {users:N0} users");
            var stats = LoadTestRunner.Run(adapter, entityCount, users, duration);
            writer.Append(entityCount, stats);
        }

        // Release this variant's index before building the next one.
        BenchmarkMatrix.ForceGc();
    }
}

Console.WriteLine($"\nDone. Results written to {resultsPath}");
