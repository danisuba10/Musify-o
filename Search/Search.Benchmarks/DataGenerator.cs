// Search.Benchmarks/DataGenerator.cs
using Search.Abstractions;

namespace Search.Benchmarks;

internal static class DataGenerator
{
    // ── Vocabulary pools — FIXED, COMPLETE, DO NOT MODIFY ────────────────
    // These lists are the canonical definition of the benchmark dataset.
    // Any change to these lists changes the trigram distribution, breaking
    // cross-run and cross-machine comparability. Treat as immutable constants.

    // 100 adjectives (seed = determinism; coverage = varied trigrams)
    private static readonly string[] Adjectives =
    {
        "Dark",      "Electric",  "Silent",    "Golden",    "Broken",
        "Midnight",  "Hollow",    "Vivid",     "Ancient",   "Burning",
        "Crystal",   "Digital",   "Endless",   "Faded",     "Giant",
        "Hidden",    "Ivory",     "Jade",      "Lunar",     "Mystic",
        "Neon",      "Onyx",      "Primal",    "Quantum",   "Rising",
        "Savage",    "Tender",    "Urban",     "Velvet",    "Wicked",
        "Arctic",    "Blazing",   "Cosmic",    "Dreamy",    "Emerald",
        "Frozen",    "Glowing",   "Hyper",     "Icy",       "Jagged",
        "Knotted",   "Luminous",  "Molten",    "Noble",     "Obsidian",
        "Phantom",   "Radiant",   "Scarlet",   "Twisted",   "Undying",
        "Violent",   "Wandering", "Amber",     "Bronze",    "Crimson",
        "Dazzling",  "Eternal",   "Fiery",     "Graceful",  "Haunted",
        "Iridescent","Jovial",    "Lavender",  "Majestic",  "Natural",
        "Pale",      "Quiet",     "Rustic",    "Smoky",     "Tranquil",
        "Vast",      "Weary",     "Bitter",    "Calm",      "Eerie",
        "Fierce",    "Grave",     "Heavy",     "Infinite",  "Shallow",
        "Lofty",     "Mellow",    "Narrow",    "Open",      "Pure",
        "Raw",       "Sober",     "Tangled",   "Veiled",    "Wild",
        "Yearning",  "Zealous",   "Ageless",   "Boundless", "Cloudy",
        "Dense",     "Empty",     "Fleeting",  "Grand",     "Hollow"
    };

    // 150 nouns
    private static readonly string[] Nouns =
    {
        "Wave",      "Rain",      "Fire",      "Echo",      "Storm",
        "Heart",     "Mirror",    "Road",      "Dream",     "Light",
        "Blade",     "Cloud",     "Dawn",      "Edge",      "Flame",
        "Ghost",     "Haven",     "Island",    "Journey",   "Kingdom",
        "Legend",    "Moon",      "Night",     "Ocean",     "Path",
        "Quest",     "River",     "Shadow",    "Thunder",   "Universe",
        "Valley",    "Wind",      "Abyss",     "Beacon",    "Canyon",
        "Delta",     "Empire",    "Forest",    "Galaxy",    "Horizon",
        "Infinity",  "Jungle",    "Karma",     "Lotus",     "Mountain",
        "Nexus",     "Oracle",    "Portal",    "Riddle",    "Sanctuary",
        "Tower",     "Utopia",    "Vortex",    "Wasteland", "Zenith",
        "Aurora",    "Bridge",    "Castle",    "Desert",    "Eden",
        "Field",     "Garden",    "Harbor",    "Isle",      "Labyrinth",
        "Mirage",    "Nebula",    "Orbit",     "Prairie",   "Realm",
        "Shore",     "Temple",    "Underworld","Void",      "Wilderness",
        "Epoch",     "Flux",      "Gate",      "Iris",      "Knot",
        "Lane",      "Mist",      "Node",      "Omen",      "Pulse",
        "Ridge",     "Shard",     "Tide",      "Veil",      "Well",
        "Arch",      "Bay",       "Crest",     "Drift",     "Ember",
        "Fog",       "Glow",      "Haze",      "Ice",       "Jewel",
        "Key",       "Leaf",      "Mesa",      "Nova",      "Oasis",
        "Peak",      "Quartz",    "Root",      "Seed",      "Trail",
        "Unity",     "Vale",      "Whirl",     "Axis",      "Bloom",
        "Core",      "Depth",     "Essence",   "Frost",     "Grove",
        "Hill",      "Ink",       "Jet",       "Kin",       "Lore",
        "Mark",      "Nerve",     "Ore",       "Pyre",      "Rift",
        "Spark",     "Thorn",     "Umbra",     "Vibe",      "Wake",
        "Ash",       "Bolt",      "Cliff",     "Dusk",      "Eve"
    };

