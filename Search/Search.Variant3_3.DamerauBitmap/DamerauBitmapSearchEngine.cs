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

        if (!_cache.TryGetValue(cacheKey, out List<Guid>? snapshot))
        {
            var candidateBitmap = _index.GetCandidateBitmap(normalised);
            var scored          = new List<(Guid Id, double Score)>();

            if (candidateBitmap != null)
            {
                foreach (int idx in candidateBitmap)
                {
                    if (_index.IsTombstoned(idx)) continue;

                    var entry = _index.GetEntry(idx);

                    if (query.EntityFilter != SearchEntityType.All
                        && entry.EntityType != query.EntityFilter)
                        continue;

                    int dist = DamerauStackCalculator.Compute(
                        normalised.AsSpan(),
                        entry.Name.ToLowerInvariant().AsSpan());

                    if (dist > MAX_EDIT_DISTANCE) continue;

                    double score = 1.0 / (1.0 + dist);
                    if (dist == 0) score += EXACT_BONUS;

                    scored.Add((entry.Id, score));
                }
            }

            snapshot = scored
                .OrderByDescending(x => x.Score)
                .Take(MAX_SCORED_RESULTS)
                .Select(x => x.Id)
                .ToList();

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
