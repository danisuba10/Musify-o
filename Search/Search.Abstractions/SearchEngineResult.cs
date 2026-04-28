namespace Search.Abstractions;

public sealed class SearchEngineResult
{
    public IReadOnlyList<SearchHit> Hits { get; init; } = [];

    /// <summary>
    /// True if more results exist beyond the current page.
    /// Variant 1: set to true when DB returned PageSize rows (may be more).
    /// Variants 2.x/3.x: set to true when (Skip + PageSize) &lt; TotalScoredCount.
    /// </summary>
    public bool HasMore { get; init; }

    /// <summary>
    /// Total number of in-memory candidates that passed distance scoring (before Skip/Take).
    /// Variants 2.x/3.x only. Variant 1: 0 (not applicable).
    /// </summary>
    public int TotalScoredCount { get; init; }

    // ── Variant 1 cursor fields ─────────────────────────────────────────────
    /// <summary>Pass back to next request as LastName for cursor pagination.</summary>
    public string?   NextLastName      { get; init; }
    public DateTime? NextLastCreatedAt { get; init; }

    // ── Diagnostic / thesis metrics ─────────────────────────────────────────
    /// <summary>Human-readable name of the active variant.</summary>
    public string VariantName { get; init; } = string.Empty;

    /// <summary>Time (ms) spent inside SearchAsync, measured inside the engine itself.</summary>
    public double ElapsedMs { get; init; }

    /// <summary>
    /// Number of candidates evaluated before final ranking.
    /// Variant 1 = N/A (0). In-memory variants = strings that passed the trigram filter.
    /// </summary>
    public int CandidatesEvaluated { get; init; }
}
