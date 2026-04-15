using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wolfgang.Extensions.Logging.InMemoryLogger;

/// <summary>
/// An implementation of <see cref="ILogger"/> that logs messages to an in-memory collection.
/// </summary>
public class InMemoryLogger : ILogger
{
	private readonly List<LogEntry<object>> _logEntries;
	private readonly object _lock = new object();
	private readonly AsyncLocal<ScopeStack?> _scopeStack = new AsyncLocal<ScopeStack?>();



	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryLogger"/> class.
	/// </summary>
	/// <param name="category">The category name for messages produced by the logger.</param>
	/// <param name="minLogLevel">The minimum log level to log.</param>
	/// <param name="capacity">The initial capacity of the log entries collection.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="category"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than 1.</exception>
	public InMemoryLogger
	(
		string category,
		LogLevel minLogLevel = LogLevel.Trace,
		int capacity = 16
	)
	{
		if (category == null)
		{
			throw new ArgumentNullException(nameof(category));
		}

		if (capacity < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(capacity), "Value cannot be less than 1");
		}

		Category = category;
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
	public object[] Scopes
	{
		get
		{
			var current = _scopeStack.Value;
			if (current == null)
			{
				return Array.Empty<object>();
			}

			var scopes = new List<object>();
			while (current != null)
			{
				scopes.Add(current.State);
				current = current.Parent;
			}

			scopes.Reverse();
			return scopes.ToArray();
		}
	}



	private sealed class ScopeStack
	{
		public ScopeStack(object state, ScopeStack? parent)
		{
			State = state;
			Parent = parent;
		}



		public object State { get; }



		public ScopeStack? Parent { get; }
	}



	private sealed class ScopeDisposable : IDisposable
	{
		private readonly InMemoryLogger _logger;
		private readonly ScopeStack _scope;
		private int _disposed;



		public ScopeDisposable(InMemoryLogger logger, ScopeStack scope)
		{
			_logger = logger;
			_scope = scope;
		}



		public void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
			{
				// Only pop if this scope is still the current top-of-stack.
				// Out-of-order disposal is silently ignored to avoid corrupting the stack.
				if (ReferenceEquals(_logger._scopeStack.Value, _scope))
				{
					_logger._scopeStack.Value = _scope.Parent;
				}
			}
		}
	}
}
