// Search.Benchmarks/SearchOnlyAdapter.cs
// Bypasses Phase 4 (DB fetch) — measures Phases 1–3 only (trigram lookup + scoring).
using Search.Abstractions;

// Type aliases to disambiguate four TrigramIndex classes from different namespaces.
using V21Index    = Search.Variant2_1.LevenshteinBasic.TrigramIndex;
using V22Index    = Search.Variant2_2.LevenshteinStack.TrigramIndex;
using V31Index    = Search.Variant3_1.DamerauBasic.TrigramIndex;
using V32Index    = Search.Variant3_2.DamerauStack.TrigramIndex;
using V33Index    = Search.Variant3_3.DamerauBitmap.RoaringBitmapTrigramIndex;
using V21Calc     = Search.Variant2_1.LevenshteinBasic.LevenshteinCalculator;
using V22Calc     = Search.Variant2_2.LevenshteinStack.LevenshteinStackCalculator;
using V31Calc     = Search.Variant3_1.DamerauBasic.DamerauCalculator;
using V32Calc     = Search.Variant3_2.DamerauStack.DamerauStackCalculator;
using V33Calc     = Search.Variant3_3.DamerauBitmap.DamerauStackCalculator;
using V21Trigrams = Search.Variant2_1.LevenshteinBasic.TrigramIndex;
using V22Trigrams = Search.Variant2_2.LevenshteinStack.TrigramIndex;
using V31Trigrams = Search.Variant3_1.DamerauBasic.TrigramIndex;
using V32Trigrams = Search.Variant3_2.DamerauStack.TrigramIndex;

namespace Search.Benchmarks;

internal sealed class SearchOnlyAdapter
{
    private readonly string _variantName;
    private readonly Func<string, SearchEntityType, IReadOnlyList<Guid>> _scoreOnly;

    // Build metadata — populated by BenchmarkMatrix.BuildVariants after index construction.
    public double IndexRamMb { get; set; }
    public double BuildTimeS { get; set; }

    private SearchOnlyAdapter(string name, Func<string, SearchEntityType, IReadOnlyList<Guid>> fn)
    {
        _variantName = name;
        _scoreOnly   = fn;
    }

    public string VariantName => _variantName;

    /// <summary>
    /// Runs Phases 1–3 only (trigram lookup + edit-distance scoring).
    /// Returns ordered List&lt;Guid&gt; (top 500 by score). No DB fetch.
    /// </summary>
    public IReadOnlyList<Guid> ScoreOnly(string term, SearchEntityType filter = SearchEntityType.All)
        => _scoreOnly(term, filter);

    // ── Factory methods — one per variant ────────────────────────────────

