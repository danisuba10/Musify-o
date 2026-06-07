// Search.Benchmarks/LatencyBenchmarks.cs
using BenchmarkDotNet.Attributes;

using V21Index = Search.Variant2_1.LevenshteinBasic.TrigramIndex;
using V22Index = Search.Variant2_2.LevenshteinStack.TrigramIndex;
using V31Index = Search.Variant3_1.DamerauBasic.TrigramIndex;
using V32Index = Search.Variant3_2.DamerauStack.TrigramIndex;
using V33Index = Search.Variant3_3.DamerauBitmap.RoaringBitmapTrigramIndex;
using V41Index = Search.Variant4_1.FstAutomaton.FstTermIndex;
using V42Index = Search.Variant4_2.FstAutomaton.FstTermIndex;

namespace Search.Benchmarks;

/// <summary>
/// Single-user micro-benchmarks: P50/P99 latency, GC allocations, allocated bytes.
/// Run with: dotnet run -c Release -- --latency
/// BenchmarkDotNet requires Release build (Optimize=true in csproj).
/// </summary>
[MemoryDiagnoser]
[HtmlExporter, CsvExporter]
public class LatencyBenchmarks
{
    /// <summary>
    /// Set by Program.cs before BenchmarkRunner.Run so OOM build failures
    /// are written to the shared results CSV as SKIPPED_OOM sentinel rows.
    /// </summary>
    internal static ResultWriter? OomWriter { get; set; }

    [Params(10_000, 100_000, 1_000_000, 10_000_000)]
    public long EntityCount { get; set; }

    // One param per variant — BenchmarkDotNet runs all (EntityCount × Variant) combinations.
    [Params("Variant2_1", "Variant2_2", "Variant3_1", "Variant3_2", "Variant3_3", "Variant4_1", "Variant4_2")]
    public string Variant { get; set; } = null!;

    private SearchOnlyAdapter _adapter = null!;
    private string _query = null!;
    private bool _oomSkipped;

    [GlobalSetup]
    public void Setup()
    {
        try
        {
            _adapter = BuildAdapter(Variant, EntityCount);
        }
        catch (OutOfMemoryException)
        {
            _oomSkipped = true;
            var label = $"{Variant}_LatencyBuildOOM_{EntityCount}";
            Console.WriteLine($"[OOM-SKIPPED] {Variant} @ {EntityCount:N0} — index build ran out of memory, benchmark results will be marked N/A.");
            OomWriter?.AppendOom(label);
            ForceGc();
            return;
        }

        // Warm up the JIT — run 100 queries before measuring
        for (int i = 0; i < 100; i++)
            _adapter.ScoreOnly(QueryPool.GetRandom());
        _query = QueryPool.GetRandom();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _oomSkipped = false;
        ForceGc();
    }

    [Benchmark]
    public System.Collections.Generic.IReadOnlyList<Guid> Search()
    {
        if (_oomSkipped) return System.Array.Empty<Guid>(); // N/A — index build OOM'd
        return _adapter.ScoreOnly(_query);
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static SearchOnlyAdapter BuildAdapter(string variant, long count)
    {
        var data = DataGenerator.Generate(count);

        if (variant == "Variant2_1")
        {
            var idx = new V21Index(); idx.Build(data);
            return SearchOnlyAdapter.ForVariant2_1(idx);
        }
        if (variant == "Variant2_2")
        {
            var idx = new V22Index(); idx.Build(data);
            return SearchOnlyAdapter.ForVariant2_2(idx);
        }
        if (variant == "Variant3_1")
        {
            var idx = new V31Index(); idx.Build(data);
            return SearchOnlyAdapter.ForVariant3_1(idx);
        }
        if (variant == "Variant3_2")
        {
            var idx = new V32Index(); idx.Build(data);
            return SearchOnlyAdapter.ForVariant3_2(idx);
        }
        if (variant == "Variant3_3")
        {
            var idx = new V33Index(); idx.Build(data);
            return SearchOnlyAdapter.ForVariant3_3(idx);
        }
        if (variant == "Variant4_1")
        {
            var idx = new V41Index(); idx.Build(data);
            return SearchOnlyAdapter.ForVariant4_1(idx);
        }
        if (variant == "Variant4_2")
        {
            var idx = new V42Index(); idx.Build(data);
            return SearchOnlyAdapter.ForVariant4_2(idx);
        }
        throw new ArgumentException($"Unknown variant: {variant}");
    }

    private static void ForceGc()
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
    }
}
