using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks;

/// <summary>
/// Microbenchmarks for the InMemoryLogger paths that do not grow the entry buffer. Each benchmark
/// measures what its description says; MemoryDiagnoser reports the bytes allocated per operation so
/// a regression shows on the gh-pages chart (benchmarks.yaml) and in the per-PR delta.
/// </summary>
/// <remarks>
/// Expected allocations per operation:
/// <list type="bullet">
///   <item><description><c>Log</c> below <c>MinimumLogLevel</c>: none (early return).</description></item>
///   <item><description><c>IsEnabled</c>: none.</description></item>
///   <item><description><c>BeginScope</c> + dispose: the scope node and its disposable handle,
///   plus the execution-context copies that setting and restoring the <c>AsyncLocal</c> scope stack
///   makes (144 B per round trip measured on .NET 10).</description></item>
///   <item><description><c>LogEntries</c>: one array of <see cref="SnapshotSize"/> entries; the
///   property returns a copy taken under the lock.</description></item>
/// </list>
/// The enabled <c>Log</c> path, which appends, is measured by <see cref="InMemoryLoggerAppendBenchmarks"/>.
/// </remarks>
[MemoryDiagnoser]
public class InMemoryLoggerBenchmarks
{
    /// <summary>Number of entries in the logger the <c>LogEntries</c> snapshot benchmark reads.</summary>
    public const int SnapshotSize = 1_000;

    private static readonly EventId Event = new(1, "bench");

    private InMemoryLogger _filteredLogger = null!;

    private InMemoryLogger _snapshotLogger = null!;



    [GlobalSetup]
    public void Setup()
    {
        _filteredLogger = new InMemoryLogger("Wolfgang.Bench.Filtered", LogLevel.Critical);

        _snapshotLogger = new InMemoryLogger("Wolfgang.Bench.Snapshot", LogLevel.Trace, SnapshotSize);
        for (var i = 0; i < SnapshotSize; i++)
        {
            _snapshotLogger.Log(LogLevel.Information, Event, "seed", exception: null, Identity);
        }
    }



    [Benchmark(Description = "Log() — below MinimumLogLevel (early return)")]
    public void LogBelowMinimum()
    {
        _filteredLogger.Log(LogLevel.Information, Event, "hello", exception: null, Identity);
    }



    [Benchmark(Description = "IsEnabled()")]
    public bool IsEnabled() => _filteredLogger.IsEnabled(LogLevel.Information);



    [Benchmark(Description = "BeginScope() — enter + dispose")]
    public void BeginScopeRoundtrip()
    {
        using var _ = _filteredLogger.BeginScope("scope-state");
    }



    [Benchmark(Description = "LogEntries — snapshot of 1,000 entries")]
    public int LogEntriesSnapshot() => _snapshotLogger.LogEntries.Count;



    private static string Identity(string state, Exception? exception) => state;
}



/// <summary>
/// The enabled <c>Log</c> path, which appends an entry. Every iteration starts from a fresh logger
/// sized for exactly that iteration's <see cref="InvocationsPerIteration"/> calls, so the measured
/// cost never includes a <c>List&lt;T&gt;</c> reallocation and the buffer does not grow across
/// iterations the way a single long-lived logger's would.
/// </summary>
/// <remarks>
/// Expected allocations per operation: none. The entry is a struct stored in the pre-sized list,
/// and the formatter adapter is cached per formatter.
/// </remarks>
[MemoryDiagnoser]
[InvocationCount(InvocationsPerIteration)]
public class InMemoryLoggerAppendBenchmarks
{
    /// <summary>
    /// Log() calls per iteration; the logger is recreated with this capacity before each one. Sized
    /// so an iteration takes over 100 ms (BenchmarkDotNet's MinIterationTime guidance) at ~45 ns a call.
    /// </summary>
    public const int InvocationsPerIteration = 2_500_000;

    private static readonly EventId Event = new(1, "bench");

    private InMemoryLogger _logger = null!;



    [IterationSetup]
    public void IterationSetup()
    {
        _logger = new InMemoryLogger("Wolfgang.Bench", LogLevel.Trace, InvocationsPerIteration);
    }



    [Benchmark(Description = "Log() — enabled (Information), scalar state")]
    public void LogInformation()
    {
        _logger.Log(LogLevel.Information, Event, "hello", exception: null, Identity);
    }



    private static string Identity(string state, Exception? exception) => state;
}
