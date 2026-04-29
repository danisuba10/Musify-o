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
            // Single-pass trigram extraction: distinct set + matching bitmaps + union.
            // No iterator, no LINQ, no intermediate List allocation.
            if (string.IsNullOrEmpty(normalisedQuery)) return [];
            var padded = "  " + normalisedQuery + " ";
            int pCount = padded.Length - 2;
            if (pCount <= 0) return [];

            var distinct = new HashSet<string>(capacity: pCount, StringComparer.Ordinal);
            var bitmaps  = new List<RoaringBitmap>(pCount);
            RoaringBitmap? union = null;
            for (int i = 0; i < pCount; i++)
            {
                var tri = padded.Substring(i, 3);
                if (!distinct.Add(tri)) continue;
                if (!_frozenIndex.TryGetValue(tri, out var bm)) continue;
                bitmaps.Add(bm);
                union = union == null ? bm : union | bm;
            }

            int tc = distinct.Count;
            if (tc == 0 || union == null) return [];

            // Threshold (unchanged): how many query trigrams must a candidate share.
            int minShared = tc switch
            {
                <= 4 => 1,
                <= 6 => 2,
                <= 9 => Math.Max(2, tc - 6),
                _    => Math.Max(2, tc / 3)
            };

            // Pooled byte[] candidate counter (was: Dictionary<int,int>).
            // For hot trigrams over 1M entities the dict resized through hundreds
            // of thousands of slots per query and dominated CPU + GC pressure.
            // byte[] is O(1) per write with zero per-query allocation; minShared
            // is small (<= ~25) so saturation at 255 is irrelevant.
            int entriesCount = _entries.Count;
            var counts = System.Buffers.ArrayPool<byte>.Shared.Rent(entriesCount);
            try
            {
                foreach (var bm in bitmaps)
                    foreach (int idx in bm)
                    {
                        if (counts[idx] < 255) counts[idx]++;
                    }

                // Iterate the union bitmap (much smaller than a full entries scan)
                // and harvest candidates that meet the shared-trigram threshold.
                // Each touched cell is reset to 0 inline so the rented buffer can
                // be returned without a second clear pass.
                var result = new List<(Guid, string)>(64);
                bool typeFiltered = filter != SearchEntityType.All;
                foreach (int idx in union)
                {
                    byte cnt = counts[idx];
                    counts[idx] = 0;
                    if (cnt < minShared) continue;
                    if (_tombstones.Contains(idx)) continue;
                    var entry = _entries[idx];
                    if (typeFiltered && entry.EntityType != filter) continue;
                    result.Add((entry.Id, _nameLowers[idx]));
                }
                return result;
            }
            finally
            {
                System.Buffers.ArrayPool<byte>.Shared.Return(counts, clearArray: false);
            }
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
