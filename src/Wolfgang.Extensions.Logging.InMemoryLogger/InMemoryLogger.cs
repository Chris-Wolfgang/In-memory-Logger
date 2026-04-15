using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wolfgang.Extensions.Logging.InMemoryLogger;

/// <summary>
/// An implementation of ILogger that logs messages to an in-memory collection.
/// </summary>
public class InMemoryLogger<T> : ILogger<T>
{
    private readonly List<LogEntry<object>> _logEntries;
    private readonly object _lock = new object();
    private readonly string _category;



    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryLogger{T}"/> class.
    /// </summary>
    /// <param name="minLogLevel">The minimum log level to log.</param>
    /// <param name="capacity">The initial capacity of the log entries collection.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than 1.</exception>
    public InMemoryLogger
    (
        LogLevel minLogLevel = LogLevel.Trace,
        int capacity = 16
    )
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Value cannot be less than 1");
        }
        MinimumLogLevel = minLogLevel;

        _logEntries = new List<LogEntry<object>>(capacity);
        _category = typeof(T).FullName ?? typeof(T).Name;
    }



    /// <summary>
    /// Logs the specified data.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="eventId">The event ID associated with the log entry.</param>
    /// <param name="state">The state associated with the log entry.</param>
    /// <param name="exception">The exception associated with the log entry, if any.</param>
    /// <param name="formatter">A function to create a string message from the state and exception.</param>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    public void Log<TState>
    (
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var entry = new LogEntry<object>
        (
            logLevel,
            _category,
            eventId,
            state!,
            exception,
            (o, ex) => formatter((TState)o!, ex)
        );

        lock (_lock)
        {
            _logEntries.Add(entry);
        }
    }



    /// <summary>
    /// Gets a value indicating if the specified log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns><see langword="true"/> if the log level is enabled; otherwise, <see langword="false"/>.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        if (MinimumLogLevel == LogLevel.None)
        {
            return false;
        }

        return (int)logLevel >= (int)MinimumLogLevel;
    }



    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <param name="state">The identifier for the scope.</param>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <returns>A disposable object that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;



    /// <summary>
    /// Returns a readonly collection of log entries that have been logged.
    /// </summary>
    public IReadOnlyList<LogEntry<object>> LogEntries
    {
        get
        {
            lock (_lock)
            {
                return _logEntries.ToArray();
            }
        }
    }



    /// <summary>
    /// Gets the minimum <see cref="Microsoft.Extensions.Logging.LogLevel"/> for this instance.
    /// </summary>
    public LogLevel MinimumLogLevel { get; }



    /// <summary>
    /// The capacity of the internal log entries collection.
    /// </summary>
    public int Capacity => _logEntries.Capacity;



    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new NullScope();



        public void Dispose()
        {
        }
    }
}
