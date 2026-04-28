using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Search.Abstractions;

namespace Search.Variant3_1.DamerauBasic;

public static class DamerauBasicExtensions
{
    public static IServiceCollection AddVariant3_1_Search(this IServiceCollection services)
    {
        services.AddMemoryCache(opts => opts.SizeLimit = 10_000);
        services.AddSingleton<TrigramIndex>();
        services.AddSingleton<ISearchEngine, DamerauBasicSearchEngine>();
        services.AddHostedService<IndexBuilderService>();

        services.AddTransient<INotificationHandler<SearchEntityCreatedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityDeletedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityUpdatedNotification>, SearchIndexMutationHandler>();

        return services;
    }
}
