namespace Search.Variant3_1.DamerauBasic;

/// <summary>
/// Computes the Optimal String Alignment (OSA / restricted Damerau-Levenshtein) distance.
/// Extends standard Levenshtein with transposition of adjacent characters (cost 1).
///
/// Algorithm: 3-row sliding window DP.
/// Space complexity: O(3n) = O(n).
/// Time complexity:  O(nm).
///
/// HEAP ALLOCATED: all three working arrays use `new int[]`.
///
/// Academic basis:
///   - Damerau (1964): defines the four edit operations.
///   - Lowrance &amp; Wagner (1975): formalises the DP formulation.
/// </summary>
internal static class DamerauCalculator
{
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

        int[] prev2 = new int[n + 1];   // row i-2
        int[] prev  = new int[n + 1];   // row i-1
        int[] curr  = new int[n + 1];   // row i

        // Row 0: dp[0][j] = j
        for (int j = 0; j <= n; j++) prev2[j] = j;

        // Row 1: no transpositions possible (i < 2)
        prev[0] = 1;
        for (int j = 1; j <= n; j++)
        {
            int cost = (source[0] == target[j - 1]) ? 0 : 1;
            prev[j] = Math.Min(
                Math.Min(prev[j - 1]  + 1,   // insert
                         prev2[j]     + 1),  // delete
                         prev2[j - 1] + cost  // substitute
            );
        }

        // Rows 2..m
        for (int i = 2; i <= m; i++)
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

                // Transposition: s[i-1] == t[j-2] && s[i-2] == t[j-1]
                if (j >= 2
                    && source[i - 1] == target[j - 2]
                    && source[i - 2] == target[j - 1])
                {
                    curr[j] = Math.Min(curr[j], prev2[j - 2] + 1);
                }
            }

            // 3-way rotation: reuse old prev2 as write buffer
            (prev2, prev, curr) = (prev, curr, prev2);
        }

        return prev[n];
    }
}
