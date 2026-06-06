# Getting Started

This guide will help you quickly get up and running with Wolfgang.Extensions.Logging.InMemoryLogger.

## Prerequisites

- The .NET 10.0 SDK is required to build the highest TFM.
- Building the `net462` target additionally requires the .NET Framework 4.6.2 reference assemblies / targeting pack (typically installed via Visual Studio's ".NET desktop development" workload on Windows; the SDK alone does not include them, and `net462` builds are Windows-only).
- The `netstandard2.0`, `netstandard2.1`, and `net8.0` targets build with the .NET 10 SDK alone on any OS.

The library ships against `net462`, `netstandard2.0`, `netstandard2.1`, `net8.0`, and `net10.0`.

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Extensions.Logging.InMemoryLogger
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.Logging.InMemoryLogger
```

## Quick Start

Construct an `InMemoryLogger<T>` and inject it wherever an `ILogger<T>` is expected:

```csharp
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.InMemoryLogger;

var logger = new InMemoryLogger<MyService>(LogLevel.Information);

// Production code under test
var service = new MyService(logger);
service.DoWork();

// Assert what was logged
Assert.Single(logger.LogEntries);
Assert.Equal(LogLevel.Information, logger.LogEntries[0].LogLevel);
```

When you need a non-generic `ILogger` and want to control the category string directly:

```csharp
var logger = new InMemoryLogger(
    category: "MyApp.MyComponent",
    minLogLevel: LogLevel.Warning,
    capacity: 32);
```

To register with the standard `ILoggerFactory`:

```csharp
using var loggerFactory = LoggerFactory.Create(b => b.AddInMemoryLogger());
var logger = loggerFactory.CreateLogger<MyService>();
```

State passed to `BeginScope` is captured in the `Scopes` property:

```csharp
using (logger.BeginScope(new { CorrelationId = "abc-123" }))
{
    logger.LogInformation("processing");
}

Assert.Single(logger.Scopes);
```

## Next Steps

- Explore the [API Reference](https://chris-wolfgang.github.io/In-memory-Logger/versions/latest/api/Wolfgang.Extensions.Logging.InMemoryLogger.html) for detailed documentation
- Read the [Introduction](introduction.md) to learn more about Wolfgang.Extensions.Logging.InMemoryLogger
- Check out example projects in the [GitHub repository](https://github.com/Chris-Wolfgang/In-memory-Logger)

## Common Issues

### `LogEntries` is empty even though my code logged

The `InMemoryLogger` constructor takes a `minLogLevel` (defaults to `LogLevel.Trace`). Anything below that threshold is filtered before reaching `LogEntries`. If you constructed the logger with `LogLevel.Warning` and the code under test only emits `Information`, no entries are captured. Lower the threshold or check `IsEnabled` first.

### `Scopes` is empty after `BeginScope`

The scope must be active *while* the log call runs — the `using` block has to wrap the `Log*` call, not just the `BeginScope` invocation. The library captures scope state when an entry is appended, not when the scope opens.

### Enumerating `LogEntries` from another thread

`LogEntries` is safe to read while a producer thread is still appending — the underlying buffer is thread-safe, and the property returns an `IReadOnlyList<LogEntry<object>>` snapshot prefix. You do not need to lock or copy before asserting.

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/In-memory-Logger)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/In-memory-Logger/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/In-memory-Logger/issues)
