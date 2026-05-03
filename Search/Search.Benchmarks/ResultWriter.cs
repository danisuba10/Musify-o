// Search.Benchmarks/ResultWriter.cs
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Search.Benchmarks;

internal sealed class ResultWriter : IDisposable
{
    private readonly string _path;
    private bool _headerWritten;

    public ResultWriter(string path)
    {
        _path = path;
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        // Detect whether the file already has a header row.
        _headerWritten = File.Exists(path) && new FileInfo(path).Length > 0;
    }

    /// <summary>Appends a completed load-test row to the CSV.</summary>
    public void Append(long entityCount, NBomberStats stats)
    {
        using var stream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        using var csv    = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = !_headerWritten,
        });

        if (!_headerWritten)
        {
            csv.WriteHeader<CsvRow>();
            csv.NextRecord();
            _headerWritten = true;
        }

        csv.WriteRecord(new CsvRow
        {
            Variant         = stats.VariantName,
            EntityCount     = entityCount,
            ConcurrentUsers = stats.ConcurrentUsers,
            MinMs           = Math.Round(stats.MinMs, 3),
            MeanMs          = Math.Round(stats.MeanMs, 3),
            StdDevMs        = Math.Round(stats.StdDevMs, 3),
            MaxMs           = Math.Round(stats.MaxMs, 3),
            P50Ms           = Math.Round(stats.P50Ms, 3),
            P75Ms           = Math.Round(stats.P75Ms, 3),
            P95Ms           = Math.Round(stats.P95Ms, 3),
            P99Ms           = Math.Round(stats.P99Ms, 3),
            Rps             = Math.Round(stats.RequestsPerSec, 1),
            Failed          = stats.FailedRequests,
            IndexRamMb      = Math.Round(stats.IndexRamMb, 1),
            BuildTimeS      = Math.Round(stats.BuildTimeS, 2),
        });
        csv.NextRecord();
    }

    /// <summary>Appends a SKIPPED_OOM marker row so the matrix remains complete.</summary>
    public void AppendOom(string label)
    {
        using var stream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        using var csv    = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = !_headerWritten,
        });

        if (!_headerWritten)
        {
            csv.WriteHeader<CsvRow>();
            csv.NextRecord();
            _headerWritten = true;
        }

        csv.WriteRecord(new CsvRow
        {
            Variant         = label,
            EntityCount     = -1,
            ConcurrentUsers = -1,
            MeanMs          = double.NaN,
            P50Ms           = double.NaN,
            P95Ms           = double.NaN,
            P99Ms           = double.NaN,
            Rps             = double.NaN,
            Failed          = -1,
            IndexRamMb      = double.NaN,
            BuildTimeS      = double.NaN,
        });
        csv.NextRecord();
    }

    public void Dispose() { }

    /// <summary>
    /// Reads the existing CSV and returns the set of already-completed keys so
    /// the runner can skip them on a resumed run.
    /// Key format: "VariantName|entityCount|concurrentUsers"
    /// </summary>
    public HashSet<string> LoadCompletedKeys()
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(_path) || new FileInfo(_path).Length == 0)
            return keys;

        try
        {
            using var stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            using var csv    = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var variant = csv.GetField<string>("variant") ?? string.Empty;
                var ec      = csv.GetField<long>("entity_count");
                var cu      = csv.GetField<int>("concurrent_users");
                if (ec > 0 && cu > 0)   // skip OOM sentinel rows
                    keys.Add($"{variant}|{ec}|{cu}");
            }
        }
        catch
        {
            // Corrupt or partially-written CSV — start fresh rather than crashing.
            keys.Clear();
        }

        return keys;
    }

    // ── CSV row shape ─────────────────────────────────────────────────────
    private sealed class CsvRow
    {
        [CsvHelper.Configuration.Attributes.Name("variant")]
        public string Variant { get; set; } = string.Empty;

        [CsvHelper.Configuration.Attributes.Name("entity_count")]
        public long EntityCount { get; set; }

        [CsvHelper.Configuration.Attributes.Name("concurrent_users")]
        public int ConcurrentUsers { get; set; }

        [CsvHelper.Configuration.Attributes.Name("min_ms")]
        public double MinMs { get; set; }

        [CsvHelper.Configuration.Attributes.Name("mean_ms")]
        public double MeanMs { get; set; }

        [CsvHelper.Configuration.Attributes.Name("stddev_ms")]
        public double StdDevMs { get; set; }

        [CsvHelper.Configuration.Attributes.Name("max_ms")]
        public double MaxMs { get; set; }

        [CsvHelper.Configuration.Attributes.Name("p50_ms")]
        public double P50Ms { get; set; }

        [CsvHelper.Configuration.Attributes.Name("p75_ms")]
        public double P75Ms { get; set; }

        [CsvHelper.Configuration.Attributes.Name("p95_ms")]
        public double P95Ms { get; set; }

        [CsvHelper.Configuration.Attributes.Name("p99_ms")]
        public double P99Ms { get; set; }

        [CsvHelper.Configuration.Attributes.Name("rps")]
        public double Rps { get; set; }

        [CsvHelper.Configuration.Attributes.Name("failed")]
        public long Failed { get; set; }

        [CsvHelper.Configuration.Attributes.Name("index_ram_mb")]
        public double IndexRamMb { get; set; }

        [CsvHelper.Configuration.Attributes.Name("build_time_s")]
        public double BuildTimeS { get; set; }
    }
}
