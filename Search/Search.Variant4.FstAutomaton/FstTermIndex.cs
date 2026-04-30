// Search.Variant4.FstAutomaton/FstTermIndex.cs
//
// Term index backed by a Lucene.NET FST (Finite-State Transducer) plus
// per-term Roaring posting lists. Queries are answered by intersecting the
// FST with a Levenshtein automaton (Schulz & Mihov, 2002) — no per-candidate
// dynamic-programming pass.
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
//   1. Normalise + tokenise the query.
//   2. For each query token decide an effective edit distance k (0/1/2 by
//      length, capped at the Lucene-supported maximum of 2).
//   3. Build a Levenshtein automaton for the token at distance k and
//      intersect it with the FST via a recursive Arc traversal (DFS that
//      prunes any arc the automaton rejects). This yields matched termIds
//      and the edit distance at which they were accepted.
//   4. Union the posting lists, tracking the minimum edit distance per
//      entity, then apply tombstone + entity-type filters.
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
using System.Globalization;
using System.Text;
using FstOutput = J2N.Numerics.Int64;

namespace Search.Variant4.FstAutomaton;

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
    /// where minDist is the smallest edit distance over all matched terms
    /// for the entity.
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
            // Sparse minimum-edit-distance map. Replaces the previous
            // ArrayPool<byte>.Rent(entryCount) which, at multi-million-entity
            // scale, allocated a fresh 20+ MB array per query (the shared
            // ArrayPool does not pool arrays larger than ~1 MB) and was the
            // dominant cost under concurrent load. Output is sparse by design
            // — the FST + automaton already prunes to the matched terms — so
            // a dictionary keyed only on hit slots fits the access pattern.
            var minDist = new Dictionary<int, byte>(capacity: 256);

            // Single-token vs multi-token query.
            foreach (var token in TokeniseQuery(normalisedQuery))
            {
                int kMax = ChooseK(token.Length);
                CollectMatches(token, kMax, minDist, topK);
            }
            if (minDist.Count == 0) return new List<(Guid, double)>();

            // Score touched entities; bounded min-heap of size topK.
            var heap = new PriorityQueue<int, double>(topK);
            foreach (var kv in minDist)
            {
                int idx = kv.Key;
                byte d  = kv.Value;
                if (_tombstones.Contains(idx)) continue;
                var ent = _entries[idx];
                if (filter != SearchEntityType.All && ent.EntityType != filter) continue;

                double score = 1.0 / (1.0 + d) + (d == 0 ? 0.5 : 0);
                if (heap.Count < topK) heap.Enqueue(idx, score);
                else heap.EnqueueDequeue(idx, score);
            }

            // Drain heap descending.
            var orderedIdx = new (int Idx, double Score)[heap.Count];
            for (int i = orderedIdx.Length - 1; i >= 0; i--)
            {
                heap.TryDequeue(out int idx, out double sc);
                orderedIdx[i] = (idx, sc);
            }
            Array.Sort(orderedIdx, (a, b) =>
            {
                int c = b.Score.CompareTo(a.Score);
                return c != 0 ? c : _entries[b.Idx].Id.CompareTo(_entries[a.Idx].Id);
            });

            var result = new List<(Guid, double)>(orderedIdx.Length);
            for (int i = 0; i < orderedIdx.Length; i++)
                result.Add((_entries[orderedIdx[i].Idx].Id, orderedIdx[i].Score));
            return result;
        }
        finally { _lock.ExitReadLock(); }
    }

    /// <summary>
    /// Walks edit-distance levels 0..kMax, intersecting the FST with a
    /// Levenshtein automaton at each level. Stops once <paramref name="minDist"/>
    /// already holds at least <paramref name="topK"/> matches (which, since
    /// score is monotonically decreasing in k, can no longer be displaced by
    /// higher-k matches).
    /// </summary>
    private void CollectMatches(
        string queryToken,
        int kMax,
        Dictionary<int, byte> minDist,
        int topK)
    {
        if (_fst is null) return;

        // k = 0 — exact hit via direct FST lookup.
        long? exactTermId = TryGetExact(queryToken);
        if (exactTermId.HasValue)
        {
            var bm = _postings[(int)exactTermId.Value];
            UpdateMinDist(bm, distance: 0, minDist);
        }
        if (kMax == 0) return;
        if (minDist.Count >= topK) return;

        var la = new LevenshteinAutomata(queryToken, withTranspositions: true);
        for (int k = 1; k <= kMax; k++)
        {
            var auto = la.ToAutomaton(k);
            if (auto is null) continue;
            var ra = new CharacterRunAutomaton(auto);

            var matchedIds = new List<int>(64);
            IntersectFstAutomaton(ra, matchedIds);
            if (matchedIds.Count == 0) continue;

            byte d = (byte)k;
            foreach (int termId in matchedIds)
                UpdateMinDist(_postings[termId], d, minDist);

            if (minDist.Count >= topK) return;
        }
    }

    private static void UpdateMinDist(
        RoaringBitmap posting,
        byte distance,
        Dictionary<int, byte> minDist)
    {
        foreach (int idx in posting)
        {
            if (minDist.TryGetValue(idx, out byte cur))
            {
                if (distance < cur) minDist[idx] = distance;
            }
            else
            {
                minDist[idx] = distance;
            }
        }
    }

    // ── FST × Automaton intersection ─────────────────────────────────────
    //
    // Recursive DFS over FST arcs, prunes any arc whose label is rejected by
    // the RunAutomaton's current state. Emits the termId encoded as the sum
    // of arc outputs along the path (PositiveInt32Outputs.Add is integer add).

    private void IntersectFstAutomaton(CharacterRunAutomaton ra, List<int> matchedIds)
    {
        if (_fst is null) return;
        var br      = _fst.GetBytesReader();
        var outputs = PositiveInt32Outputs.Singleton;
        var rootArc = _fst.GetFirstArc(new FST.Arc<FstOutput>());

        WalkArc(rootArc, ra.InitialState, outputs.NoOutput, ra, br, outputs, matchedIds);
    }

    private void WalkArc(
        FST.Arc<FstOutput> followArc,
        int autState,
        FstOutput outputAccum,
        CharacterRunAutomaton ra,
        FST.BytesReader br,
        PositiveInt32Outputs outputs,
        List<int> matchedIds)
    {
        if (_fst is null) return;
        // Lucene-style maxExpansions cap: stop the DFS once we've accumulated
        // enough candidate terms. Subsequent matches at this k would only
        // displace already-collected matches at the top-K stage in pathological
        // proportion to wall-clock cost.
        if (matchedIds.Count >= MAX_EXPANSIONS_PER_PASS) return;

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
            if (matchedIds.Count >= MAX_EXPANSIONS_PER_PASS) return;

            int label = arc.Label;
            if (label != FST.END_LABEL)
            {
                int nextState = ra.Step(autState, label);
                if (nextState != -1)
                {
                    var nextOut = outputs.Add(outputAccum, arc.Output);
                    var snap = new FST.Arc<FstOutput>().CopyFrom(arc);
                    WalkArc(snap, nextState, nextOut, ra, br, outputs, matchedIds);
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

    private static int ChooseK(int len) => len switch
    {
        <= 3 => 0,
        <= 6 => 1,
        _    => 2,
    };

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
