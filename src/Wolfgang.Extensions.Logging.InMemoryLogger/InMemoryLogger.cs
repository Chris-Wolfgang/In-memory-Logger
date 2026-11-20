using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wolfgang.Extensions.Logging.InMemoryLogger;

/// <summary>
/// An implementation of <see cref="ILogger"/> that logs messages to an in-memory collection.
/// </summary>
[SuppressMessage("Style", "MA0049:Type name should not match containing namespace",
    Justification = "The package, namespace, and type intentionally share a name; the namespace is named after the project's primary type.")]
public class InMemoryLogger : ILogger
{
    private readonly List<LogEntry<object>> _logEntries;
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif
    private readonly AsyncLocal<ScopeStack?> _scopeStack = new();



    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryLogger"/> class.
    /// </summary>
    /// <param name="category">The category name for messages produced by the logger.</param>
    /// <param name="minLogLevel">The minimum log level to log.</param>
    /// <param name="capacity">The initial capacity of the log entries collection.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="category"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity"/> is less than 1.</exception>
    /// <example>
    /// <code>
    /// using Microsoft.Extensions.Logging;
    /// using Wolfgang.Extensions.Logging.InMemoryLogger;
    ///
    /// var logger = new InMemoryLogger("MyApp.Orders", LogLevel.Information, capacity: 32);
    /// logger.LogInformation("Order {Id} placed", 42);
    /// logger.LogDebug("below the minimum level, not captured");
    ///
    /// var captured = logger.LogEntries.Count; // 1
    /// var level = logger.LogEntries[0].LogLevel; // LogLevel.Information
    /// </code>
    /// </example>
    public InMemoryLogger
    (
        string category,
        LogLevel minLogLevel = LogLevel.Trace,
        int capacity = 16
    )
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));

        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Value cannot be less than 1");
        }

        MinimumLogLevel = minLogLevel;
        _logEntries = new List<LogEntry<object>>(capacity);
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
            Category,
            eventId,
            state!,
            exception,
            FormatterAdapter<TState>.For(formatter)
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
        if (logLevel == LogLevel.None || MinimumLogLevel == LogLevel.None)
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
    /// <remarks>
    /// Scopes may be disposed in any order. A disposed scope stops appearing in <see cref="Scopes"/>
    /// at once; when an inner scope is disposed after its outer scope, both leave the stack, so
    /// later scopes are not nested under a scope that was already disposed. Disposing a handle
    /// more than once has no further effect.
    /// </remarks>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        var parent = _scopeStack.Value;
        var scope = new ScopeStack(state, parent);
        _scopeStack.Value = scope;

        return new ScopeDisposable(this, scope);
    }



    /// <summary>
    /// Returns a readonly list of log entries that have been logged.
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
    /// Gets the category name for this logger.
    /// </summary>
    public string Category { get; }



    /// <summary>
    /// Gets the minimum <see cref="Microsoft.Extensions.Logging.LogLevel"/> for this instance.
    /// </summary>
    public LogLevel MinimumLogLevel { get; }



    /// <summary>
    /// The capacity of the internal log entries collection.
    /// </summary>
    /// <remarks>
    /// This is the <em>current</em> capacity of the internal list, not the constructor argument
    /// echoed back: it starts at the <c>capacity</c> passed to the constructor and grows as
    /// entries are added beyond it.
    /// </remarks>
    public int Capacity
    {
        get
        {
            lock (_lock)
            {
                return _logEntries.Capacity;
            }
        }
    }



    /// <summary>
    /// Gets the currently active scopes as an array, from outermost to innermost.
    /// </summary>
    /// <remarks>
    /// This is the live scope stack of the current async flow: a scope appears while its
    /// <see cref="BeginScope{TState}(TState)"/> handle is undisposed and disappears when it is
    /// disposed. It is not recorded into <see cref="LogEntries"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// using Microsoft.Extensions.Logging;
    /// using Wolfgang.Extensions.Logging.InMemoryLogger;
    ///
    /// var logger = new InMemoryLogger("MyApp");
    /// using (logger.BeginScope("request-17"))
    /// using (logger.BeginScope(new { UserId = 5 }))
    /// {
    ///     var depth = logger.Scopes.Length; // 2
    ///     var outermost = logger.Scopes[0]; // "request-17"
    /// }
    ///
    /// var afterwards = logger.Scopes.Length; // 0
    /// </code>
    /// </example>
    public object[] Scopes
    {
        get
        {
            var current = _scopeStack.Value;
            if (current == null)
            {
                return Array.Empty<object>();
            }

            // One walk of the parent chain from the innermost end, skipping scopes whose handle
            // was disposed out of order (they stay linked until the scopes above them go), then
            // reverse so the result runs from outermost to innermost. A single walk keeps the
            // result consistent if another thread disposes a scope while this getter runs.
            var live = new List<object>();
            for (var scope = current; scope != null; scope = scope.Parent)
            {
                if (!scope.IsDisposed)
                {
                    live.Add(scope.State);
                }
            }

            var scopes = new object[live.Count];
            for (var i = 0; i < scopes.Length; i++)
            {
                scopes[i] = live[scopes.Length - 1 - i];
            }

            return scopes;
        }
    }



    /// <summary>
    /// Caches the adapter from the caller's <c>Func&lt;TState, Exception?, string&gt;</c> to the
    /// <c>Func&lt;object, Exception?, string&gt;</c> a <see cref="LogEntry{TState}"/> of
    /// <see cref="object"/> stores. Microsoft.Extensions.Logging passes the same cached formatter
    /// instance on every <c>LogInformation(...)</c>-style call, so remembering the last formatter
    /// seen per <typeparamref name="TState"/> makes the common path allocation-free instead of
    /// allocating a closure per call.
    /// </summary>
    /// <typeparam name="TState">The state type of the log call.</typeparam>
    [SuppressMessage("Minor Code Smell", "S2743:Static fields should not be used in generic types",
        Justification = "One cache slot per TState is the point: the adapter's type depends on TState.")]
    private static class FormatterAdapter<TState>
    {
        private static Entry? _last;



        public static Func<object, Exception?, string> For(Func<TState, Exception?, string> formatter)
        {
            // A single reference read and write: racing callers can only replace the slot with an
            // equally valid entry, never observe a torn one.
            var last = Volatile.Read(ref _last);
            if (last != null && ReferenceEquals(last.Formatter, formatter))
            {
                return last.Adapter;
            }

            var created = new Entry(formatter);
            Volatile.Write(ref _last, created);
            return created.Adapter;
        }



        private sealed class Entry
        {
            public Entry(Func<TState, Exception?, string> formatter)
            {
                Formatter = formatter;
                Adapter = (state, exception) => formatter((TState)state, exception);
            }



            public Func<TState, Exception?, string> Formatter { get; }



            public Func<object, Exception?, string> Adapter { get; }
        }
    }



    private sealed class ScopeStack
    {
        private int _disposed;



        public ScopeStack(object state, ScopeStack? parent)
        {
            State = state;
            Parent = parent;
        }



        public object State { get; }



        public ScopeStack? Parent { get; }



        public bool IsDisposed => Volatile.Read(ref _disposed) == 1;



        /// <summary>
        /// Marks the scope as disposed. Returns <see langword="true"/> only for the first call.
        /// </summary>
        public bool TryMarkDisposed()
        {
            return Interlocked.CompareExchange(ref _disposed, 1, 0) == 0;
        }
    }



    private sealed class ScopeDisposable : IDisposable
    {
        private readonly InMemoryLogger _logger;
        private readonly ScopeStack _scope;



        public ScopeDisposable(InMemoryLogger logger, ScopeStack scope)
        {
            _logger = logger;
            _scope = scope;
        }



        public void Dispose()
        {
            // The first dispose marks the scope. If it is the current top of the stack it is
            // popped together with every ancestor directly beneath it that was already disposed
            // out of order; otherwise it stays linked (so the scopes above it are not corrupted)
            // but Scopes no longer reports it.
            if (!_scope.TryMarkDisposed()
                || !ReferenceEquals(_logger._scopeStack.Value, _scope))
            {
                return;
            }

            var top = _scope.Parent;
            while (top != null && top.IsDisposed)
            {
                top = top.Parent;
            }

            _logger._scopeStack.Value = top;
        }
    }
}
