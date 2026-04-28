using MediatR;

namespace Search.Abstractions;

/// <summary>
/// Published by MediatR command handlers after a searchable entity is created.
/// Handled by each variant's SearchIndexMutationHandler to keep the in-memory index in sync.
/// </summary>
public record SearchEntityCreatedNotification(
    Guid Id, string Name, SearchEntityType EntityType) : INotification;

/// <summary>
/// Published after a searchable entity is deleted.
/// IMPORTANT: Name must be captured BEFORE SaveChangesAsync — the row no longer exists after deletion.
/// </summary>
public record SearchEntityDeletedNotification(
    Guid Id, string Name, SearchEntityType EntityType) : INotification;

/// <summary>
/// Published after a searchable entity's name is changed.
/// OldName is required to remove the old trigrams from the index.
/// </summary>
public record SearchEntityUpdatedNotification(
    Guid Id, string OldName, string NewName, SearchEntityType EntityType) : INotification;
