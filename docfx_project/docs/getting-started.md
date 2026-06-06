# Getting Started

This guide walks through installing the package and writing your first assertion against `InMemoryLogger<T>`.

## Install

```bash
dotnet add package Wolfgang.Extensions.Logging.InMemoryLogger
```

## Basic usage

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

## Non-generic form

When you need an `ILogger` and want to control the category string directly, use the non-generic constructor:

```csharp
var logger = new InMemoryLogger(
    category: "MyApp.MyComponent",
    minLogLevel: LogLevel.Warning,
    capacity: 32);
```

## Registering with the standard ILoggerFactory

`InMemoryLoggerBuilderExtensions.AddInMemoryLogger` plugs into the standard `ILoggingBuilder`:

```csharp
using var loggerFactory = LoggerFactory.Create(b => b.AddInMemoryLogger());
var logger = loggerFactory.CreateLogger<MyService>();
```

## Asserting on scopes

State passed to `BeginScope` is captured in the `Scopes` property:

```csharp
using (logger.BeginScope(new { CorrelationId = "abc-123" }))
{
    logger.LogInformation("processing");
}

Assert.Single(logger.Scopes);
```

## Next steps

- [API Reference](../api/Wolfgang.Extensions.Logging.InMemoryLogger.html)
- [GitHub repository](https://github.com/Chris-Wolfgang/In-memory-Logger)
