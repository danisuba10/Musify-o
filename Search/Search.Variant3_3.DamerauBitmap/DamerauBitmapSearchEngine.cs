using Equativ.RoaringBitmaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Search.Abstractions;

namespace Search.Variant3_3.DamerauBitmap;

internal sealed class DamerauBitmapSearchEngine : ISearchEngine
{
    public string VariantName => "Variant3_3_DamerauBitmap";

    private const int    MAX_EDIT_DISTANCE  = 5;
    private const double EXACT_BONUS        = 0.5;
    private const int    MAX_SCORED_RESULTS = 500;

    private readonly RoaringBitmapTrigramIndex _index;
    private readonly IServiceScopeFactory      _scopeFactory;
    private readonly IMemoryCache              _cache;

    public DamerauBitmapSearchEngine(
        RoaringBitmapTrigramIndex index,
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

        if (!_cache.TryGetValue(cacheKey, out List<(Guid Id, double Score)>? snapshot))
        {
            snapshot = CalculateRankedSnapshot(normalised, query.EntityFilter);
            _cache.Set(cacheKey, snapshot, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(60),
                Size = 1
            });
        }

        int totalScoredCount = snapshot!.Count;

        var page = snapshot.Skip(query.Skip).Take(query.PageSize).ToList();
        var pageIds = page.Select(x => x.Id).ToList();
        var scoreMap = page.ToDictionary(x => x.Id, x => x.Score);

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

    // Isolate the Span logic in a synchronous method to satisfy C# 12
    private List<(Guid Id, double Score)> CalculateRankedSnapshot(string normalised, SearchEntityType entityFilter)
    {
        // Use the optimised filter+score path: pooled byte[] candidate counter,
        // pre-lowered names, single read-lock acquisition. Same hot path as
    // the benchmark adapter so production matches the measured numbers.
        var candidates = _index.GetFilteredCandidates(normalised, entityFilter);
        bool multiWord = normalised.Contains(' ');
        int  maxDist   = multiWord ? MAX_EDIT_DISTANCE : Math.Min(3, normalised.Length / 2);
        int  qLen      = normalised.Length;

        var heap = new PriorityQueue<Guid, double>(MAX_SCORED_RESULTS);

        foreach (var (id, nameLower) in candidates)
        {
            int dist;
            if (multiWord)
            {
                if (Math.Abs(nameLower.Length - qLen) > maxDist) continue;
                dist = DamerauStackCalculator.Compute(
                    normalised.AsSpan(), nameLower.AsSpan(), maxDist);
            }
            else
            {
                dist = int.MaxValue;
                ReadOnlySpan<char> nameSpan = nameLower.AsSpan();

                while(!nameSpan.IsEmpty)
                {
                    int spaceIdx = nameSpan.IndexOf(' ');
                    ReadOnlySpan<char> token = spaceIdx == -1 ? nameSpan : nameSpan.Slice(0, spaceIdx);

                    if (Math.Abs(token.Length - qLen) <= maxDist)
                    {
                        int d = DamerauStackCalculator.Compute(normalised.AsSpan(), token, maxDist);
                        if (d < dist) { dist = d; if (dist == 0) break; }
                    }

                    if (spaceIdx == -1) break;
                    nameSpan = nameSpan.Slice(spaceIdx + 1);
                }

                if (dist == int.MaxValue) continue;
            }

            if (dist > maxDist) continue;

            double score = 1.0 / (1.0 + dist) + (dist == 0 ? EXACT_BONUS : 0);
            if (heap.Count < MAX_SCORED_RESULTS) heap.Enqueue(id, score);
            else                                 heap.EnqueueDequeue(id, score);
        }

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

        var snapshot = new List<(Guid Id, double Score)>(ordered.Length);
        foreach (var item in ordered) snapshot.Add(item);

        return snapshot;
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
