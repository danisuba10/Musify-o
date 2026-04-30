// Search.Benchmarks/BenchmarkMatrix.cs
// Matrix configuration and index-build helpers used by Program.cs.
using System.Diagnostics;

using V21Index = Search.Variant2_1.LevenshteinBasic.TrigramIndex;
using V22Index = Search.Variant2_2.LevenshteinStack.TrigramIndex;
using V31Index = Search.Variant3_1.DamerauBasic.TrigramIndex;
using V32Index = Search.Variant3_2.DamerauStack.TrigramIndex;
using V33Index = Search.Variant3_3.DamerauBitmap.RoaringBitmapTrigramIndex;
using V41Index = Search.Variant4_1.FstAutomaton.FstTermIndex;
using V42Index = Search.Variant4_2.FstAutomaton.FstTermIndex;

namespace Search.Benchmarks;

internal static class BenchmarkMatrix
{
    public static readonly long[]     EntityCounts  = { 10_000_000 };
    public static readonly int[]      Concurrencies = { 1_000 };
    public static readonly TimeSpan   TestDuration  = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Builds all five variant indexes for the given entity count.
    /// Uses <see cref="OomGuard"/> to survive expected OOM at high counts for
    /// HashSet-based variants (2.1–3.2). Returns <c>null</c> slots for OOM variants.
    /// </summary>
    public static List<SearchOnlyAdapter?> BuildVariants(long count, ResultWriter writer)
    {
        var adapters = new List<SearchOnlyAdapter?>();

        // Variant 2.1 — LevenshteinBasic (HashSet index, heap allocation)
        adapters.Add(OomGuard.TryRun(() =>
        {
            var idx = new V21Index();
            BuildAndMeasure("Variant2_1", count, idx.Build, DataGenerator.Generate(count),
                out double ramMb, out double buildS);
            var adapter = SearchOnlyAdapter.ForVariant2_1(idx);
            adapter.IndexRamMb = ramMb;
            adapter.BuildTimeS = buildS;
            return adapter;
        }, $"Variant2_1 Build({count:N0})", writer));

        // Variant 2.2 — LevenshteinStack (stackalloc/ArrayPool calculator, same index shape)
        adapters.Add(OomGuard.TryRun(() =>
        {
            var idx = new V22Index();
            BuildAndMeasure("Variant2_2", count, idx.Build, DataGenerator.Generate(count),
                out double ramMb, out double buildS);
            var adapter = SearchOnlyAdapter.ForVariant2_2(idx);
            adapter.IndexRamMb = ramMb;
            adapter.BuildTimeS = buildS;
            return adapter;
        }, $"Variant2_2 Build({count:N0})", writer));

        // Variant 3.1 — DamerauBasic (OSA, heap allocation)
        adapters.Add(OomGuard.TryRun(() =>
        {
            var idx = new V31Index();
            BuildAndMeasure("Variant3_1", count, idx.Build, DataGenerator.Generate(count),
                out double ramMb, out double buildS);
            var adapter = SearchOnlyAdapter.ForVariant3_1(idx);
            adapter.IndexRamMb = ramMb;
            adapter.BuildTimeS = buildS;
            return adapter;
        }, $"Variant3_1 Build({count:N0})", writer));

        // Variant 3.2 — DamerauStack (OSA, stackalloc/ArrayPool)
        adapters.Add(OomGuard.TryRun(() =>
        {
            var idx = new V32Index();
            BuildAndMeasure("Variant3_2", count, idx.Build, DataGenerator.Generate(count),
                out double ramMb, out double buildS);
            var adapter = SearchOnlyAdapter.ForVariant3_2(idx);
            adapter.IndexRamMb = ramMb;
            adapter.BuildTimeS = buildS;
            return adapter;
        }, $"Variant3_2 Build({count:N0})", writer));

        // Variant 3.3 — DamerauBitmap (RoaringBitmap index — expected to survive 100M)
        adapters.Add(OomGuard.TryRun(() =>
        {
            var idx = new V33Index();
            BuildAndMeasure("Variant3_3", count, idx.Build, DataGenerator.Generate(count),
                out double ramMb, out double buildS);
            var adapter = SearchOnlyAdapter.ForVariant3_3(idx);
            adapter.IndexRamMb = ramMb;
            adapter.BuildTimeS = buildS;
            return adapter;
        }, $"Variant3_3 Build({count:N0})", writer));

        // Variant 4.1 — FST + Levenshtein automaton (Schulz–Mihov), Dictionary best-distance map
        adapters.Add(OomGuard.TryRun(() =>
        {
            var idx = new V41Index();
            BuildAndMeasure("Variant4_1", count, idx.Build, DataGenerator.Generate(count),
                out double ramMb, out double buildS);
            var adapter = SearchOnlyAdapter.ForVariant4_1(idx);
            adapter.IndexRamMb = ramMb;
            adapter.BuildTimeS = buildS;
            return adapter;
        }, $"Variant4_1 Build({count:N0})", writer));

        // Variant 4.2 — FST + Levenshtein automaton with lazy heap-push (no per-query Dictionary)
        adapters.Add(OomGuard.TryRun(() =>
        {
            var idx = new V42Index();
            BuildAndMeasure("Variant4_2", count, idx.Build, DataGenerator.Generate(count),
                out double ramMb, out double buildS);
            var adapter = SearchOnlyAdapter.ForVariant4_2(idx);
            adapter.IndexRamMb = ramMb;
            adapter.BuildTimeS = buildS;
            return adapter;
        }, $"Variant4_2 Build({count:N0})", writer));

        return adapters;
    }

