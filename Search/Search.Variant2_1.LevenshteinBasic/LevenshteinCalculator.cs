namespace Search.Variant2_1.LevenshteinBasic;

/// <summary>
/// Computes the standard Levenshtein edit distance between two strings.
/// Uses the 1D space-optimised sliding window: O(n) space, O(nm) time.
/// HEAP ALLOCATED: both working arrays are allocated with `new int[]`.
///
/// Implementation basis: Navarro (2001), Section 3.1.
/// Space reduction: classic DP observation — only the previous row is needed.
/// </summary>
internal static class LevenshteinCalculator
{
    internal static int Compute(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        int m = source.Length;
        int n = target.Length;

        if (m == 0) return n;
        if (n == 0) return m;
        if (source.SequenceEqual(target)) return 0;

        // Ensure n >= m so we allocate the smaller array.
        if (m > n)
        {
            var tmp = source; source = target; target = tmp;
            int t = m; m = n; n = t;
        }

        // Heap-allocated working arrays (the key difference vs. Variant 2.2)
        int[] prev = new int[n + 1];
        int[] curr = new int[n + 1];

        // Base case: distance from empty string to target prefixes
        for (int j = 0; j <= n; j++) prev[j] = j;

        for (int i = 1; i <= m; i++)
        {
            curr[0] = i;

            for (int j = 1; j <= n; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;
                curr[j] = Math.Min(
                    Math.Min(curr[j - 1] + 1,   // insert
                             prev[j]     + 1),  // delete
                             prev[j - 1] + cost  // substitute
                );
            }

            // Swap prev and curr (pointer swap, zero copying)
            (prev, curr) = (curr, prev);
        }

        return prev[n];
    }
}
