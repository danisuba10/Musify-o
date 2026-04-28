using Microsoft.Extensions.DependencyInjection;
using Search.Abstractions;

namespace Search.Variant1.Pagination;

public static class PaginationSearchEngineExtensions
{
    /// <summary>
    /// Registers Variant 1 (cursor-paginated SQL LIKE search) as the active ISearchEngine.
    /// No IHostedService needed — all work happens per-request via EF Core.
    /// </summary>
    public static IServiceCollection AddVariant1_Search(this IServiceCollection services)
    {
        services.AddSingleton<ISearchEngine, PaginationSearchEngine>();
        return services;
    }
}
