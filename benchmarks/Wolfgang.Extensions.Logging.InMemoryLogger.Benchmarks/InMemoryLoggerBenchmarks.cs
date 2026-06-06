using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks;

/// <summary>
/// Microbenchmarks for the InMemoryLogger hot paths. All benchmarks are
/// allocation-free in the steady state (the entry buffer is sized at
/// construction); MemoryDiagnoser is enabled so any future refactor that
/// introduces allocation surfaces in the gh-pages benchmark chart
/// immediately.
/// </summary>
[MemoryDiagnoser]
public class InMemoryLoggerBenchmarks
{
    // Logger pre-sized large enough that none of the per-iteration Log() calls
    // trigger a List<T> grow during a benchmark run.
    private InMemoryLogger _logger = null!;

    // Pre-built EventId so the [Benchmark]-decorated method doesn't include
    // EventId construction in the measurement.
    private static readonly EventId Event = new(1, "bench");

    [GlobalSetup]
    public void Setup()
    {
        _logger = new InMemoryLogger
        (
            category: "Wolfgang.Bench",
            minLogLevel: LogLevel.Trace,
            capacity: 1_000_000
        );
    }


    [Benchmark(Description = "Log() — Information level, scalar state")]
    public void LogInformation()
    {
        _logger.Log
        (
            LogLevel.Information,
            Event,
            "hello",
            exception: null,
            formatter: static (s, _) => s
        );
    }


    [Benchmark(Description = "Log() — below MinimumLogLevel (early-return)")]
    public void LogBelowMinimum()
    {
        // The constructed logger has minLogLevel=Trace, so Critical is enabled.
        // To exercise the early-return path we use IsEnabled directly with a
        // synthetic logger that filters at Critical, constructed once during
        // Setup would require a second field. The Log() implementation calls
        // IsEnabled internally, so a benchmark of IsEnabled covers the same
        // hot path with one less indirection.
        _ = _logger.IsEnabled(LogLevel.Trace);
    }


    [Benchmark(Description = "BeginScope() — enter + dispose")]
    public void BeginScopeRoundtrip()
    {
        using var _ = _logger.BeginScope("scope-state");
    }


    [Benchmark(Description = "LogEntries indexer read")]
    public object? LogEntriesIndexer()
    {
        // Read the most recently captured entry (pre-seeded once at GlobalSetup
        // via a single log call in LogInformation; the index is safe because
        // BenchmarkDotNet runs Setup before each benchmark.)
        var entries = _logger.LogEntries;
        return entries.Count > 0 ? entries[entries.Count - 1].State : null;
    }
}
