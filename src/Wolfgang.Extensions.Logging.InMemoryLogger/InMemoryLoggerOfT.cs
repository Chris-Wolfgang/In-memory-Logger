using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Wolfgang.Extensions.Logging.InMemoryLogger;

/// <summary>
/// An implementation of <see cref="ILogger{TCategoryName}"/> that logs messages to an in-memory collection.
/// </summary>
/// <typeparam name="T">The type whose name is used for the logger category name.</typeparam>
[SuppressMessage("Style", "MA0049:Type name should not match containing namespace",
	Justification = "The package, namespace, and type intentionally share a name; the namespace is named after the project's primary type.")]
public class InMemoryLogger<T> : ILogger<T>
{
	private readonly InMemoryLogger _innerLogger;



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
		_innerLogger = new InMemoryLogger
		(
			typeof(T).FullName ?? typeof(T).Name,
			minLogLevel,
			capacity
		);
	}



	/// <inheritdoc />
	public void Log<TState>
	(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter
	)
	{
		_innerLogger.Log(logLevel, eventId, state, exception, formatter);
	}



	/// <inheritdoc />
	public bool IsEnabled(LogLevel logLevel) => _innerLogger.IsEnabled(logLevel);



	/// <inheritdoc />
	public IDisposable BeginScope<TState>(TState state) where TState : notnull => _innerLogger.BeginScope(state);



	/// <summary>
	/// Returns a readonly list of log entries that have been logged.
	/// </summary>
	public IReadOnlyList<LogEntry<object>> LogEntries => _innerLogger.LogEntries;



	/// <summary>
	/// Gets the minimum <see cref="Microsoft.Extensions.Logging.LogLevel"/> for this instance.
	/// </summary>
	public LogLevel MinimumLogLevel => _innerLogger.MinimumLogLevel;



	/// <summary>
	/// The capacity of the internal log entries collection.
	/// </summary>
	public int Capacity => _innerLogger.Capacity;



	/// <summary>
	/// Gets the currently active scopes as an array, from outermost to innermost.
	/// </summary>
	public object[] Scopes => _innerLogger.Scopes;
}
