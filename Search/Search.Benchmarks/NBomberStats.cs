// Search.Benchmarks/NBomberStats.cs
namespace Search.Benchmarks;

internal record NBomberStats
{
    public required string VariantName      { get; init; }
    public required int    ConcurrentUsers  { get; init; }
    public required double MinMs            { get; init; }
    public required double MeanMs           { get; init; }
    public required double StdDevMs         { get; init; }
    public required double MaxMs            { get; init; }
    public required double P50Ms            { get; init; }
    public required double P75Ms            { get; init; }
    public required double P95Ms            { get; init; }
    public required double P99Ms            { get; init; }
    public required double RequestsPerSec   { get; init; }
    public required long   FailedRequests   { get; init; }
    public          double IndexRamMb       { get; init; }
    public          double BuildTimeS       { get; init; }
}
