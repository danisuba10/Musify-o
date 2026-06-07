// Search.Variant4_2.FstAutomaton/FstTermIndex.cs
//
// Variant 4.2 — FST + Levenshtein automaton with lazy top-K admission.
//
// Current query path has two modes. Multi-token qualification semantics are
// inherited from current Variant 4.1 and extended with lazy baseline handling:
//   1) Baseline lazy heap-push (single-token / fallback mode): posting lists
//      are streamed into a bounded top-K min-heap, deduplicated by HashSet<int>.
//      A MaxScore-style bound can terminate remaining k-passes early once the
//      heap is saturated and future best-possible scores cannot displace it.
//   2) Multi-token coverage mode: candidates are qualified only after matching
//      a minimum number of distinct query tokens. If strict coverage is empty,
//      one relaxed pass is allowed, followed by a short-token rescue check
//      (subsequence heuristic) to recover typo cases such as "sft" -> "swift".
//
// This implementation therefore no longer models Variant 4.2 as only "remove
// minDist + add MaxScore"; it additionally applies token-coverage gating and
// adaptive fallback/rescue logic for multi-token typo robustness.
//
// Build pipeline and FST/posting layout are identical to Variant 4.1.
//
// Pipeline (build):
//   1. Tokenise + lower + diacritic-fold every name into a sorted, unique
//      vocabulary. Each unique term receives a stable termId (0..V-1).
//   2. Build an FST<long?> mapping termId encoded as the path output, using
//      INPUT_TYPE.BYTE4 (one transition per Unicode code point — matches
//      what LevenshteinAutomata produces).
//   3. Build a posting list per termId: a RoaringBitmap of the entity slot
//      indices (positions inside _entries) whose name contains the term.
//
// Pipeline (query):
//   1. Normalise + tokenise (deduplicated query tokens).
//   2. For each token choose kMax (0/1/2) from token length and query shape.
//   3. For k = 0..maxK, intersect FST with Levenshtein automata and stream
//      matched postings into either:
//         - direct heap admission (baseline mode), or
//         - coverage qualification state (multi-token mode).
//   4. In coverage mode, require a minimum token-match count; if strict mode
//      is empty, relax once and optionally rescue short-token typo cases with
//      a subsequence heuristic before final top-K admission.
//   5. Apply tombstone + entity-type filters and return deterministic score order.
//
// Memory footprint at 10 M entities is dominated by:
//   - the FST (compact byte array), and
//   - the Roaring posting lists.
// Both are reported via GC.GetTotalMemory()-deltas in BenchmarkMatrix.

using Equativ.RoaringBitmaps;
using Lucene.Net.Util;
using Lucene.Net.Util.Automaton;
using Lucene.Net.Util.Fst;
using Search.Abstractions;
using System.Numerics;
using System.Globalization;
using System.Text;
using FstOutput = J2N.Numerics.Int64;

namespace Search.Variant4_2.FstAutomaton;

internal sealed class FstTermIndex
{
    // Per-entity (slot-indexed) state — kept small to allow large N.
    private readonly List<SearchProjection> _entries     = new();
    private readonly List<string>           _nameLowers  = new();
    private readonly Dictionary<Guid, int>  _guidToIndex = new();
    private readonly HashSet<int>           _tombstones  = new();

    // Vocabulary state — terms sorted ordinally.
    private string[] _terms      = Array.Empty<string>();
    private RoaringBitmap[] _postings = Array.Empty<RoaringBitmap>();
    private FST<FstOutput>?  _fst;

    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

    private volatile bool _isReady;
    public bool IsReady => _isReady;

