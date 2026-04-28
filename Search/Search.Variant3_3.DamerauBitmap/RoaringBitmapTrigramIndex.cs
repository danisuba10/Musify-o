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
        _guidToIndex.Clear();
        _mutableIndex.Clear();
        _tombstones.Clear();

        foreach (var proj in projections)
        {
            int idx = _entries.Count;
            _entries.Add(proj);
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
