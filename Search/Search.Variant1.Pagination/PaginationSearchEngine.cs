using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Search.Abstractions;

namespace Search.Variant1.Pagination;

public sealed class PaginationSearchEngine : ISearchEngine
{
    public string VariantName => "Variant1_CursorPagination";

    private readonly IServiceScopeFactory _scopeFactory;

    public PaginationSearchEngine(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    public async Task<SearchEngineResult> SearchAsync(SearchEngineQuery query, CancellationToken ct = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var term     = (query.Term ?? string.Empty).ToLower();
        var lastName = query.LastName?.ToLower();
        var lastAt   = query.LastCreatedAt;
        var pageSize = query.PageSize;
        var filter   = query.EntityFilter;

        var hits = new List<SearchHit>();

        // ── Songs ──────────────────────────────────────────────────────────────
        if (filter is SearchEntityType.All or SearchEntityType.Song)
        {
            var songQ = db.Songs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
                songQ = songQ.Where(s => s.Title.ToLower().Contains(term));

            if (!string.IsNullOrWhiteSpace(lastName) && lastAt.HasValue)
                songQ = songQ.Where(s =>
                    string.Compare(s.Title.ToLower(), lastName) > 0 ||
                    (string.Compare(s.Title.ToLower(), lastName) == 0 && s.CreatedAt > lastAt.Value));

            var songs = await songQ
                .OrderBy(s => s.Title)
                .ThenBy(s => s.CreatedAt)
                .Include(s => s.Album)
                .Take(pageSize)
                .ToListAsync(ct);

            hits.AddRange(songs.Select(s => new SearchHit
            {
                Id            = s.Id,
                Name          = s.Title,
                EntityType    = "Song",
                Score         = 1.0,
                ImageLocation = s.Album?.ImageLocation,
                ParentId      = s.AlbumId
            }));
        }

        // ── Artists ────────────────────────────────────────────────────────────
        if (filter is SearchEntityType.All or SearchEntityType.Artist)
        {
            var artQ = db.Artists.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
                artQ = artQ.Where(a => a.Name.ToLower().Contains(term));

            if (!string.IsNullOrWhiteSpace(lastName) && lastAt.HasValue)
                artQ = artQ.Where(a =>
                    string.Compare(a.Name.ToLower(), lastName) > 0 ||
                    (string.Compare(a.Name.ToLower(), lastName) == 0 && a.CreatedAt > lastAt.Value));

            var artists = await artQ
                .OrderBy(a => a.Name)
                .ThenBy(a => a.CreatedAt)
                .Take(pageSize)
                .ToListAsync(ct);

            hits.AddRange(artists.Select(a => new SearchHit
            {
                Id            = a.Id,
                Name          = a.Name,
                EntityType    = "Artist",
                Score         = 1.0,
                ImageLocation = a.ImageLocation
            }));
        }

        // ── Albums ─────────────────────────────────────────────────────────────
        if (filter is SearchEntityType.All or SearchEntityType.Album)
        {
            var albQ = db.Albums.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
                albQ = albQ.Where(a => a.Name.ToLower().Contains(term));

            if (!string.IsNullOrWhiteSpace(lastName) && lastAt.HasValue)
                albQ = albQ.Where(a =>
                    string.Compare(a.Name.ToLower(), lastName) > 0 ||
                    (string.Compare(a.Name.ToLower(), lastName) == 0 && a.CreatedAt > lastAt.Value));

            var albums = await albQ
                .OrderBy(a => a.Name)
                .ThenBy(a => a.CreatedAt)
                .Take(pageSize)
                .ToListAsync(ct);

            hits.AddRange(albums.Select(a => new SearchHit
            {
                Id            = a.Id,
                Name          = a.Name,
                EntityType    = "Album",
                Score         = 1.0,
                ImageLocation = a.ImageLocation
            }));
        }

        // ── Build cursor from last hit ─────────────────────────────────────────
        // Cursor is only meaningful when filtering to a single entity type.
        // When EntityFilter = All, the cursor concept does not apply cleanly.
        var lastHit = hits.LastOrDefault();

        sw.Stop();

        return new SearchEngineResult
        {
            Hits                = hits,
            HasMore             = hits.Count >= pageSize,
            NextLastName        = lastHit?.Name,
            NextLastCreatedAt   = null,
            VariantName         = VariantName,
            ElapsedMs           = sw.Elapsed.TotalMilliseconds,
            CandidatesEvaluated = 0
        };
    }
}
