using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Search.Abstractions;

namespace Search.Variant3_3.DamerauBitmap;

public static class DamerauBitmapExtensions
{
    public static IServiceCollection AddVariant3_3_Search(this IServiceCollection services)
    {
        services.AddMemoryCache(opts => opts.SizeLimit = 10_000);
        services.AddSingleton<RoaringBitmapTrigramIndex>();
        services.AddSingleton<ISearchEngine, DamerauBitmapSearchEngine>();
        services.AddHostedService<IndexBuilderService>();

        services.AddTransient<INotificationHandler<SearchEntityCreatedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityDeletedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityUpdatedNotification>, SearchIndexMutationHandler>();

        return services;
    }
}
