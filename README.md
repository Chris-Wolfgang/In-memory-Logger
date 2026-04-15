# Wolfgang.Extensions.Logging.InMemoryLogger

An implementation of `ILogger<T>` from `Microsoft.Extensions.Logging` that writes log entries to an in-memory collection. Designed for use in unit and integration tests where you need to assert against logged messages.

## Features

- Thread-safe in-memory log entry storage
- Configurable minimum log level
- Configurable initial capacity
- Supports all .NET log levels (Trace through Critical)
- Returns log entries as `IReadOnlyList<LogEntry<object>>` for easy assertion

## Installation

```shell
dotnet add package Wolfgang.Extensions.Logging.InMemoryLogger
```

## Usage

```csharp
var logger = new InMemoryLogger<MyService>(LogLevel.Information);

// Use the logger in your code under test
var service = new MyService(logger);
service.DoWork();

// Assert against captured log entries
Assert.Single(logger.LogEntries);
Assert.Equal(LogLevel.Information, logger.LogEntries[0].LogLevel);
```

## Target Frameworks

- .NET Framework 4.6.2
- .NET Standard 2.0
- .NET Standard 2.1
- .NET 8.0
- .NET 10.0

## License

[MIT](LICENSE)