    // 80 verbs
    private static readonly string[] Verbs =
    {
        "Run",   "Fall",  "Rise",  "Shine", "Break", "Dream", "Burn",  "Fly",
        "Bleed", "Chase", "Dance", "Fade",  "Grow",  "Hide",  "Jump",  "Keep",
        "Lose",  "Make",  "Need",  "Open",  "Pull",  "Reach", "Stay",  "Take",
        "Call",  "Find",  "Hold",  "Kill",  "Leave", "Move",  "Name",  "Own",
        "Play",  "Quit",  "Read",  "Save",  "Turn",  "Walk",  "Seek",  "Hear",
        "Feel",  "Know",  "Live",  "Love",  "Miss",  "See",   "Sing",  "Speak",
        "Stand", "Think", "Trust", "Wake",  "Wish",  "Work",  "Write", "Fight",
        "Flow",  "Forge", "Give",  "Guard", "Heal",  "Lead",  "Learn", "Lift",
        "Light", "Meet",  "Mend",  "Paint", "Pray",  "Ride",  "Rock",  "Roll",
        "Roam",  "Sail",  "Soar",  "Spell", "Spin",  "Stir",  "Weep",  "Yield"
    };

    // 200 given names
    private static readonly string[] GivenNames =
    {
        "James", "Aria",  "Leon",  "Maya",  "Cole",  "Zara",  "Finn",  "Nova",
        "Axel",  "Luna",  "Blake", "Jade",  "Crew",  "Iris",  "Drew",  "Lyra",
        "Miles", "Sage",  "Reid",  "Wren",  "Atlas", "Cleo",  "Dean",  "Eden",
        "Fox",   "Gael",  "Hope",  "Ivan",  "Juno",  "Knox",  "Luca",  "Mia",
        "Nash",  "Ora",   "Penn",  "Quinn", "Rex",   "Sky",   "Troy",  "Uma",
        "Wade",  "Xan",   "Yael",  "Zion",  "Ace",   "Bea",   "Cal",   "Dot",
        "Eli",   "Fay",   "Gil",   "Hana",  "Ike",   "Jax",   "Kay",   "Lou",
        "Mae",   "Ned",   "Opal",  "Paz",   "Rue",   "Sam",   "Teo",   "Ula",
        "Val",   "Win",   "Xia",   "Yves",  "Zoe",   "Abel",  "Bram",  "Cruz",
        "Dale",  "Earl",  "Fern",  "Glen",  "Hazel", "Ines",  "Joel",  "Kyle",
        "Lena",  "Marc",  "Nina",  "Owen",  "Prue",  "Remi",  "Seth",  "Tara",
        "Ursa",  "Vera",  "Will",  "Xena",  "Yann",  "Zaid",  "Alma",  "Boyd",
        "Cain",  "Dana",  "Evan",  "Faye",  "Grey",  "Hugh",  "Isak",  "Jana",
        "Kurt",  "Liam",  "Mara",  "Neil",  "Olga",  "Paul",  "Rosa",  "Stan",
        "Thea",  "Umar",  "Vida",  "Ward",  "Xara",  "Yuki",  "Zula",  "Arlo",
        "Beau",  "Clay",  "Demi",  "Ezra",  "Gus",   "Hank",  "Isla",  "Jada",
        "Kira",  "Lars",  "Mads",  "Nico",  "Ola",   "Pax",   "Rox",   "Sven",
        "Tess",  "Udo",   "Viv",   "Wes",   "Yara",  "Zeb",   "Ami",   "Bix",
        "Cam",   "Dev",   "Elio",  "Flo",   "Gio",   "Hiro",  "Jeb",   "Kim",
        "Leo",   "Meg",   "Noa",   "Pip",   "Rae",   "Sol",   "Tad",   "Uri",
        "Von",   "Wei",   "Yul",   "Zev",   "Ada",   "Ben",   "Cyd",   "Del",
        "Eno",   "Gem",   "Hal",   "Ila",   "Jin",   "Kai",   "Lin",   "Max",
        "Nan",   "Obi",   "Peg",   "Rio",   "Stu",   "Tab",   "Una",   "Vee",
        "Wai",   "Yam",   "Zia",   "Amy",   "Bob",   "Cat",   "Don",   "Eve",
        "Gil",   "Hue",   "Ian",   "Joy",   "Ken",   "Lee",   "Nan",   "Ole"
    };

