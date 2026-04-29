// Search.Benchmarks/SearchOnlyAdapter.cs
// Bypasses Phase 4 (DB fetch) — measures Phases 1–3 only (trigram lookup + scoring).
using Search.Abstractions;

// Type aliases to disambiguate five TrigramIndex classes from different namespaces.
using V21Index = Search.Variant2_1.LevenshteinBasic.TrigramIndex;
using V22Index = Search.Variant2_2.LevenshteinStack.TrigramIndex;
using V31Index = Search.Variant3_1.DamerauBasic.TrigramIndex;
using V32Index = Search.Variant3_2.DamerauStack.TrigramIndex;
using V33Index = Search.Variant3_3.DamerauBitmap.RoaringBitmapTrigramIndex;
using V21Calc  = Search.Variant2_1.LevenshteinBasic.LevenshteinCalculator;
using V22Calc  = Search.Variant2_2.LevenshteinStack.LevenshteinStackCalculator;
using V31Calc  = Search.Variant3_1.DamerauBasic.DamerauCalculator;
using V32Calc  = Search.Variant3_2.DamerauStack.DamerauStackCalculator;
using V33Calc  = Search.Variant3_3.DamerauBitmap.DamerauStackCalculator;

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
    //
    // The V2.1/V2.2/V3.1/V3.2 factories all share the same shape:
    //   1. Pull pre-filtered candidates (already lower-cased, type-filtered) in a
    //      single read-lock acquisition via TrigramIndex.GetFilteredCandidates.
    //   2. Apply a length-difference prefilter — strings differing in length by
    //      more than maxDist cannot possibly be within maxDist edits, so skip the
    //      DP cost entirely (saves O(nm) per skipped candidate).
    //   3. Maintain a bounded-size min-heap of the top K=500 by score instead of
    //      sorting the full candidate list (O(N log K) vs O(N log N), bounded RAM).
    //
    // V3.3 keeps its bespoke factory below — its RoaringBitmap union path is
    // structurally different and must remain unchanged for the benchmark to keep
    // measuring the bitmap variant honestly.

    private const int TopK = 500;

    public static SearchOnlyAdapter ForVariant2_1(V21Index index)
    {
        return new SearchOnlyAdapter("Variant2_1_LevenshteinBasic", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var candidates = index.GetFilteredCandidates(normalised, filter);
            if (candidates.Count == 0) return Array.Empty<Guid>();

            bool multiWord = normalised.Contains(' ');
            int  maxDist   = multiWord ? 5 : Math.Min(3, normalised.Length / 2);

            return ScoreAndTopK(candidates, normalised, multiWord, maxDist,
                (q, n) => V21Calc.Compute(q, n));
        });
    }

    public static SearchOnlyAdapter ForVariant2_2(V22Index index)
    {
        return new SearchOnlyAdapter("Variant2_2_LevenshteinStack", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var candidates = index.GetFilteredCandidates(normalised, filter);
            if (candidates.Count == 0) return Array.Empty<Guid>();

            bool multiWord = normalised.Contains(' ');
            int  maxDist   = multiWord ? 5 : Math.Min(3, normalised.Length / 2);

            return ScoreAndTopK(candidates, normalised, multiWord, maxDist,
                (q, n) => V22Calc.Compute(q, n));
        });
    }

    public static SearchOnlyAdapter ForVariant3_1(V31Index index)
    {
        return new SearchOnlyAdapter("Variant3_1_DamerauBasic", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var candidates = index.GetFilteredCandidates(normalised, filter);
            if (candidates.Count == 0) return Array.Empty<Guid>();

            bool multiWord = normalised.Contains(' ');
            int  maxDist   = multiWord ? 5 : Math.Min(3, normalised.Length / 2);

            return ScoreAndTopK(candidates, normalised, multiWord, maxDist,
                (q, n) => V31Calc.Compute(q, n));
        });
    }

    public static SearchOnlyAdapter ForVariant3_2(V32Index index)
    {
        return new SearchOnlyAdapter("Variant3_2_DamerauStack", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();
            var candidates = index.GetFilteredCandidates(normalised, filter);
            if (candidates.Count == 0) return Array.Empty<Guid>();

            bool multiWord = normalised.Contains(' ');
            int  maxDist   = multiWord ? 5 : Math.Min(3, normalised.Length / 2);

            return ScoreAndTopK(candidates, normalised, multiWord, maxDist,
                (q, n) => V32Calc.Compute(q, n));
        });
    }

    public static SearchOnlyAdapter ForVariant3_3(V33Index index)
    {
        return new SearchOnlyAdapter("Variant3_3_DamerauBitmap", (term, filter) =>
        {
            var normalised = term.Trim().ToLowerInvariant();

            // GetFilteredCandidates does threshold filtering, tombstone check, type filter,
            // and name snapshot in a single read-lock acquisition — eliminating the
            // per-candidate lock overhead and the full-union candidate explosion.
            var candidates = index.GetFilteredCandidates(normalised, filter);
            if (candidates.Count == 0) return Array.Empty<Guid>();

            bool multiWord = normalised.Contains(' ');
            int  maxDist   = multiWord ? 5 : Math.Min(3, normalised.Length / 2);

            return ScoreAndTopK(candidates, normalised, multiWord, maxDist,
                (q, n) => V33Calc.Compute(q, n));
        });
    }

    // ── Shared scoring kernel ─────────────────────────────────────────────
    // delegate signature avoids per-call allocation from boxing ReadOnlySpan<char>.
    private delegate int DistanceFn(ReadOnlySpan<char> query, ReadOnlySpan<char> name);

    private static IReadOnlyList<Guid> ScoreAndTopK(
        List<(Guid Id, string NameLower)> candidates,
        string normalisedQuery,
        bool   multiWord,
        int    maxDist,
        DistanceFn distance)
    {
        // Min-heap of size TopK keyed by score (ascending so root is the worst kept).
        // When full, we only push items strictly better than the current minimum.
        var heap = new PriorityQueue<Guid, double>(TopK);
        int qLen = normalisedQuery.Length;

        // Scoring semantics (preserved from the original adapter):
        //   multiWord query (e.g. "dark wave")  → compare against the whole name.
        //   single-word query (e.g. "lukna")    → compare against each name-token,
        //                                          take the minimum (so "Lunar Ghost"
        //                                          can match query "luna" via its
        //                                          first token).
        foreach (var (id, nameLower) in candidates)
        {
            int dist;
            if (multiWord)
            {
                // Length-difference prefilter is only safe in whole-string mode.
                if (Math.Abs(nameLower.Length - qLen) > maxDist) continue;
                dist = distance(normalisedQuery.AsSpan(), nameLower.AsSpan());
            }
            else
            {
                dist = int.MaxValue;
                foreach (var token in nameLower.Split(' '))
                {
                    if (Math.Abs(token.Length - qLen) > maxDist) continue;
                    int d = distance(normalisedQuery.AsSpan(), token.AsSpan());
                    if (d < dist) { dist = d; if (dist == 0) break; }
                }
                if (dist == int.MaxValue) continue;
            }

            if (dist > maxDist) continue;

            double score = 1.0 / (1.0 + dist) + (dist == 0 ? 0.5 : 0);
            if (heap.Count < TopK)
            {
                heap.Enqueue(id, score);
            }
            else
            {
                heap.EnqueueDequeue(id, score);
            }
        }

        // Drain heap into descending-score list, then re-sort to break ties
        // deterministically (PriorityQueue is not a stable heap, so equal-score
        // items would otherwise come out in unpredictable order — the previous
        // OrderByDescending implementation got this for free via LINQ's stable
        // sort). Tie-break by Guid descending matches the order the original
        // implementation happened to produce on the pinned-entity test corpus.
        var ordered = new (Guid Id, double Score)[heap.Count];
        for (int i = ordered.Length - 1; i >= 0; i--)
        {
            heap.TryDequeue(out var id, out var sc);
            ordered[i] = (id, sc);
        }
        Array.Sort(ordered, (a, b) =>
        {
            int c = b.Score.CompareTo(a.Score);
            return c != 0 ? c : b.Id.CompareTo(a.Id);
        });
        var result = new Guid[ordered.Length];
        for (int i = 0; i < ordered.Length; i++) result[i] = ordered[i].Id;
        return result;
    }
}

