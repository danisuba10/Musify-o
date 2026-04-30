using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Search.Abstractions;

namespace Search.Variant4_1.FstAutomaton;

public static class FstAutomatonExtensions
{
    public static IServiceCollection AddVariant4_Search(this IServiceCollection services)
    {
        services.AddMemoryCache(opts => opts.SizeLimit = 10_000);
        services.AddSingleton<FstTermIndex>();
        services.AddSingleton<ISearchEngine, FstAutomatonSearchEngine>();
        services.AddHostedService<IndexBuilderService>();

        services.AddTransient<INotificationHandler<SearchEntityCreatedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityDeletedNotification>, SearchIndexMutationHandler>();
        services.AddTransient<INotificationHandler<SearchEntityUpdatedNotification>, SearchIndexMutationHandler>();

        return services;
    }
}
