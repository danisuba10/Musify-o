using System.Buffers;

namespace Search.Variant2_2.LevenshteinStack;

/// <summary>
/// Computes Levenshtein edit distance using the 1D sliding-window algorithm.
/// For strings ≤ 256 chars: allocates working arrays on the CPU stack (stackalloc).
///   → Zero heap allocation. Zero GC pressure.
/// For strings > 256 chars: rents arrays from ArrayPool (reused heap buffer).
///   → Zero GC allocation (pool is pre-allocated).
///
/// Implementation basis: Navarro (2001), Section 3.1.
/// Space complexity: O(n) — only two rows of the DP matrix are kept.
/// Time complexity:  O(nm) — unchanged.
/// </summary>
internal static class LevenshteinStackCalculator
{
    private const int MAX_STACKALLOC_LENGTH = 256;

    internal static int Compute(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        int m = source.Length;
        int n = target.Length;

        if (m == 0) return n;
        if (n == 0) return m;
        if (source.SequenceEqual(target)) return 0;

        if (m > n)
        {
            var tmp = source; source = target; target = tmp;
            int t = m; m = n; n = t;
        }

        if (n <= MAX_STACKALLOC_LENGTH)
        {
            Span<int> prev = stackalloc int[n + 1];
            Span<int> curr = stackalloc int[n + 1];
            return ComputeCore(source, target, m, n, prev, curr);
        }
        else
        {
            int[] prevArr = ArrayPool<int>.Shared.Rent(n + 1);
            int[] currArr = ArrayPool<int>.Shared.Rent(n + 1);
            try
            {
                return ComputeCore(source, target, m, n,
                    prevArr.AsSpan(0, n + 1),
                    currArr.AsSpan(0, n + 1));
            }
            finally
            {
                ArrayPool<int>.Shared.Return(prevArr);
                ArrayPool<int>.Shared.Return(currArr);
            }
        }
    }

    private static int ComputeCore(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target,
        int m, int n,
        Span<int> prev,
        Span<int> curr)
    {
        for (int j = 0; j <= n; j++) prev[j] = j;

        for (int i = 1; i <= m; i++)
        {
            curr[0] = i;

            for (int j = 1; j <= n; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;
                curr[j] = Math.Min(
                    Math.Min(curr[j - 1] + 1,
                             prev[j]     + 1),
                             prev[j - 1] + cost
                );
            }

            var swapTmp = prev;
            prev = curr;
            curr = swapTmp;
        }

        return prev[n];
    }
}