    // ── Pinned entries — GUARANTEED exact matches for QueryPool.ExactQueries ──────────
    // These are always yielded first, before any random generation.
    // QueryPool.ExactQueries is derived directly from these names (lowercase).
    // Never reorder, add, or remove entries here without updating QueryPool.ExactQueries.
    // IDs use slot -1..-16 (encoded via ulong wrap-around) — stable, non-overlapping with
    // the random stream which uses slots 0..count-1.
    private static readonly (string Name, SearchEntityType Type)[] _pinned =
    {
        ("Dark Wave",       SearchEntityType.Song),
        ("Luna",            SearchEntityType.Artist),
        ("Rise the Fire",   SearchEntityType.Song),
        ("Silent Road",     SearchEntityType.Song),
        ("Golden Dream",    SearchEntityType.Song),
        ("Electric Edge",   SearchEntityType.Song),
        ("Broken Heart",    SearchEntityType.Song),
        ("Midnight Flame",  SearchEntityType.Song),
        ("Crystal Echo",    SearchEntityType.Song),
        ("Neon Storm",      SearchEntityType.Song),
        ("Ivory Path",      SearchEntityType.Song),
        ("Jade Moon",       SearchEntityType.Song),
        ("Lunar Ghost",     SearchEntityType.Song),
        ("Mystic Haven",    SearchEntityType.Song),
        ("Burning Light",   SearchEntityType.Song),
        ("Ancient Mirror",  SearchEntityType.Song),
    };

    /// <summary>Returns the deterministic Guid for pinned slot <paramref name="pinnedIndex"/> (0-based).</summary>
    public static Guid PinnedId(int pinnedIndex) =>
        GenerateDeterministicGuid(-(pinnedIndex + 1), seed: 42);

    /// <summary>
    /// Lazily generates <paramref name="count"/> projections using a fixed seed.
    /// The first <c>_pinned.Length</c> entries are always the pinned set (guaranteed
    /// exact matches). The remaining <c>count - _pinned.Length</c> entries are random.
    /// Never allocates all N objects at once — passes them one by one to Build().
    /// </summary>
    public static IEnumerable<SearchProjection> Generate(long count, int seed = 42)
    {
        // 1. Yield pinned entries first — exact-match guarantees.
        for (int p = 0; p < _pinned.Length && p < count; p++)
        {
            yield return new SearchProjection
            {
                Id         = GenerateDeterministicGuid(-(p + 1), seed),
                Name       = _pinned[p].Name,
                EntityType = _pinned[p].Type
            };
        }

        // 2. Yield random entries for the remainder.
        var rng = new Random(seed);
        long remaining = count - _pinned.Length;
        for (long i = 0; i < remaining; i++)
        {
            var type = GetEntityType(i, remaining);
            yield return new SearchProjection
            {
                Id         = GenerateDeterministicGuid(i, seed),
                Name       = GenerateName(type, i, rng),
                EntityType = type
            };
        }
    }

