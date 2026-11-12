using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wolfgang.Extensions.Logging.InMemoryLogger;

/// <summary>
/// An <see cref="ILoggerProvider"/> that creates <see cref="InMemoryLogger"/> instances
/// and provides access to all log entries across all loggers.
/// </summary>
/// <example>
/// <code>
/// using Microsoft.Extensions.Logging;
/// using Wolfgang.Extensions.Logging.InMemoryLogger;
///
/// var provider = new InMemoryLoggerProvider(LogLevel.Information);
/// using var factory = LoggerFactory.Create(builder => builder.AddInMemoryLogger(provider));
///
/// factory.CreateLogger("Orders").LogInformation("placed");
/// factory.CreateLogger("Billing").LogInformation("charged");
///
/// var all = provider.LogEntries.Count; // 2
/// var orders = provider.Loggers["Orders"].LogEntries.Count; // 1
/// </code>
/// </example>
public sealed class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, InMemoryLogger> _loggers = new(StringComparer.Ordinal);

    private readonly LogLevel _minLogLevel;



    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryLoggerProvider"/> class.
    /// </summary>
    /// <param name="minLogLevel">The minimum log level for all loggers created by this provider.</param>
    public InMemoryLoggerProvider(LogLevel minLogLevel = LogLevel.Trace)
    {
        _minLogLevel = minLogLevel;
    }



    /// <summary>
    /// Creates a new <see cref="InMemoryLogger"/> with the given category name.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>An <see cref="ILogger"/> instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="categoryName"/> is <see langword="null"/>.
    /// </exception>
    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd
        (
            categoryName ?? throw new ArgumentNullException(nameof(categoryName)),
            name => new InMemoryLogger(name, _minLogLevel)
        );
    }



    /// <summary>
    /// Returns all log entries from all loggers created by this provider.
    /// Entries are grouped by logger category; ordering across categories is not guaranteed.
    /// </summary>
    /// <remarks>
    /// Each access materializes a snapshot of the entries across all loggers: every logger's own
    /// <see cref="InMemoryLogger.LogEntries"/> snapshot is copied once more into the combined array,
    /// so the cost is two copies of the n entries (2n) plus one array per logger. Test code
    /// typically reads this once per assertion.
    /// </remarks>
    [SuppressMessage("Major Code Smell", "S2365:Properties should not make collection or array copies",
        Justification = "This is a test-helper property whose entire purpose is to return a snapshot view; the copy semantics are intentional and idiomatic for assertion code.")]
    public IReadOnlyList<LogEntry<object>> LogEntries
    {
        get
        {
            return _loggers.Values
                .SelectMany(logger => logger.LogEntries)
                .ToArray();
        }
    }



    /// <summary>
    /// Returns all logger instances created by this provider.
    /// </summary>
    /// <remarks>
    /// Each access materializes a snapshot copy of the underlying logger map; cost is O(n)
    /// in the number of distinct categories logged through this provider.
    /// </remarks>
    [SuppressMessage("Major Code Smell", "S2365:Properties should not make collection or array copies",
        Justification = "This is a test-helper property whose entire purpose is to return a snapshot view; the copy semantics are intentional and idiomatic for assertion code.")]
    public IReadOnlyDictionary<string, InMemoryLogger> Loggers =>
        new Dictionary<string, InMemoryLogger>(_loggers, StringComparer.Ordinal);



    /// <inheritdoc />
    public void Dispose()
    {
    }
}
