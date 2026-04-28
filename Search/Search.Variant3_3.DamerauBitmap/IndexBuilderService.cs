using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using Search.Abstractions;

namespace Search.Variant3_3.DamerauBitmap;

internal sealed class IndexBuilderService : BackgroundService
{
    private readonly IServiceScopeFactory              _scopeFactory;
    private readonly RoaringBitmapTrigramIndex         _index;
    private readonly ILogger<IndexBuilderService>      _logger;

    public IndexBuilderService(
        IServiceScopeFactory scopeFactory,
        RoaringBitmapTrigramIndex index,
        ILogger<IndexBuilderService> logger)
    {
        _scopeFactory = scopeFactory;
        _index        = index;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Variant3_3] Building roaring-bitmap trigram index...");
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var projections = new List<SearchProjection>();

        var songs = await db.Songs.AsNoTracking()
            .Select(s => new { s.Id, s.Title })
            .ToListAsync(stoppingToken);
        projections.AddRange(songs.Select(s => new SearchProjection
            { Id = s.Id, Name = s.Title, EntityType = SearchEntityType.Song }));

        var artists = await db.Artists.AsNoTracking()
            .Select(a => new { a.Id, a.Name })
            .ToListAsync(stoppingToken);
        projections.AddRange(artists.Select(a => new SearchProjection
            { Id = a.Id, Name = a.Name, EntityType = SearchEntityType.Artist }));

        var albums = await db.Albums.AsNoTracking()
            .Select(a => new { a.Id, a.Name })
            .ToListAsync(stoppingToken);
        projections.AddRange(albums.Select(a => new SearchProjection
            { Id = a.Id, Name = a.Name, EntityType = SearchEntityType.Album }));

        _index.Build(projections);
        _logger.LogInformation("[Variant3_3] Bitmap trigram index ready — {Count} entries.", projections.Count);
    }
}