    // ── Tunables ─────────────────────────────────────────────────────────
    //
    // Maximum number of FST terms expanded per (token, k) pass. Mirrors
    // Lucene's FuzzyQuery.defaultMaxExpansions = 50 (Białecki, Muir & Ingersoll,
    // 2012, "Apache Lucene 4", §4.2). At k = 2 the Levenshtein automaton can
    // accept thousands of FST terms for short, common tokens; the union of
    // their posting lists is what dominates wall-clock under concurrent load.
    // Bounding the expansion bounds the work and the result-set size while
    // still surfacing the closest matches first (FST traversal visits arcs
    // in lexicographic order; the top-K stage downstream re-ranks by score).
    private const int MAX_EXPANSIONS_PER_PASS = 50;
    public int TermCount => _terms.Length;
    public int EntryCount => _entries.Count - _tombstones.Count;

    public bool TryGetEntry(Guid id, out SearchProjection entry)
    {
        _lock.EnterReadLock();
        try
        {
            if (_guidToIndex.TryGetValue(id, out var idx))
            {
                entry = _entries[idx];
                return true;
            }
            entry = default;
            return false;
        }
        finally { _lock.ExitReadLock(); }
    }

    // ── Build ─────────────────────────────────────────────────────────────

    public void Build(IEnumerable<SearchProjection> projections)
    {
        _entries.Clear();
        _nameLowers.Clear();
        _guidToIndex.Clear();
        _tombstones.Clear();

        // Term -> growing list of entity slot indices (during build).
        var termToIndices = new Dictionary<string, List<int>>(StringComparer.Ordinal);

        foreach (var proj in projections)
        {
            int idx = _entries.Count;
            _entries.Add(proj);
            string nameLower = Normalize(proj.Name);
            _nameLowers.Add(nameLower);
            _guidToIndex[proj.Id] = idx;

            foreach (var token in EnumerateTerms(nameLower))
            {
                if (!termToIndices.TryGetValue(token, out var list))
                    termToIndices[token] = list = new List<int>();
                // Avoid duplicate posting entries for the same (term, entity)
                // pair — common when a name has the same token twice.
                if (list.Count == 0 || list[^1] != idx) list.Add(idx);
            }
        }

        // Sort vocabulary ordinally for FST construction.
        var sortedTerms = new string[termToIndices.Count];
        termToIndices.Keys.CopyTo(sortedTerms, 0);
        Array.Sort(sortedTerms, StringComparer.Ordinal);

        _terms    = sortedTerms;
        _postings = new RoaringBitmap[sortedTerms.Length];

        // Build the FST: input = code-point ints, output = termId (long?).
        var outputs = PositiveInt32Outputs.Singleton;
        var builder = new Builder<FstOutput>(FST.INPUT_TYPE.BYTE4, outputs);
        var scratchInts = new Int32sRef();

        for (int termId = 0; termId < sortedTerms.Length; termId++)
        {
            var term = sortedTerms[termId];
            ToInt32sRef(term, scratchInts);
            builder.Add(scratchInts, FstOutput.GetInstance(termId));
            _postings[termId] = RoaringBitmap.Create(termToIndices[term]);
        }
        _fst = builder.Finish();

        _isReady = true;
    }

    // ── Mutations ─────────────────────────────────────────────────────────
    //
    // The FST is immutable. We accumulate adds/updates/removes against the
    // mutable per-entity arrays and rebuild the FST + postings lazily on the
    // next Build() invocation. For benchmarks Build() is called once at
    // startup, so mutation handlers are wire-compatible no-ops on the
    // term-level side; the search path still picks up new entities through
    // the linear-scan fallback inside Search() (skipped here to keep the
    // hot path identical to the published plan).

