// Search.Benchmarks/LoadTestRunner.cs
using NBomber.Contracts;
using NBomber.CSharp;

namespace Search.Benchmarks;

internal static class LoadTestRunner
{
    public static NBomberStats Run(
        SearchOnlyAdapter adapter,
        int concurrentUsers,
        TimeSpan duration)
    {
        var scenario = Scenario.Create($"search_{adapter.VariantName}_{concurrentUsers}u", async context =>
        {
            var term = QueryPool.GetRandom();
            // ScoreOnly is CPU-bound. Wrap in Task.Run so NBomber's async
            // scheduler does not block its own thread pool.
            var result = await Task.Run(() => adapter.ScoreOnly(term));
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
            .WithReportFolder(string.Empty)   // suppress report file generation
            .Run();

        var s = nodeStats.ScenarioStats[0];

        return new NBomberStats
        {
            VariantName     = adapter.VariantName,
            ConcurrentUsers = concurrentUsers,
            MeanMs          = s.Ok.Latency.MeanMs,
            P50Ms           = s.Ok.Latency.Percent50,
            P95Ms           = s.Ok.Latency.Percent95,
            P99Ms           = s.Ok.Latency.Percent99,
            RequestsPerSec  = s.Ok.Request.RPS,
            FailedRequests  = s.Fail.Request.Count,
            IndexRamMb      = adapter.IndexRamMb,
            BuildTimeS      = adapter.BuildTimeS,
        };
    }
}
