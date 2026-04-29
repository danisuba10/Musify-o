using Equativ.RoaringBitmaps;
using Search.Abstractions;

namespace Search.Variant3_3.DamerauBitmap;

/// <summary>
/// Trigram index backed by RoaringBitmaps for compact, fast set-union operations.
///
/// The Equativ.RoaringBitmaps library is fully immutable:
///   - Bitmaps are created with RoaringBitmap.Create.
///   - OR is performed via the | operator (returns a new bitmap).
///   - Iteration yields int values.
///
/// Design:
/// - _entries: dense List&lt;SearchProjection&gt; — each entity gets a stable int index.
/// - _guidToIndex: Guid → int position lookup.
/// - _mutableIndex: trigram → List&lt;int&gt; used to accumulate indices.
/// - _frozenIndex: trigram → frozen RoaringBitmap (rebuilt after mutations).
/// - _tombstones: HashSet&lt;int&gt; of deleted slot indices.
/// </summary>
internal sealed class RoaringBitmapTrigramIndex
{
    private readonly List<SearchProjection>            _entries      = new();
    // Pre-stored lowercase names — avoids ToLowerInvariant() allocations per candidate during search.
    private readonly List<string>                      _nameLowers   = new();
    private readonly Dictionary<Guid, int>             _guidToIndex  = new();
    private readonly Dictionary<string, List<int>>     _mutableIndex = new(StringComparer.Ordinal);
    private          Dictionary<string, RoaringBitmap> _frozenIndex  = new(StringComparer.Ordinal);
    private readonly HashSet<int>                      _tombstones   = new();
    private readonly ReaderWriterLockSlim              _lock         = new(LockRecursionPolicy.NoRecursion);

    private volatile bool _isReady = false;
    public bool IsReady => _isReady;

    public void Build(IEnumerable<SearchProjection> projections)
    {
        _entries.Clear();
        _nameLowers.Clear();
        _guidToIndex.Clear();
        _mutableIndex.Clear();
        _tombstones.Clear();

        foreach (var proj in projections)
        {
            int idx = _entries.Count;
            _entries.Add(proj);
            _nameLowers.Add(proj.Name.ToLowerInvariant());
            _guidToIndex[proj.Id] = idx;

            foreach (var trigram in GetTrigrams(proj.Name))
            {
                if (!_mutableIndex.TryGetValue(trigram, out var list))
                    _mutableIndex[trigram] = list = new List<int>();
                list.Add(idx);
            }
        }

        var frozen = new Dictionary<string, RoaringBitmap>(_mutableIndex.Count, StringComparer.Ordinal);
        foreach (var kv in _mutableIndex)
            frozen[kv.Key] = RoaringBitmap.Create(kv.Value);
        _frozenIndex = frozen;

        _isReady = true;
    }

    public void Add(SearchProjection proj)
    {
        _lock.EnterWriteLock();
        try
        {
            int idx = _entries.Count;
            _entries.Add(proj);
            _nameLowers.Add(proj.Name.ToLowerInvariant());
            _guidToIndex[proj.Id] = idx;

            foreach (var trigram in GetTrigrams(proj.Name))
            {
                if (!_mutableIndex.TryGetValue(trigram, out var list))
                    _mutableIndex[trigram] = list = new List<int>();
                list.Add(idx);
                _frozenIndex[trigram] = RoaringBitmap.Create(list);
            }
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
            _nameLowers.Add(newProjection.Name.ToLowerInvariant());
            _guidToIndex[newProjection.Id] = newIdx;

            foreach (var trigram in GetTrigrams(newProjection.Name))
            {
                if (!_mutableIndex.TryGetValue(trigram, out var list))
                    _mutableIndex[trigram] = list = new List<int>();
                list.Add(newIdx);
                _frozenIndex[trigram] = RoaringBitmap.Create(list);
            }
        }
        finally { _lock.ExitWriteLock(); }
    }

