namespace Search.Abstractions;

/// <summary>
/// Unified query sent to any ISearchEngine variant.
///
/// Pagination strategy differs per variant family:
///   Variant 1  — keyset cursor (LastName + LastCreatedAt). SQL-efficient; no OFFSET.
///   Variants 2.x / 3.x — offset pagination (Skip + PageSize). Safe because all
///     scoring runs in memory; there is no repeated DB table scan per page.
/// </summary>
public sealed class SearchEngineQuery
{
    /// <summary>Raw text typed by the user.</summary>
    public string Term { get; init; } = string.Empty;

    /// <summary>Filter to a specific entity type, or All.</summary>
    public SearchEntityType EntityFilter { get; init; } = SearchEntityType.All;

    /// <summary>Maximum number of hits to return in a single page.</summary>
    public int PageSize { get; init; } = 20;

    // ── Offset pagination (Variants 2.x / 3.x) ──────────────────────────
    /// <summary>
    /// Zero-based offset into the scored result list.
    /// Page 1 = Skip 0, Page 2 = Skip PageSize, Page 3 = Skip 2*PageSize, …
    /// </summary>
    public int Skip { get; init; } = 0;

    // ── Keyset cursor pagination (Variant 1 only) ────────────────────────
    /// <summary>Name of the last item on the previous page. Pass null for first page.</summary>
    public string? LastName { get; init; }

    /// <summary>CreatedAt of the last item on the previous page. Pass null for first page.</summary>
    public DateTime? LastCreatedAt { get; init; }
}