    private static SearchEntityType GetEntityType(long index, long total)
    {
        // Deterministic distribution by position: Song 50%, Artist 20%, Album 20%,
        // Playlist 5%, User 5%  — using modulo so distribution is perfectly uniform.
        // Note: Playlist and User projections are not indexed by the search engines
        // (they filter to Song/Artist/Album only), but they test the engine's
        // entity-filter logic.
        long mod = index % 20;
        return mod switch
        {
            < 10 => SearchEntityType.Song,       // 0–9  → 50%
            < 14 => SearchEntityType.Artist,     // 10–13 → 20%
            < 18 => SearchEntityType.Album,      // 14–17 → 20%
            18   => SearchEntityType.Playlist,   // 18    →  5%
            _    => SearchEntityType.User        // 19    →  5%
        };
    }

    private static string GenerateName(SearchEntityType type, long index, Random rng)
    {
        string suffix = (index / (Adjectives.Length * Nouns.Length)) > 0
            ? $" {index / (Adjectives.Length * Nouns.Length)}"
            : string.Empty;

        return type switch
        {
            SearchEntityType.Song   => GenerateSongName(rng, suffix),
            SearchEntityType.Artist => GenerateArtistName(rng, suffix),
            SearchEntityType.Album  => GenerateAlbumName(rng, suffix),
            _                       => $"{GivenNames[rng.Next(GivenNames.Length)]}_{index}"
        };
    }

    private static string GenerateSongName(Random rng, string suffix) =>
        rng.Next(3) switch
        {
            0 => $"{Verbs[rng.Next(Verbs.Length)]} the {Nouns[rng.Next(Nouns.Length)]}{suffix}",
            1 => $"{Adjectives[rng.Next(Adjectives.Length)]} {Nouns[rng.Next(Nouns.Length)]}{suffix}",
            _ => $"{GivenNames[rng.Next(GivenNames.Length)]}'s {Nouns[rng.Next(Nouns.Length)]}{suffix}"
        };

    private static string GenerateArtistName(Random rng, string suffix) =>
        rng.Next(3) switch
        {
            0 => $"{GivenNames[rng.Next(GivenNames.Length)]} {GivenNames[rng.Next(GivenNames.Length)]}{suffix}",
            1 => $"The {Adjectives[rng.Next(Adjectives.Length)]} {Nouns[rng.Next(Nouns.Length)]}s{suffix}",
            _ => $"{GivenNames[rng.Next(GivenNames.Length)]} & the {Nouns[rng.Next(Nouns.Length)]}s{suffix}"
        };

    private static string GenerateAlbumName(Random rng, string suffix) =>
        rng.Next(3) switch
        {
            0 => $"The {Nouns[rng.Next(Nouns.Length)]} of {Nouns[rng.Next(Nouns.Length)]}{suffix}",
            1 => $"{Adjectives[rng.Next(Adjectives.Length)]} {Nouns[rng.Next(Nouns.Length)]}{suffix}",
            _ => $"{Nouns[rng.Next(Nouns.Length)]} Vol. {(rng.Next(9) + 1)}{suffix}"
        };

    private static Guid GenerateDeterministicGuid(long index, int seed)
    {
        // Produces a stable Guid from index+seed without random drift.
        // Not cryptographic — purely for test data identity.
        // Pinned entries use negative indices (-1..-16); random entries use 0..count-1.
        // Negative longs cast to ulong via two's complement — no collision with positive range.
        Span<byte> bytes = stackalloc byte[16];
        BitConverter.TryWriteBytes(bytes[0..8],  (ulong)index);
        BitConverter.TryWriteBytes(bytes[8..16], (ulong)(uint)seed * 0x9E3779B97F4A7C15UL);
        return new Guid(bytes);
    }
}
