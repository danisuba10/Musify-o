namespace Search.Abstractions;

public sealed class SearchHit
{
    /// <summary>Primary key in the database.</summary>
    public Guid Id { get; init; }

    /// <summary>Display name (Song.Title / Artist.Name / Album.Name).</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>"Song" | "Artist" | "Album"</summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    /// Relevance score in [0.0, 1.0].
    /// Variant 1: always 1.0 (SQL result, no distance score).
    /// Variants 2.x / 3.x: 1.0 / (1.0 + editDistance).
    /// </summary>
    public double Score { get; init; }

    /// <summary>Optional: image URL/path forwarded from DB fetch.</summary>
    public string? ImageLocation { get; init; }

    /// <summary>Optional: parent entity ID (e.g. AlbumId for a Song).</summary>
    public Guid? ParentId { get; init; }
}
