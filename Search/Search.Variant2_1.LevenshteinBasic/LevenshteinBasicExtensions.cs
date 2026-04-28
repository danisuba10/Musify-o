using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Search.Abstractions;

namespace Search.Variant2_1.LevenshteinBasic;

public static class LevenshteinBasicExtensions
{
    /// <summary>
    /// Registers Variant 2.1 (Levenshtein, heap-allocated) as the active ISearchEngine.
    /// </summary>
    public static IServiceCollection AddVariant2_1_Search(this IServiceCollection services)
    {
        services.AddMemoryCache(opts => opts.SizeLimit = 10_000);
        services.AddSingleton<TrigramIndex>();
        services.AddSingleton<ISearchEngine, LevenshteinBasicSearchEngine>();
        services.AddHostedService<IndexBuilderService>();

        services.AddTransient<INotificationHandler<SearchEntityCreatedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityDeletedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityUpdatedNotification>, SearchIndexMutationHandler>();

        return services;
    }
}
