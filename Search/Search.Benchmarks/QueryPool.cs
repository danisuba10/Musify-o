// Search.Benchmarks/QueryPool.cs
namespace Search.Benchmarks;

internal static class QueryPool
{
    // Fixed 60-term pool — covers exact, typo, prefix, multi-word, and gibberish.
    // Do NOT generate or extend this list dynamically; doing so changes trigram hit rates.
    //
    // EXACT QUERIES: derived directly from DataGenerator._pinned (lowercased).
    // Those entries are always injected at the start of Generate(), so every benchmark
    // run is guaranteed to contain at least one entity matching each exact query.
    // If you change _pinned, update the exact section below to match.
    private static readonly string[] Queries =
    {
        // Exact — each term matches a guaranteed pinned entity in DataGenerator._pinned
        "dark wave",    "luna",         "rise the fire",  "silent road",
        "golden dream", "electric edge","broken heart",   "midnight flame",
        "crystal echo", "neon storm",   "ivory path",     "jade moon",
        "lunar ghost",  "mystic haven", "burning light",  "ancient mirror",

        // Single-edit typos (transpositions / substitutions)
        "lukna",        "eelctric",     "blde",           "silennt",
        "goldin",       "brokn",        "midnigt",        "crystl",
        "noen",         "ivorey",       "jadde",          "lunarr",

        // Two-edit typos (2 substitutions, or 1 transposition + 1 deletion, etc.)
        // Each is derived from a pinned name so the target entity is guaranteed present.
        "drak waev",    "eelctrik",     "goldne dreem",   "burnng lgiht",
        "siilent rod",  "jad moun",     "mysitc havne",   "ancinet mirro",
        "crsytal ehco", "ivroy phat",   "lnarr goost",    "mdinight flmae",
        "noen strm",    "rize teh fier","elctric egde",   "brokn herat",

        // Prefix / partial
        "mid",          "ech",          "bre",            "sil",
        "gol",          "lun",          "cry",            "neo",

        // Multi-word
        "the ancient",  "jade & the",   "rise the",       "of the moon",
        "dark and",     "electric storm","lost in the",    "beyond the",

        // Gibberish (must return empty)
        "zzz",          "xqk",          "qwerty",         "aaaaaa",
        "bbbbb",        "zzzzzz",       "123",            "!!!"
    };

    private static readonly ThreadLocal<Random> Rng =
        new(() => new Random(Environment.CurrentManagedThreadId));

    public static string GetRandom() => Queries[Rng.Value!.Next(Queries.Length)];
}
