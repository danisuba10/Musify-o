using Search.Abstractions;

namespace Search.Variant3_1.DamerauBasic;

internal sealed class TrigramIndex
{
    private Dictionary<string, HashSet<Guid>> _index = new();
    private Dictionary<Guid, SearchProjection> _entries = new();
    // Pre-lowered names — eliminates per-candidate ToLowerInvariant() in scoring loop.
    private Dictionary<Guid, string> _namesLower = new();
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

    private volatile bool _isReady = false;
    public bool IsReady => _isReady;

    public void Build(IEnumerable<SearchProjection> projections)
    {
        var newIndex      = new Dictionary<string, HashSet<Guid>>(StringComparer.Ordinal);
        var newEntries    = new Dictionary<Guid, SearchProjection>();
        var newNamesLower = new Dictionary<Guid, string>();

        foreach (var proj in projections)
        {
            newEntries[proj.Id]    = proj;
            newNamesLower[proj.Id] = proj.Name.ToLowerInvariant();
            foreach (var trigram in GetTrigrams(proj.Name))
            {
                if (!newIndex.TryGetValue(trigram, out var set))
                {
                    set = new HashSet<Guid>();
                    newIndex[trigram] = set;
                }
                set.Add(proj.Id);
            }
        }

        _index      = newIndex;
        _entries    = newEntries;
        _namesLower = newNamesLower;
        _isReady    = true;
    }

    public void Add(SearchProjection proj)
    {
        _lock.EnterWriteLock();
        try
        {
            _entries[proj.Id]    = proj;
            _namesLower[proj.Id] = proj.Name.ToLowerInvariant();
            foreach (var trigram in GetTrigrams(proj.Name))
            {
                if (!_index.TryGetValue(trigram, out var set))
                    _index[trigram] = set = new HashSet<Guid>();
                set.Add(proj.Id);
            }
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Remove(Guid id, string name)
    {
        _lock.EnterWriteLock();
        try
        {
            _entries.Remove(id);
            _namesLower.Remove(id);
            foreach (var trigram in GetTrigrams(name))
            {
                if (!_index.TryGetValue(trigram, out var set)) continue;
                set.Remove(id);
                if (set.Count == 0) _index.Remove(trigram);
            }
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Update(Guid id, string oldName, SearchProjection newProjection)
    {
        _lock.EnterWriteLock();
        try
        {
            _entries.Remove(id);
            _namesLower.Remove(id);
            foreach (var trigram in GetTrigrams(oldName))
            {
                if (!_index.TryGetValue(trigram, out var set)) continue;
                set.Remove(id);
                if (set.Count == 0) _index.Remove(trigram);
            }
            _entries[newProjection.Id]    = newProjection;
            _namesLower[newProjection.Id] = newProjection.Name.ToLowerInvariant();
            foreach (var trigram in GetTrigrams(newProjection.Name))
            {
                if (!_index.TryGetValue(trigram, out var set))
                    _index[trigram] = set = new HashSet<Guid>();
                set.Add(newProjection.Id);
            }
        }
        finally { _lock.ExitWriteLock(); }
    }

    public bool TryGetCandidates(string trigram, out HashSet<Guid> ids)
    {
        _lock.EnterReadLock();
        try   { return _index.TryGetValue(trigram, out ids!); }
        finally { _lock.ExitReadLock(); }
    }

    public bool TryGetEntry(Guid id, out SearchProjection entry)
    {
        _lock.EnterReadLock();
        try   { return _entries.TryGetValue(id, out entry); }
        finally { _lock.ExitReadLock(); }
    }

    /// <summary>
    /// Single-pass adaptive trigram-overlap filter — see Variant 2.1 for full rationale.
    /// Returns (Guid, pre-lowered name) tuples so the scoring loop never has to call
    /// ToLowerInvariant() or re-acquire the lock per candidate.
    /// </summary>
    public List<(Guid Id, string NameLower)> GetFilteredCandidates(
        string normalisedQuery,
        SearchEntityType filter = SearchEntityType.All)
    {
        _lock.EnterReadLock();
        try
        {
            var trigrams = new HashSet<string>(StringComparer.Ordinal);
            foreach (var tg in GetTrigrams(normalisedQuery)) trigrams.Add(tg);
            if (trigrams.Count == 0) return [];

            int tc = trigrams.Count;
            int minShared = tc switch
            {
                <= 4 => 1,
                <= 6 => 2,
                <= 9 => Math.Max(2, tc - 6),
                _    => Math.Max(2, tc / 3)
            };

            var counts = new Dictionary<Guid, int>(capacity: 1024);
            foreach (var trigram in trigrams)
            {
                if (!_index.TryGetValue(trigram, out var ids)) continue;
                foreach (var id in ids)
                {
                    counts.TryGetValue(id, out int c);
                    counts[id] = c + 1;
                }
            }

            var result = new List<(Guid, string)>(64);
            foreach (var (id, cnt) in counts)
            {
                if (cnt < minShared) continue;
                if (!_entries.TryGetValue(id, out var entry)) continue;
                if (filter != SearchEntityType.All && entry.EntityType != filter) continue;
                if (!_namesLower.TryGetValue(id, out var nl)) continue;
                result.Add((id, nl));
            }
            return result;
        }
        finally { _lock.ExitReadLock(); }
    }

    public int EntryCount
    {
        get
        {
            _lock.EnterReadLock();
            try   { return _entries.Count; }
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
