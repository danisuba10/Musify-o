// Search.Benchmarks/OomGuard.cs
namespace Search.Benchmarks;

internal static class OomGuard
{
    /// <summary>
    /// Executes <paramref name="action"/>, catching <see cref="OutOfMemoryException"/>.
    /// On OOM, logs a SKIPPED_OOM row to <paramref name="writer"/> and forces GC.
    /// Returns <c>null</c> on OOM so the caller can skip that adapter in the matrix.
    /// </summary>
    public static T? TryRun<T>(Func<T> action, string label, ResultWriter writer)
        where T : class
    {
        try
        {
            return action();
        }
        catch (OutOfMemoryException)
        {
            Console.WriteLine($"[OOM] {label} — skipping, logging SKIPPED_OOM");
            writer.AppendOom(label);
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);
            return null;
        }
    }
}
