using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Search.Abstractions;

namespace Search.Variant3_2.DamerauStack;

public static class DamerauStackExtensions
{
    public static IServiceCollection AddVariant3_2_Search(this IServiceCollection services)
    {
        services.AddMemoryCache(opts => opts.SizeLimit = 10_000);
        services.AddSingleton<TrigramIndex>();
        services.AddSingleton<ISearchEngine, DamerauStackSearchEngine>();
        services.AddHostedService<IndexBuilderService>();

        services.AddTransient<INotificationHandler<SearchEntityCreatedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityDeletedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityUpdatedNotification>, SearchIndexMutationHandler>();

        return services;
    }
}
