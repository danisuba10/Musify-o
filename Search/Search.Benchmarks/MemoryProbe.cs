// Search.Benchmarks/MemoryProbe.cs
namespace Search.Benchmarks;

internal static class MemoryProbe
{
    /// <summary>
    /// Measures managed heap delta caused by <paramref name="action"/>.
    /// Forces two GC passes before and after to eliminate unrelated allocations.
    /// Returns approximate bytes held by the index after build.
    /// </summary>
    public static long MeasureBytes(Action action)
    {
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);

        long before = GC.GetTotalMemory(false);
        action();

        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        long after = GC.GetTotalMemory(false);

        return after - before;
    }
}
