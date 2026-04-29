using System.Buffers;

namespace Search.Variant3_3.DamerauBitmap;

/// <summary>
/// Computes OSA (restricted Damerau-Levenshtein) distance.
/// Identical algorithm to Variant 3.2 — single flat stackalloc/ArrayPool buffer.
/// </summary>
internal static class DamerauStackCalculator
{
    private const int MAX_STACKALLOC_LENGTH = 256;

    internal static int Compute(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
        => Compute(source, target, int.MaxValue);

    /// <summary>
    /// Ukkonen-style early-exit overload. After each row is filled, if the
    /// row minimum exceeds <paramref name="maxDistance"/> the routine returns
    /// <c>maxDistance + 1</c> without finishing the matrix. This is the
    /// dominant per-candidate cost in the search hot path: most candidates
    /// passing the trigram filter are still rejected by edit distance, so
    /// bailing as early as possible is critical.
    /// </summary>
    internal static int Compute(ReadOnlySpan<char> source, ReadOnlySpan<char> target, int maxDistance)
    {
        int m = source.Length;
        int n = target.Length;

        if (m == 0) return n;
        if (n == 0) return m;
        if (source.SequenceEqual(target)) return 0;

        // Lower bound on edit distance is the length difference.
        if (Math.Abs(m - n) > maxDistance) return maxDistance + 1;

        if (m > n)
        {
            var tmp = source; source = target; target = tmp;
            int t = m; m = n; n = t;
        }

        int rowLen = n + 1;

        if (n <= MAX_STACKALLOC_LENGTH)
        {
            Span<int> buffer = stackalloc int[3 * rowLen];
            return ComputeCore(source, target, m, n, rowLen, buffer, maxDistance);
        }
        else
        {
            int[] rented = ArrayPool<int>.Shared.Rent(3 * rowLen);
            try
            {
                return ComputeCore(source, target, m, n, rowLen, rented.AsSpan(0, 3 * rowLen), maxDistance);
            }
            finally
            {
                ArrayPool<int>.Shared.Return(rented);
            }
        }
    }

    private static int ComputeCore(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target,
        int m, int n, int rowLen,
        Span<int> buffer,
        int maxDistance)
    {
        int r0 = 0;
        int r1 = rowLen;
        int r2 = 2 * rowLen;

        for (int j = 0; j <= n; j++) buffer[r0 + j] = j;

        buffer[r1] = 1;
        int row1Min = 1;
        for (int j = 1; j <= n; j++)
        {
            int cost = (source[0] == target[j - 1]) ? 0 : 1;
            int v = Math.Min(
                Math.Min(buffer[r1 + j - 1] + 1,
                         buffer[r0 + j]     + 1),
                         buffer[r0 + j - 1] + cost
            );
            buffer[r1 + j] = v;
            if (v < row1Min) row1Min = v;
        }
        if (m == 1) return buffer[r1 + n];
        if (row1Min > maxDistance) return maxDistance + 1;

        for (int i = 2; i <= m; i++)
        {
            buffer[r2] = i;
            int rowMin = i;

            for (int j = 1; j <= n; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;

                int v = Math.Min(
                    Math.Min(buffer[r2 + j - 1] + 1,
                             buffer[r1 + j]     + 1),
                             buffer[r1 + j - 1] + cost
                );

                if (j >= 2
                    && source[i - 1] == target[j - 2]
                    && source[i - 2] == target[j - 1])
                {
                    int transposeCost = buffer[r0 + j - 2] + 1;
                    if (transposeCost < v) v = transposeCost;
                }

                buffer[r2 + j] = v;
                if (v < rowMin) rowMin = v;
            }

            // Early exit: every cell of the next row is >= rowMin (monotone DP),
            // so if rowMin already exceeds maxDistance the final answer must too.
            if (rowMin > maxDistance) return maxDistance + 1;

            int tmp = r0;
            r0 = r1;
            r1 = r2;
            r2 = tmp;
        }

        return buffer[r1 + n];
    }
}
