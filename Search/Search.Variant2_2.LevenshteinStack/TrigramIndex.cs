using Search.Abstractions;

namespace Search.Variant2_2.LevenshteinStack;

internal sealed class TrigramIndex
{
    private Dictionary<string, HashSet<Guid>> _index = new();
    private Dictionary<Guid, SearchProjection> _entries = new();
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

    private volatile bool _isReady = false;
    public bool IsReady => _isReady;

    public void Build(IEnumerable<SearchProjection> projections)
    {
        var newIndex   = new Dictionary<string, HashSet<Guid>>(StringComparer.Ordinal);
        var newEntries = new Dictionary<Guid, SearchProjection>();

        foreach (var proj in projections)
        {
            newEntries[proj.Id] = proj;
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

        _index   = newIndex;
        _entries = newEntries;
        _isReady = true;
    }

    public void Add(SearchProjection proj)
    {
        _lock.EnterWriteLock();
        try
        {
            _entries[proj.Id] = proj;
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
            foreach (var trigram in GetTrigrams(oldName))
            {
                if (!_index.TryGetValue(trigram, out var set)) continue;
                set.Remove(id);
                if (set.Count == 0) _index.Remove(trigram);
            }
            _entries[newProjection.Id] = newProjection;
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
