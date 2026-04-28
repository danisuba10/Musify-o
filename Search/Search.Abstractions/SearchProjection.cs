namespace Search.Abstractions;

/// <summary>
/// Lightweight projection of a searchable entity stored in the in-memory trigram index.
/// A struct to minimise GC pressure during index build and mutation.
/// </summary>
public struct SearchProjection
{
    public Guid             Id;
    public string           Name;
    public SearchEntityType EntityType;
}