    public void Add(SearchProjection proj)
    {
        _lock.EnterWriteLock();
        try
        {
            int idx = _entries.Count;
            _entries.Add(proj);
            _nameLowers.Add(Normalize(proj.Name));
            _guidToIndex[proj.Id] = idx;
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Remove(Guid id, string name)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_guidToIndex.TryGetValue(id, out var idx)) return;
            _tombstones.Add(idx);
            _guidToIndex.Remove(id);
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Update(Guid id, string oldName, SearchProjection newProjection)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_guidToIndex.TryGetValue(id, out var oldIdx))
            {
                _tombstones.Add(oldIdx);
                _guidToIndex.Remove(id);
            }
            int newIdx = _entries.Count;
            _entries.Add(newProjection);
            _nameLowers.Add(Normalize(newProjection.Name));
            _guidToIndex[newProjection.Id] = newIdx;
        }
        finally { _lock.ExitWriteLock(); }
    }

    // ── Query ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a list of (Guid, score) pairs already ranked descending by
    /// score, capped at <paramref name="topK"/>. Scoring matches the rest of
    /// the search-variant family: <c>1/(1+minDist) + (minDist==0 ? 0.5 : 0)</c>
    /// where minDist is the smallest edit distance at which the entity was
    /// first admitted to the candidate heap.
    ///
    /// Variant 4.2 query evaluation:
    /// single-token queries use lazy heap-push with HashSet-based dedup and
    /// optional MaxScore-style early termination; multi-token queries use
    /// token-coverage qualification (strict then relaxed fallback) plus a
    /// short-token rescue check in the relaxed path. Scores remain compatible
    /// with the family baseline, with an additional small coverage boost in
    /// coverage mode to prefer candidates matching more query tokens.
    /// </summary>
    public List<(Guid Id, double Score)> Search(
        string normalisedQuery,
        SearchEntityType filter,
        int topK)
    {
        if (!_isReady || _fst is null) return new List<(Guid, double)>();
        if (string.IsNullOrEmpty(normalisedQuery)) return new List<(Guid, double)>();

        _lock.EnterReadLock();
        try
        {
            var queryTokens = new List<string>(4);
            var uniqueTokens = new HashSet<string>(StringComparer.Ordinal);
            foreach (var tok in TokeniseQuery(normalisedQuery))
            {
                if (uniqueTokens.Add(tok))
                    queryTokens.Add(tok);
            }
            if (queryTokens.Count == 0) return new List<(Guid, double)>();

            bool multiToken = queryTokens.Count > 1;

            // Pre-compute per-token kMax and the (lazy) Levenshtein automaton.
            // Building the automaton once per token (rather than per (token,k)
            // pass) is cheap because LevenshteinAutomata caches its parametric
            // construction across ToAutomaton(k) calls.
            var tokens = new List<(string Token, int KMax, LevenshteinAutomata? La)>(queryTokens.Count);
            int maxK = 0;
            foreach (var tok in queryTokens)
            {
                int kMax = ChooseK(tok.Length, multiToken);
                tokens.Add((tok, kMax,
                    kMax > 0 ? new LevenshteinAutomata(tok, withTranspositions: true) : null));
                if (kMax > maxK) maxK = kMax;
            }

            // For multi-token queries we enforce token coverage before admission
            // to avoid single common tokens (e.g. "taylor") flooding top-K.
            // If strict coverage yields no candidates, we relax once (to 1 token)
            // so typo-heavy inputs still return useful results.
            bool useCoverage = multiToken && tokens.Count <= 64;

            PriorityQueue<int, double> heap;

            if (!useCoverage)
            {
                heap = new PriorityQueue<int, double>(topK);
                var seen = new HashSet<int>(capacity: 256);

                for (int k = 0; k <= maxK; k++)
                {
                    double scoreAtK = 1.0 / (1.0 + k) + (k == 0 ? 0.5 : 0.0);

                    for (int tokenIdx = 0; tokenIdx < tokens.Count; tokenIdx++)
                    {
                        var (token, kMax, la) = tokens[tokenIdx];
                        if (k > kMax) continue;

                        if (k == 0)
                        {
                            long? exact = TryGetExact(token);
                            if (exact.HasValue)
                                AdmitPosting(_postings[(int)exact.Value], scoreAtK, filter, seen, heap, topK);
                        }
                        else
                        {
                            var auto = la!.ToAutomaton(k);
                            if (auto is null) continue;
                            var ra = new CharacterRunAutomaton(auto);

                            var matchedIds = new List<int>(64);
                            IntersectFstAutomaton(ra, matchedIds);
                            foreach (int termId in matchedIds)
                                AdmitPosting(_postings[termId], scoreAtK, filter, seen, heap, topK);
                        }
                    }

                    // Score-bounded early termination. Once the heap is full,
                    // every future admission would carry score 1/(1+(k+1)) < the
                    // worst kept score, so it could never displace anything.
                    if (heap.Count >= topK && k < maxK)
                    {
                        int nextK = k + 1;
                        double bestNext = 1.0 / (1.0 + nextK); // no exact-match bonus for k>=1
                        if (heap.TryPeek(out _, out double minScore) && minScore >= bestNext)
                            break;
                    }
                }
            }
            else
            {
                int requiredTokenMatches = ChooseRequiredTokenMatches(tokens.Count);
                int requiredThisPass = requiredTokenMatches;
                var qualifiedScores = new Dictionary<int, double>(capacity: 256);
                Dictionary<int, byte>? finalFirstDistByIdx = null;
                Dictionary<int, ulong>? finalTokenMaskByIdx = null;

                while (true)
                {
                    var firstDistByIdx = new Dictionary<int, byte>(capacity: 256);
                    var tokenMaskByIdx = new Dictionary<int, ulong>(capacity: 256);
                    qualifiedScores.Clear();

                    for (int k = 0; k <= maxK; k++)
                    {
                        for (int tokenIdx = 0; tokenIdx < tokens.Count; tokenIdx++)
                        {
                            var (token, kMax, la) = tokens[tokenIdx];
                            if (k > kMax) continue;

                            if (k == 0)
                            {
                                long? exact = TryGetExact(token);
                                if (exact.HasValue)
                                {
                                    AdmitPostingWithCoverage(
                                        _postings[(int)exact.Value],
                                        (byte)k,
                                        1UL << tokenIdx,
                                        filter,
                                        requiredThisPass,
                                        tokens.Count,
                                        firstDistByIdx,
                                        tokenMaskByIdx,
                                        qualifiedScores);
                                }
                            }
                            else
                            {
                                var auto = la!.ToAutomaton(k);
                                if (auto is null) continue;
                                var ra = new CharacterRunAutomaton(auto);

                                var matchedIds = new List<int>(64);
                                int expansionCap = ChooseExpansionCapForCoverage(token.Length, k);
                                IntersectFstAutomaton(ra, matchedIds, expansionCap);
                                foreach (int termId in matchedIds)
                                {
                                    AdmitPostingWithCoverage(
                                        _postings[termId],
                                        (byte)k,
                                        1UL << tokenIdx,
                                        filter,
                                        requiredThisPass,
                                        tokens.Count,
                                        firstDistByIdx,
                                        tokenMaskByIdx,
                                        qualifiedScores);
                                }
                            }
                        }
                    }

                    finalFirstDistByIdx = firstDistByIdx;
                    finalTokenMaskByIdx = tokenMaskByIdx;

                    if (qualifiedScores.Count > 0 || requiredThisPass <= 1)
                        break;

                    requiredThisPass = 1;
                }

                // If strict token coverage produced no matches and we had to
                // relax to single-token admission, recover precision by trying
                // a cheap name-token approximation for still-missing tokens.
                if (requiredTokenMatches > 1 && requiredThisPass == 1 &&
                    qualifiedScores.Count > 0 &&
                    finalFirstDistByIdx is not null &&
                    finalTokenMaskByIdx is not null)
                {
                    var rescuedScores = new Dictionary<int, double>(capacity: 64);
                    foreach (var kv in qualifiedScores)
                    {
                        int idx = kv.Key;
                        if (!finalTokenMaskByIdx.TryGetValue(idx, out ulong mask))
                            continue;

                        for (int tokenIdx = 0; tokenIdx < tokens.Count; tokenIdx++)
                        {
                            ulong bit = 1UL << tokenIdx;
                            if ((mask & bit) != 0) continue;

                            var (token, kMax, _) = tokens[tokenIdx];
                            if (NameContainsApproxToken(_nameLowers[idx], token, kMax))
                                mask |= bit;
                        }

                        int matchedTokenCount = BitOperations.PopCount(mask);
                        if (matchedTokenCount < requiredTokenMatches) continue;
                        if (!finalFirstDistByIdx.TryGetValue(idx, out byte firstDistance)) continue;

                        double baseScore = 1.0 / (1.0 + firstDistance) + (firstDistance == 0 ? 0.5 : 0.0);
                        double coverageBoost = tokens.Count <= 1 ? 0.0 : 0.2 * (matchedTokenCount - 1);
                        rescuedScores[idx] = baseScore + coverageBoost;
                    }

                    if (rescuedScores.Count > 0)
                        qualifiedScores = rescuedScores;
                }

                heap = new PriorityQueue<int, double>(topK);
                foreach (var kv in qualifiedScores)
                {
                    if (heap.Count < topK) heap.Enqueue(kv.Key, kv.Value);
                    else heap.EnqueueDequeue(kv.Key, kv.Value);
                }
            }

            if (heap.Count == 0) return new List<(Guid, double)>();

            // Drain heap descending.
            var ordered = new (int Idx, double Score)[heap.Count];
            for (int i = ordered.Length - 1; i >= 0; i--)
            {
                heap.TryDequeue(out int idx, out double sc);
                ordered[i] = (idx, sc);
            }
            Array.Sort(ordered, (a, b) =>
            {
                int c = b.Score.CompareTo(a.Score);
                return c != 0 ? c : _entries[b.Idx].Id.CompareTo(_entries[a.Idx].Id);
            });

            var result = new List<(Guid, double)>(ordered.Length);
            for (int i = 0; i < ordered.Length; i++)
                result.Add((_entries[ordered[i].Idx].Id, ordered[i].Score));
            return result;
        }
        finally { _lock.ExitReadLock(); }
    }

    private void AdmitPostingWithCoverage(
        RoaringBitmap posting,
        byte distance,
        ulong tokenMask,
        SearchEntityType filter,
        int requiredTokenMatches,
        int totalTokenCount,
        Dictionary<int, byte> firstDistByIdx,
        Dictionary<int, ulong> tokenMaskByIdx,
        Dictionary<int, double> qualifiedScores)
    {
        foreach (int idx in posting)
        {
            if (_tombstones.Contains(idx)) continue;
            if (filter != SearchEntityType.All)
            {
                var ent = _entries[idx];
                if (ent.EntityType != filter) continue;
            }

            if (!firstDistByIdx.ContainsKey(idx))
                firstDistByIdx[idx] = distance;

            tokenMaskByIdx.TryGetValue(idx, out ulong currentMask);
            ulong mergedMask = currentMask | tokenMask;
            if (mergedMask == currentMask) continue;
            tokenMaskByIdx[idx] = mergedMask;

            int matchedTokenCount = BitOperations.PopCount(mergedMask);
            if (matchedTokenCount < requiredTokenMatches) continue;

            byte firstDistance = firstDistByIdx[idx];
            double baseScore = 1.0 / (1.0 + firstDistance) + (firstDistance == 0 ? 0.5 : 0.0);
            double coverageBoost = totalTokenCount <= 1 ? 0.0 : 0.2 * (matchedTokenCount - 1);
            double score = baseScore + coverageBoost;

            if (!qualifiedScores.TryGetValue(idx, out double currentScore) || score > currentScore)
                qualifiedScores[idx] = score;
        }
    }

    private static bool NameContainsApproxToken(string nameLower, string token, int kMax)
    {
        foreach (var nameToken in nameLower.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (nameToken.Equals(token, StringComparison.Ordinal))
                return true;

            // Rescue path for ultra-short typo tokens where fuzzy expansion
            // can be over-pruned by expansion limits (e.g. "sft" -> "swift").
            if (kMax >= 1 && token.Length <= 3 && IsSubsequence(token, nameToken))
                return true;
        }

        return false;
    }

    private static bool IsSubsequence(string pattern, string text)
    {
        if (pattern.Length == 0) return true;
        if (text.Length == 0 || pattern.Length > text.Length) return false;

        int p = 0;
        for (int i = 0; i < text.Length && p < pattern.Length; i++)
            if (text[i] == pattern[p]) p++;
        return p == pattern.Length;
    }

    /// <summary>
    /// Stream a posting list directly into the bounded top-K heap. An entity
    /// slot is admitted only the first time it is seen across all (token, k)
    /// passes; subsequent occurrences are no-ops because the outer Search
    /// loop walks k in increasing order, so the first-seen score is also the
    /// best score that slot can earn in this query.
    /// </summary>
    private void AdmitPosting(
        RoaringBitmap posting,
        double score,
        SearchEntityType filter,
        HashSet<int> seen,
        PriorityQueue<int, double> heap,
        int topK)
    {
        foreach (int idx in posting)
        {
            if (!seen.Add(idx)) continue;
            if (_tombstones.Contains(idx)) continue;
            if (filter != SearchEntityType.All)
            {
                var ent = _entries[idx];
                if (ent.EntityType != filter) continue;
            }
            if (heap.Count < topK) heap.Enqueue(idx, score);
            else heap.EnqueueDequeue(idx, score);
        }
    }

    // ── FST × Automaton intersection ─────────────────────────────────────
    //
    // Recursive DFS over FST arcs, prunes any arc whose label is rejected by
    // the RunAutomaton's current state. Emits the termId encoded as the sum
    // of arc outputs along the path (PositiveInt32Outputs.Add is integer add).

    private static int ChooseExpansionCapForCoverage(int tokenLength, int distance)
    {
        if (tokenLength <= 3 && distance >= 1) return 20_000;
        if (tokenLength <= 4 && distance >= 2) return 4_000;
        return MAX_EXPANSIONS_PER_PASS;
    }

    private void IntersectFstAutomaton(CharacterRunAutomaton ra, List<int> matchedIds, int maxExpansions = MAX_EXPANSIONS_PER_PASS)
    {
        if (_fst is null) return;
        var br      = _fst.GetBytesReader();
        var outputs = PositiveInt32Outputs.Singleton;
        var rootArc = _fst.GetFirstArc(new FST.Arc<FstOutput>());

        WalkArc(rootArc, ra.InitialState, outputs.NoOutput, ra, br, outputs, matchedIds, maxExpansions);
    }

    private void WalkArc(
        FST.Arc<FstOutput> followArc,
        int autState,
        FstOutput outputAccum,
        CharacterRunAutomaton ra,
        FST.BytesReader br,
        PositiveInt32Outputs outputs,
        List<int> matchedIds,
        int maxExpansions)
    {
        if (_fst is null) return;
        // Lucene-style maxExpansions cap: stop the DFS once we've accumulated
        // enough candidate terms. Subsequent matches at this k would only
        // displace already-collected matches at the top-K stage in pathological
        // proportion to wall-clock cost.
        if (matchedIds.Count >= maxExpansions) return;

        if (!FST<FstOutput>.TargetHasArcs(followArc))
        {
            if (followArc.IsFinal && ra.IsAccept(autState))
                EmitTerm(outputs.Add(outputAccum, followArc.NextFinalOutput), matchedIds);
            return;
        }

        if (followArc.IsFinal && ra.IsAccept(autState))
            EmitTerm(outputs.Add(outputAccum, followArc.NextFinalOutput), matchedIds);

        var arc = new FST.Arc<FstOutput>();
        _fst.ReadFirstTargetArc(followArc, arc, br);
        while (true)
        {
            if (matchedIds.Count >= maxExpansions) return;

            int label = arc.Label;
            if (label != FST.END_LABEL)
            {
                int nextState = ra.Step(autState, label);
                if (nextState != -1)
                {
                    var nextOut = outputs.Add(outputAccum, arc.Output);
                    var snap = new FST.Arc<FstOutput>().CopyFrom(arc);
                    WalkArc(snap, nextState, nextOut, ra, br, outputs, matchedIds, maxExpansions);
                }
            }
            if (arc.IsLast) break;
            _fst.ReadNextArc(arc, br);
        }
    }

    private static void EmitTerm(FstOutput termIdOutput, List<int> matchedIds)
    {
        if (termIdOutput is null) return;
        matchedIds.Add((int)(long)termIdOutput);
    }

    private long? TryGetExact(string term)
    {
        if (_fst is null) return null;
        var scratch = new Int32sRef();
        ToInt32sRef(term, scratch);
        try
        {
            var result = Lucene.Net.Util.Fst.Util.Get(_fst, scratch);
            return result is null ? null : (long)result;
        }
        catch { return null; }
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private static int ChooseK(int len, bool multiToken)
    {
        if (!multiToken)
        {
            return len switch
            {
                <= 3 => 0,
                <= 6 => 1,
                _    => 2,
            };
        }

        return len switch
        {
            <= 2 => 0,
            <= 3 => 1,
            <= 6 => 1,
            _    => 2,
        };
    }

    private static int ChooseRequiredTokenMatches(int tokenCount)
    {
        if (tokenCount <= 1) return 1;
        if (tokenCount == 2) return 2;

        int sixtyPercent = (int)Math.Ceiling(tokenCount * 0.6);
        return Math.Min(tokenCount, Math.Max(2, sixtyPercent));
    }

    private static IEnumerable<string> TokeniseQuery(string normalised)
    {
        // Split on whitespace; also yield the joined string (for whole-name
        // matches when the vocabulary contains multi-word names — though our
        // build only inserts per-token entries, the joined form harmlessly
        // adds a longer-k fuzzy probe that almost never hits for short inputs).
        bool any = false;
        foreach (var tok in normalised.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            any = true;
            yield return tok;
        }
        if (!any) yield return normalised;
    }

    private static IEnumerable<string> EnumerateTerms(string normalisedName)
    {
        foreach (var tok in normalisedName.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (tok.Length == 0) continue;
            yield return tok;
        }
    }

    /// <summary>
    /// Lower-case and strip diacritics, matching what V3.3 implicitly does
    /// via <see cref="string.ToLowerInvariant"/> on already-ASCII input. We
    /// fold here so the FST's BYTE4 alphabet stays small.
    /// </summary>
    private static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        var lower = s.ToLowerInvariant();
        // Quick ASCII fast-path.
        bool needsFold = false;
        for (int i = 0; i < lower.Length; i++)
            if (lower[i] > 127) { needsFold = true; break; }
        if (!needsFold) return lower;

        var formD = lower.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(formD.Length);
        foreach (var ch in formD)
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Encode a string as a sequence of Unicode code points into an
    /// <see cref="Int32sRef"/> — required by FST.INPUT_TYPE.BYTE4.
    /// </summary>
    private static void ToInt32sRef(string s, Int32sRef scratch)
    {
        if (scratch.Int32s.Length < s.Length) scratch.Int32s = new int[s.Length];
        int n = 0;
        for (int i = 0; i < s.Length; )
        {
            int cp = char.ConvertToUtf32(s, i);
            scratch.Int32s[n++] = cp;
            i += char.IsSurrogatePair(s, i) ? 2 : 1;
        }
        scratch.Offset = 0;
        scratch.Length = n;
    }
}
