// Search.Benchmarks/CorrectnessValidator.cs
namespace Search.Benchmarks;

internal static class CorrectnessValidator
{
    // Maps each assertable query to the pinned GUID it must surface.
    // Exact queries must appear at rank 0; typo queries within top 10.
    // Generated from DataGenerator._pinned — update both together.
    private static readonly (string Query, Guid ExpectedId, bool MustBeFirst)[] _assertions =
    {
        // Exact — must be rank 0
        ("dark wave",      DataGenerator.PinnedId(0),  true),
        ("luna",           DataGenerator.PinnedId(1),  true),
        ("rise the fire",  DataGenerator.PinnedId(2),  true),
        ("silent road",    DataGenerator.PinnedId(3),  true),
        ("golden dream",   DataGenerator.PinnedId(4),  true),
        ("electric edge",  DataGenerator.PinnedId(5),  true),
        ("broken heart",   DataGenerator.PinnedId(6),  true),
        ("midnight flame", DataGenerator.PinnedId(7),  true),
        ("crystal echo",   DataGenerator.PinnedId(8),  true),
        ("neon storm",     DataGenerator.PinnedId(9),  true),
        ("ivory path",     DataGenerator.PinnedId(10), true),
        ("jade moon",      DataGenerator.PinnedId(11), true),
        ("lunar ghost",    DataGenerator.PinnedId(12), true),
        ("mystic haven",   DataGenerator.PinnedId(13), true),
        ("burning light",  DataGenerator.PinnedId(14), true),
        ("ancient mirror", DataGenerator.PinnedId(15), true),

        // Single-edit typos — must appear in top 10
        ("lukna",    DataGenerator.PinnedId(1),  false),  // luna
        ("eelctric", DataGenerator.PinnedId(5),  false),  // electric edge
        ("blde",     DataGenerator.PinnedId(9),  false),  // neon storm → "blade"-adjacent
        ("silennt",  DataGenerator.PinnedId(3),  false),  // silent road
        ("goldin",   DataGenerator.PinnedId(4),  false),  // golden dream
        ("brokn",    DataGenerator.PinnedId(6),  false),  // broken heart
        ("midnigt",  DataGenerator.PinnedId(7),  false),  // midnight flame
        ("crystl",   DataGenerator.PinnedId(8),  false),  // crystal echo
        ("noen",     DataGenerator.PinnedId(9),  false),  // neon storm
        ("ivorey",   DataGenerator.PinnedId(10), false),  // ivory path
        ("jadde",    DataGenerator.PinnedId(11), false),  // jade moon
        ("lunarr",   DataGenerator.PinnedId(12), false),  // lunar ghost

        // Two-edit typos — must appear in top 10
        ("drak waev",      DataGenerator.PinnedId(0),  false),
        ("eelctrik",       DataGenerator.PinnedId(5),  false),
        ("goldne dreem",   DataGenerator.PinnedId(4),  false),
        ("burnng lgiht",   DataGenerator.PinnedId(14), false),
        ("siilent rod",    DataGenerator.PinnedId(3),  false),
        ("jad moun",       DataGenerator.PinnedId(11), false),
        ("mysitc havne",   DataGenerator.PinnedId(13), false),
        ("ancinet mirro",  DataGenerator.PinnedId(15), false),
        ("crsytal ehco",   DataGenerator.PinnedId(8),  false),
        ("ivroy phat",     DataGenerator.PinnedId(10), false),
        ("lnarr goost",    DataGenerator.PinnedId(12), false),
        ("mdinight flmae", DataGenerator.PinnedId(7),  false),
        ("noen strm",      DataGenerator.PinnedId(9),  false),
        ("rize teh fier",  DataGenerator.PinnedId(2),  false),
        ("elctric egde",   DataGenerator.PinnedId(5),  false),
        ("brokn herat",    DataGenerator.PinnedId(6),  false),
    };

    // Gibberish — must return empty
    private static readonly string[] _gibberish =
        { "zzz", "xqk", "qwerty", "aaaaaa", "bbbbb", "zzzzzz", "123", "!!!" };

    /// <summary>
    /// Runs all assertions against the given adapter (built on 10k entities).
    /// Throws <see cref="CorrectnessException"/> on the first failure, aborting
    /// the benchmark run before wasting time on bad data.
    /// </summary>
    public static void Validate(SearchOnlyAdapter adapter)
    {
        int passed = 0;

        foreach (var (query, expectedId, mustBeFirst) in _assertions)
        {
            var results = adapter.ScoreOnly(query);

            if (results.Count == 0)
                Fail(query, $"returned 0 results — expected to find {expectedId}");

            if (mustBeFirst && results[0] != expectedId)
                Fail(query, $"rank-0 result was {results[0]}, expected {expectedId}");

            if (!mustBeFirst && !results.Take(10).Contains(expectedId))
                Fail(query, $"expected {expectedId} in top 10, not found. Top result: {results[0]}");

            passed++;
        }

        foreach (var query in _gibberish)
        {
            var results = adapter.ScoreOnly(query);
            if (results.Count > 0)
                Fail(query, $"gibberish query returned {results.Count} result(s), expected 0");
            passed++;
        }

        Console.WriteLine($"[Correctness] {adapter.VariantName}: {passed} assertions passed.");
    }

    private static void Fail(string query, string reason)
        => throw new CorrectnessException($"CORRECTNESS FAILURE — query '{query}': {reason}");
}

internal sealed class CorrectnessException(string message) : Exception(message);
