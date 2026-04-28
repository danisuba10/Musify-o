using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using Search.Abstractions;

namespace Search.Variant2_1.LevenshteinBasic;

internal sealed class IndexBuilderService : BackgroundService
{
    private readonly IServiceScopeFactory        _scopeFactory;
    private readonly TrigramIndex                _index;
    private readonly ILogger<IndexBuilderService> _logger;

    public IndexBuilderService(
        IServiceScopeFactory scopeFactory,
        TrigramIndex index,
        ILogger<IndexBuilderService> logger)
    {
        _scopeFactory = scopeFactory;
        _index        = index;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Variant2_1] Building trigram index...");

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var songs = await db.Songs
            .AsNoTracking()
            .Select(s => new SearchProjection { Id = s.Id, Name = s.Title, EntityType = SearchEntityType.Song })
            .ToListAsync(stoppingToken);

        var artists = await db.Artists
            .AsNoTracking()
            .Select(a => new SearchProjection { Id = a.Id, Name = a.Name, EntityType = SearchEntityType.Artist })
            .ToListAsync(stoppingToken);

        var albums = await db.Albums
            .AsNoTracking()
            .Select(a => new SearchProjection { Id = a.Id, Name = a.Name, EntityType = SearchEntityType.Album })
            .ToListAsync(stoppingToken);

        _index.Build(songs.Concat(artists).Concat(albums));

        _logger.LogInformation(
            "[Variant2_1] Index ready. Songs={S}, Artists={A}, Albums={Al}, Total={T}",
            songs.Count, artists.Count, albums.Count,
            songs.Count + artists.Count + albums.Count);
    }
}
