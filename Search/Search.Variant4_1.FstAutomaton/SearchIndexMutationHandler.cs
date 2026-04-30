using MediatR;
using Search.Abstractions;

namespace Search.Variant4_1.FstAutomaton;

internal sealed class SearchIndexMutationHandler :
    INotificationHandler<SearchEntityCreatedNotification>,
    INotificationHandler<SearchEntityDeletedNotification>,
    INotificationHandler<SearchEntityUpdatedNotification>
{
    private readonly FstTermIndex _index;

    public SearchIndexMutationHandler(FstTermIndex index) => _index = index;

    public Task Handle(SearchEntityCreatedNotification n, CancellationToken ct)
    {
        _index.Add(new SearchProjection { Id = n.Id, Name = n.Name, EntityType = n.EntityType });
        return Task.CompletedTask;
    }

    public Task Handle(SearchEntityDeletedNotification n, CancellationToken ct)
    {
        _index.Remove(n.Id, n.Name);
        return Task.CompletedTask;
    }

    public Task Handle(SearchEntityUpdatedNotification n, CancellationToken ct)
    {
        _index.Update(n.Id, n.OldName,
            new SearchProjection { Id = n.Id, Name = n.NewName, EntityType = n.EntityType });
        return Task.CompletedTask;
    }
}