    /// <summary>
    /// Returns a list of factory functions, one per variant. Invoke each factory
    /// individually so only one variant's index lives in RAM at a time.
    /// </summary>
    public static List<Func<SearchOnlyAdapter?>> GetVariantFactories(long count, ResultWriter writer)
    {
        return new List<Func<SearchOnlyAdapter?>>
        {
            () => OomGuard.TryRun(() =>
            {
                var idx = new V21Index();
                BuildAndMeasure("Variant2_1", count, idx.Build, DataGenerator.Generate(count),
                    out double ramMb, out double buildS);
                var adapter = SearchOnlyAdapter.ForVariant2_1(idx);
                adapter.IndexRamMb = ramMb;
                adapter.BuildTimeS = buildS;
                return adapter;
            }, $"Variant2_1 Build({count:N0})", writer),

            () => OomGuard.TryRun(() =>
            {
                var idx = new V22Index();
                BuildAndMeasure("Variant2_2", count, idx.Build, DataGenerator.Generate(count),
                    out double ramMb, out double buildS);
                var adapter = SearchOnlyAdapter.ForVariant2_2(idx);
                adapter.IndexRamMb = ramMb;
                adapter.BuildTimeS = buildS;
                return adapter;
            }, $"Variant2_2 Build({count:N0})", writer),

            () => OomGuard.TryRun(() =>
            {
                var idx = new V31Index();
                BuildAndMeasure("Variant3_1", count, idx.Build, DataGenerator.Generate(count),
                    out double ramMb, out double buildS);
                var adapter = SearchOnlyAdapter.ForVariant3_1(idx);
                adapter.IndexRamMb = ramMb;
                adapter.BuildTimeS = buildS;
                return adapter;
            }, $"Variant3_1 Build({count:N0})", writer),

            () => OomGuard.TryRun(() =>
            {
                var idx = new V32Index();
                BuildAndMeasure("Variant3_2", count, idx.Build, DataGenerator.Generate(count),
                    out double ramMb, out double buildS);
                var adapter = SearchOnlyAdapter.ForVariant3_2(idx);
                adapter.IndexRamMb = ramMb;
                adapter.BuildTimeS = buildS;
                return adapter;
            }, $"Variant3_2 Build({count:N0})", writer),

            () => OomGuard.TryRun(() =>
            {
                var idx = new V33Index();
                BuildAndMeasure("Variant3_3", count, idx.Build, DataGenerator.Generate(count),
                    out double ramMb, out double buildS);
                var adapter = SearchOnlyAdapter.ForVariant3_3(idx);
                adapter.IndexRamMb = ramMb;
                adapter.BuildTimeS = buildS;
                return adapter;
            }, $"Variant3_3 Build({count:N0})", writer),

            () => OomGuard.TryRun(() =>
            {
                var idx = new V41Index();
                BuildAndMeasure("Variant4_1", count, idx.Build, DataGenerator.Generate(count),
                    out double ramMb, out double buildS);
                var adapter = SearchOnlyAdapter.ForVariant4_1(idx);
                adapter.IndexRamMb = ramMb;
                adapter.BuildTimeS = buildS;
                return adapter;
            }, $"Variant4_1 Build({count:N0})", writer),

            () => OomGuard.TryRun(() =>
            {
                var idx = new V42Index();
                BuildAndMeasure("Variant4_2", count, idx.Build, DataGenerator.Generate(count),
                    out double ramMb, out double buildS);
                var adapter = SearchOnlyAdapter.ForVariant4_2(idx);
                adapter.IndexRamMb = ramMb;
                adapter.BuildTimeS = buildS;
                return adapter;
            }, $"Variant4_2 Build({count:N0})", writer),
        };
    }

    public static void ForceGc()
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private static void BuildAndMeasure(
        string variantLabel,
        long count,
        Action<System.Collections.Generic.IEnumerable<Search.Abstractions.SearchProjection>> buildFn,
        System.Collections.Generic.IEnumerable<Search.Abstractions.SearchProjection> data,
        out double ramMb,
        out double buildS)
    {
        ForceGc();
        long before = GC.GetTotalMemory(false);
        var watch = Stopwatch.StartNew();
        buildFn(data);
        watch.Stop();
        long after = GC.GetTotalMemory(false);

        ramMb  = (after - before) / 1_048_576.0;
        buildS = watch.Elapsed.TotalSeconds;

        Console.WriteLine(
            $"  [{variantLabel}] Built {count:N0} entities in {buildS:F1}s | Index RAM ≈ {ramMb:F0} MB");
    }
}
