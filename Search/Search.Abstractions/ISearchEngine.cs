namespace Search.Abstractions;

/// <summary>
/// Core contract that every search variant implements.
/// Register as a singleton. Inject into SearchEngineController.
/// </summary>
public interface ISearchEngine
{
    /// <summary>
    /// Human-readable variant identifier used in response metadata and benchmarks.
    /// Example: "Variant2_2_LevenshteinStack"
    /// </summary>
    string VariantName { get; }

    /// <summary>
    /// Execute a search and return ranked hits.
    /// Must be thread-safe (called concurrently under load).
    /// </summary>
    Task<SearchEngineResult> SearchAsync(SearchEngineQuery query, CancellationToken ct = default);
}
