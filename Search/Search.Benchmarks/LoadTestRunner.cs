// Search.Benchmarks/LoadTestRunner.cs
using NBomber.Contracts;
using NBomber.CSharp;

namespace Search.Benchmarks;

internal static class LoadTestRunner
{
    public static NBomberStats Run(
        SearchOnlyAdapter adapter,
        long entityCount,
        int concurrentUsers,
        TimeSpan duration)
    {
        var scenario = Scenario.Create($"search_{adapter.VariantName}_{entityCount:N0}e_{concurrentUsers}u", async context =>
        {
            var term = QueryPool.GetRandom();
            // ScoreOnly is CPU-bound. Wrapping it in Task.Run was previously used to
            // avoid blocking NBomber's async scheduler, but at high concurrency this
            // doubled the thread-pool pressure (one TP slot for the NBomber awaiter
            // plus one for the work item) and amplified tail latencies. NBomber's
            // scheduler is fine with synchronous CPU work — keeping the call inline
            // halves the TP demand and lets the per-virtual-user back-pressure cap
            // queue depth naturally.
            await Task.Yield();
            var result = adapter.ScoreOnly(term);
            return result.Count >= 0 ? Response.Ok() : Response.Fail();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            // KeepConstant: maintains exactly N concurrent virtual users
            // for the full duration. Each user fires requests back-to-back.
            Simulation.KeepConstant(copies: concurrentUsers, during: duration)
        );

        var nodeStats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFormats()   // empty = suppress all report file generation
            .Run();

        var s = nodeStats.ScenarioStats[0];

        return new NBomberStats
        {
            VariantName     = adapter.VariantName,
            ConcurrentUsers = concurrentUsers,
            MinMs           = s.Ok.Latency.MinMs,
            MeanMs          = s.Ok.Latency.MeanMs,
            StdDevMs        = s.Ok.Latency.StdDev,
            MaxMs           = s.Ok.Latency.MaxMs,
            P50Ms           = s.Ok.Latency.Percent50,
            P75Ms           = s.Ok.Latency.Percent75,
            P95Ms           = s.Ok.Latency.Percent95,
            P99Ms           = s.Ok.Latency.Percent99,
            RequestsPerSec  = s.Ok.Request.RPS,
            FailedRequests  = s.Fail.Request.Count,
            IndexRamMb      = adapter.IndexRamMb,
            BuildTimeS      = adapter.BuildTimeS,
        };
    }
}