    public static SearchOnlyAdapter ForVariant2_1(V21Index index)
    {
        return new SearchOnlyAdapter("Variant2_1_LevenshteinBasic", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var counts     = BuildCandidateCounts_V21(index, normalised, filter);

            var scored = new List<(Guid Id, double Score)>(counts.Count);
            foreach (var (id, _) in counts)
            {
                if (!index.TryGetEntry(id, out var entry)) continue;
                int dist = V21Calc.Compute(normalised.AsSpan(), entry.Name.ToLowerInvariant().AsSpan());
                if (dist > 5) continue;
                scored.Add((id, 1.0 / (1.0 + dist) + (dist == 0 ? 0.5 : 0)));
            }

            return scored.OrderByDescending(x => x.Score)
                         .Take(500)
                         .Select(x => x.Id)
                         .ToList();
        });
    }

    public static SearchOnlyAdapter ForVariant2_2(V22Index index)
    {
        return new SearchOnlyAdapter("Variant2_2_LevenshteinStack", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var counts     = BuildCandidateCounts_V22(index, normalised, filter);

            var scored = new List<(Guid Id, double Score)>(counts.Count);
            foreach (var (id, _) in counts)
            {
                if (!index.TryGetEntry(id, out var entry)) continue;
                int dist = V22Calc.Compute(normalised.AsSpan(), entry.Name.ToLowerInvariant().AsSpan());
                if (dist > 5) continue;
                scored.Add((id, 1.0 / (1.0 + dist) + (dist == 0 ? 0.5 : 0)));
            }

            return scored.OrderByDescending(x => x.Score)
                         .Take(500)
                         .Select(x => x.Id)
                         .ToList();
        });
    }

    public static SearchOnlyAdapter ForVariant3_1(V31Index index)
    {
        return new SearchOnlyAdapter("Variant3_1_DamerauBasic", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var counts     = BuildCandidateCounts_V31(index, normalised, filter);

            var scored = new List<(Guid Id, double Score)>(counts.Count);
            foreach (var (id, _) in counts)
            {
                if (!index.TryGetEntry(id, out var entry)) continue;
                int dist = V31Calc.Compute(normalised.AsSpan(), entry.Name.ToLowerInvariant().AsSpan());
                if (dist > 5) continue;
                scored.Add((id, 1.0 / (1.0 + dist) + (dist == 0 ? 0.5 : 0)));
            }

            return scored.OrderByDescending(x => x.Score)
                         .Take(500)
                         .Select(x => x.Id)
                         .ToList();
        });
    }

    public static SearchOnlyAdapter ForVariant3_2(V32Index index)
    {
        return new SearchOnlyAdapter("Variant3_2_DamerauStack", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var counts     = BuildCandidateCounts_V32(index, normalised, filter);

            var scored = new List<(Guid Id, double Score)>(counts.Count);
            foreach (var (id, _) in counts)
            {
                if (!index.TryGetEntry(id, out var entry)) continue;
                int dist = V32Calc.Compute(normalised.AsSpan(), entry.Name.ToLowerInvariant().AsSpan());
                if (dist > 5) continue;
                scored.Add((id, 1.0 / (1.0 + dist) + (dist == 0 ? 0.5 : 0)));
            }

            return scored.OrderByDescending(x => x.Score)
                         .Take(500)
                         .Select(x => x.Id)
                         .ToList();
        });
    }

    public static SearchOnlyAdapter ForVariant3_3(V33Index index)
    {
        return new SearchOnlyAdapter("Variant3_3_DamerauBitmap", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var bitmap     = index.GetCandidateBitmap(normalised);

            if (bitmap == null) return Array.Empty<Guid>();

            var scored = new List<(Guid Id, double Score)>(64);
            // RoaringBitmap iterates int (not uint) — see Equativ.RoaringBitmaps API
            foreach (int idx in bitmap)
            {
                if (index.IsTombstoned(idx)) continue;
                var entry = index.GetEntry(idx);
                if (filter != SearchEntityType.All && entry.EntityType != filter) continue;
                int dist = V33Calc.Compute(normalised.AsSpan(), entry.Name.ToLowerInvariant().AsSpan());
                if (dist > 5) continue;
                scored.Add((entry.Id, 1.0 / (1.0 + dist) + (dist == 0 ? 0.5 : 0)));
            }

            return scored.OrderByDescending(x => x.Score)
                         .Take(500)
                         .Select(x => x.Id)
                         .ToList();
        });
    }

    // ── Private helpers — one per HashSet-based TrigramIndex type ─────────
    // These are duplicated per type because there is no common interface for
    // the internal TrigramIndex classes across variant namespaces.

    private static Dictionary<Guid, int> BuildCandidateCounts_V21(
        V21Index index, string normalised, SearchEntityType filter)
    {
        var counts = new Dictionary<Guid, int>();
        foreach (var tg in V21Trigrams.GetTrigrams(normalised).Distinct())
        {
            if (!index.TryGetCandidates(tg, out var ids)) continue;
            foreach (var id in ids)
            {
                if (filter != SearchEntityType.All
                    && index.TryGetEntry(id, out var e)
                    && e.EntityType != filter) continue;
                counts[id] = counts.TryGetValue(id, out var c) ? c + 1 : 1;
            }
        }
        return counts;
    }

    private static Dictionary<Guid, int> BuildCandidateCounts_V22(
        V22Index index, string normalised, SearchEntityType filter)
    {
        var counts = new Dictionary<Guid, int>();
        foreach (var tg in V22Trigrams.GetTrigrams(normalised).Distinct())
        {
            if (!index.TryGetCandidates(tg, out var ids)) continue;
            foreach (var id in ids)
            {
                if (filter != SearchEntityType.All
                    && index.TryGetEntry(id, out var e)
                    && e.EntityType != filter) continue;
                counts[id] = counts.TryGetValue(id, out var c) ? c + 1 : 1;
            }
        }
        return counts;
    }

    private static Dictionary<Guid, int> BuildCandidateCounts_V31(
        V31Index index, string normalised, SearchEntityType filter)
    {
        var counts = new Dictionary<Guid, int>();
        foreach (var tg in V31Trigrams.GetTrigrams(normalised).Distinct())
        {
            if (!index.TryGetCandidates(tg, out var ids)) continue;
            foreach (var id in ids)
            {
                if (filter != SearchEntityType.All
                    && index.TryGetEntry(id, out var e)
                    && e.EntityType != filter) continue;
                counts[id] = counts.TryGetValue(id, out var c) ? c + 1 : 1;
            }
        }
        return counts;
    }

    private static Dictionary<Guid, int> BuildCandidateCounts_V32(
        V32Index index, string normalised, SearchEntityType filter)
    {
        var counts = new Dictionary<Guid, int>();
        foreach (var tg in V32Trigrams.GetTrigrams(normalised).Distinct())
        {
            if (!index.TryGetCandidates(tg, out var ids)) continue;
            foreach (var id in ids)
            {
                if (filter != SearchEntityType.All
                    && index.TryGetEntry(id, out var e)
                    && e.EntityType != filter) continue;
                counts[id] = counts.TryGetValue(id, out var c) ? c + 1 : 1;
            }
        }
        return counts;
    }
}
