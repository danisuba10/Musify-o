using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Search.Abstractions;

namespace Search.Variant2_2.LevenshteinStack;

internal sealed class LevenshteinStackSearchEngine : ISearchEngine
{
    public string VariantName => "Variant2_2_LevenshteinStack";

    private const int    MAX_EDIT_DISTANCE = 5;
    private const double EXACT_BONUS       = 0.5;
    private const int    MAX_SCORED_RESULTS = 500;

    private readonly TrigramIndex         _index;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache         _cache;

    public LevenshteinStackSearchEngine(
        TrigramIndex index,
        IServiceScopeFactory scopeFactory,
        IMemoryCache cache)
    {
        _index        = index;
        _scopeFactory = scopeFactory;
        _cache        = cache;
    }

    public async Task<SearchEngineResult> SearchAsync(
        SearchEngineQuery query,
        CancellationToken ct = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        if (!_index.IsReady)
        {
            return new SearchEngineResult
            {
                Hits                = [],
                VariantName         = VariantName,
                ElapsedMs           = 0,
                CandidatesEvaluated = 0
            };
        }

        var normalised = (query.Term ?? string.Empty).Trim().ToLowerInvariant();
        var cacheKey   = $"{normalised}:{query.EntityFilter}";

        if (!_cache.TryGetValue(cacheKey, out List<Guid>? snapshot))
        {
            // Phase 1+2: adaptive trigram filter, length prefilter, top-K min-heap.
            // See Variant 2.1 for the full rationale.
            var candidates = _index.GetFilteredCandidates(normalised, query.EntityFilter);
            int qLen = normalised.Length;
            var heap = new PriorityQueue<Guid, double>(MAX_SCORED_RESULTS);

            foreach (var (id, nameLower) in candidates)
            {
                if (Math.Abs(nameLower.Length - qLen) > MAX_EDIT_DISTANCE) continue;

                int dist = LevenshteinStackCalculator.Compute(
                    normalised.AsSpan(), nameLower.AsSpan());

                if (dist > MAX_EDIT_DISTANCE) continue;

                double score = 1.0 / (1.0 + dist);
                if (dist == 0) score += EXACT_BONUS;

                if (heap.Count < MAX_SCORED_RESULTS) heap.Enqueue(id, score);
                else                                  heap.EnqueueDequeue(id, score);
            }

            // Deterministic tie-break: see Variant 2.1 for rationale.
            var ordered = new (Guid Id, double Score)[heap.Count];
            for (int i = ordered.Length - 1; i >= 0; i--)
            {
                heap.TryDequeue(out var oid, out var osc);
                ordered[i] = (oid, osc);
            }
            Array.Sort(ordered, (a, b) =>
            {
                int c = b.Score.CompareTo(a.Score);
                return c != 0 ? c : b.Id.CompareTo(a.Id);
            });
            snapshot = new List<Guid>(ordered.Length);
            foreach (var item in ordered) snapshot.Add(item.Id);

            _cache.Set(cacheKey, snapshot, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(60),
                Size = 1
            });
        }

        int totalScoredCount = snapshot!.Count;
        var pageIds = snapshot
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToList();

        var scoreMap = pageIds
            .Select((id, i) => (id, score: 1.0 / (1.0 + i)))
            .ToDictionary(x => x.id, x => x.score);

        var hits = await FetchHitsFromDb(pageIds, scoreMap, ct);

        sw.Stop();

        return new SearchEngineResult
        {
            Hits                = hits,
            HasMore             = (query.Skip + query.PageSize) < totalScoredCount,
            TotalScoredCount    = totalScoredCount,
            VariantName         = VariantName,
            ElapsedMs           = sw.Elapsed.TotalMilliseconds,
            CandidatesEvaluated = totalScoredCount
        };
    }

    private Dictionary<Guid, int> GetCandidates(string queryTerm, SearchEntityType filter)
    {
        // Kept for any future external use; the hot path now uses
        // _index.GetFilteredCandidates directly inside SearchAsync.
        var dict = new Dictionary<Guid, int>();
        foreach (var (id, _) in _index.GetFilteredCandidates(queryTerm, filter))
            dict[id] = 1;
        return dict;
    }

    private async Task<List<SearchHit>> FetchHitsFromDb(
        List<Guid> pageIds,
        Dictionary<Guid, double> scoreMap,
        CancellationToken ct)
    {
        if (pageIds.Count == 0) return [];

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var songIds   = new List<Guid>();
        var artistIds = new List<Guid>();
        var albumIds  = new List<Guid>();

        foreach (var id in pageIds)
        {
            if (_index.TryGetEntry(id, out var entry))
            {
                switch (entry.EntityType)
                {
                    case SearchEntityType.Song:   songIds.Add(id);   break;
                    case SearchEntityType.Artist: artistIds.Add(id); break;
                    case SearchEntityType.Album:  albumIds.Add(id);  break;
                }
            }
        }

        var results = new List<SearchHit>(pageIds.Count);

        if (songIds.Count > 0)
        {
            var songs = await db.Songs.AsNoTracking()
                .Where(s => songIds.Contains(s.Id))
                .Include(s => s.Album)
                .ToListAsync(ct);

            results.AddRange(songs.Select(s => new SearchHit
            {
                Id            = s.Id,
                Name          = s.Title,
                EntityType    = "Song",
                Score         = scoreMap.TryGetValue(s.Id, out var sc) ? sc : 0,
                ImageLocation = s.Album?.ImageLocation,
                ParentId      = s.AlbumId
            }));
        }

        if (artistIds.Count > 0)
        {
            var artists = await db.Artists.AsNoTracking()
                .Where(a => artistIds.Contains(a.Id))
                .ToListAsync(ct);

            results.AddRange(artists.Select(a => new SearchHit
            {
                Id            = a.Id,
                Name          = a.Name,
                EntityType    = "Artist",
                Score         = scoreMap.TryGetValue(a.Id, out var sc) ? sc : 0,
                ImageLocation = a.ImageLocation
            }));
        }

        if (albumIds.Count > 0)
        {
            var albums = await db.Albums.AsNoTracking()
                .Where(a => albumIds.Contains(a.Id))
                .ToListAsync(ct);

            results.AddRange(albums.Select(a => new SearchHit
            {
                Id            = a.Id,
                Name          = a.Name,
                EntityType    = "Album",
                Score         = scoreMap.TryGetValue(a.Id, out var sc) ? sc : 0,
                ImageLocation = a.ImageLocation
            }));
        }

        return results.OrderByDescending(h => h.Score).ToList();
    }
}