    public RoaringBitmap? GetCandidateBitmap(string queryTerm)
    {
        _lock.EnterReadLock();
        try
        {
            RoaringBitmap? result = null;
            foreach (var trigram in GetTrigrams(queryTerm).Distinct())
            {
                if (!_frozenIndex.TryGetValue(trigram, out var bm)) continue;
                result = result == null ? bm : result | bm;
            }
            return result;
        }
        finally { _lock.ExitReadLock(); }
    }

    /// <summary>
    /// Single read-lock pass that:
    /// 1. Counts how many distinct query trigrams each candidate matches.
    /// 2. Keeps only candidates meeting an adaptive threshold (fewer misses = more recall;
    ///    more misses = lower candidate count = faster Levenshtein phase).
    /// 3. Filters tombstones and entity-type in the same pass.
    /// 4. Returns (Guid, pre-lowered name) tuples — no further lock needed by the caller.
    ///
    /// Threshold formula (internal):
    ///   trigramCount ≤ 4  → 1  (short queries: full union — recall matters more)
    ///   trigramCount 5–6  → 2
    ///   trigramCount 7–9  → max(2, trigramCount − 4)   (allow up to 4 misses for typos)
    ///   trigramCount ≥ 10 → max(3, trigramCount / 2)   (long queries: require ≥ half)
    /// </summary>
    public List<(Guid Id, string NameLower)> GetFilteredCandidates(
        string normalisedQuery,
        SearchEntityType filter = SearchEntityType.All)
    {
        _lock.EnterReadLock();
        try
        {
            var trigrams = GetTrigrams(normalisedQuery).Distinct().ToList();
            if (trigrams.Count == 0) return [];

            int tc = trigrams.Count;
            // Threshold: how many query trigrams must a candidate share to be kept.
            // Transpositions destroy trigram pairs completely ("dark"→"drak" shares 0 of 2),
            // so thresholds must be lenient enough to survive worst-case two-edit typos.
            // Verified against every two-edit assertion in CorrectnessValidator.
            int minShared = tc switch
            {
                <= 4 => 1,
                <= 6 => 2,
                <= 9 => Math.Max(2, tc - 6),   // allows up to 6 misses (tc=9 → 3)
                _    => Math.Max(2, tc / 3)     // allows up to 2/3 to be lost to transpositions
            };

            // Count how many query trigrams each candidate index matches.
            var counts = new Dictionary<int, int>(capacity: 1024);
            foreach (var trigram in trigrams)
            {
                if (!_frozenIndex.TryGetValue(trigram, out var bm)) continue;
                foreach (int idx in bm)
                {
                    counts.TryGetValue(idx, out int c);
                    counts[idx] = c + 1;
                }
            }

            // Collect candidates that pass the threshold, are alive, and match the type filter.
            var result = new List<(Guid, string)>(64);
            foreach (var (idx, cnt) in counts)
            {
                if (cnt < minShared) continue;
                if (_tombstones.Contains(idx)) continue;
                var entry = _entries[idx];
                if (filter != SearchEntityType.All && entry.EntityType != filter) continue;
                result.Add((entry.Id, _nameLowers[idx]));
            }
            return result;
        }
        finally { _lock.ExitReadLock(); }
    }

    public bool IsTombstoned(int idx)
    {
        _lock.EnterReadLock();
        try   { return _tombstones.Contains(idx); }
        finally { _lock.ExitReadLock(); }
    }

    public SearchProjection GetEntry(int idx)
    {
        _lock.EnterReadLock();
        try   { return _entries[idx]; }
        finally { _lock.ExitReadLock(); }
    }

    public bool TryGetIndex(Guid id, out int idx)
    {
        _lock.EnterReadLock();
        try   { return _guidToIndex.TryGetValue(id, out idx); }
        finally { _lock.ExitReadLock(); }
    }

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

    public int EntryCount
    {
        get
        {
            _lock.EnterReadLock();
            try   { return _guidToIndex.Count; }
            finally { _lock.ExitReadLock(); }
        }
    }

    internal static IEnumerable<string> GetTrigrams(string input)
    {
        if (string.IsNullOrEmpty(input)) yield break;
        var padded = "  " + input.ToLowerInvariant() + " ";
        for (int i = 0; i <= padded.Length - 3; i++)
            yield return padded.Substring(i, 3);
    }
}
