using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.InMemoryLogger;
using Wolfgang.Extensions.Logging.InMemoryLogger.Tests.AotSmoke;

// Native AOT smoke consumer. Calls every public member of InMemoryLogger, InMemoryLogger<T>,
// InMemoryLoggerProvider and AddInMemoryLogger once, through the M.E.Logging extension methods a
// real consumer uses (LogInformation, BeginScope, LoggerFactory.Create), so the trimmer keeps
// every generic instantiation the library needs and a fault on any path fails the workflow's run
// step. Each check prints the failing case before the process exits with code 1.

var failures = 0;

// InMemoryLogger: constructor, Log via the extension methods, IsEnabled, scopes, properties.
var logger = new InMemoryLogger("aot.category", LogLevel.Information, capacity: 4);
logger.LogDebug("filtered");
logger.LogInformation("Hello {Name}", "AOT");
logger.Log(LogLevel.Error, new EventId(7, "seven"), new Payload(42), new InvalidOperationException("boom"), static (p, _) => $"payload {p.Value}");
Check("LogEntries.Count", logger.LogEntries.Count == 2);
Check("formatted message", string.Equals(logger.LogEntries[0].Formatter(logger.LogEntries[0].State, null), "Hello AOT", StringComparison.Ordinal));
Check("custom state + formatter", string.Equals(logger.LogEntries[1].Formatter(logger.LogEntries[1].State, logger.LogEntries[1].Exception), "payload 42", StringComparison.Ordinal));
Check("exception captured", logger.LogEntries[1].Exception is InvalidOperationException);
Check("IsEnabled(Debug) false", !logger.IsEnabled(LogLevel.Debug));
Check("IsEnabled(Error) true", logger.IsEnabled(LogLevel.Error));
Check("Category", string.Equals(logger.Category, "aot.category", StringComparison.Ordinal));
Check("MinimumLogLevel", logger.MinimumLogLevel == LogLevel.Information);
Check("Capacity", logger.Capacity >= 4);
using (logger.BeginScope("outer"))
using (logger.BeginScope(new Payload(1)))
{
    Check("Scopes nested", logger.Scopes.Length == 2 && Equals(logger.Scopes[0], "outer"));
}

Check("Scopes empty after dispose", logger.Scopes.Length == 0);

// InMemoryLogger<T>
var typed = new InMemoryLogger<Payload>(LogLevel.Trace, capacity: 2);
typed.LogTrace("typed");
Check("InMemoryLogger<T>.LogEntries", typed.LogEntries.Count == 1 && typed.IsEnabled(LogLevel.Trace));
Check("InMemoryLogger<T> category", string.Equals(typed.LogEntries[0].Category, typeof(Payload).FullName, StringComparison.Ordinal));
using (typed.BeginScope(5))
{
    Check("InMemoryLogger<T>.Scopes", typed.Scopes.Length == 1);
}

Check("InMemoryLogger<T> properties", typed.MinimumLogLevel == LogLevel.Trace && typed.Capacity >= 2);

// InMemoryLoggerProvider + AddInMemoryLogger through LoggerFactory.
var provider = new InMemoryLoggerProvider(LogLevel.Warning);
using (var factory = LoggerFactory.Create(builder => builder.AddInMemoryLogger(provider)))
{
    factory.CreateLogger<Payload>().LogWarning("from the factory");
    factory.CreateLogger("other").LogInformation("filtered by the provider level");
}

Check("provider LogEntries", provider.LogEntries.Count == 1);
Check("provider Loggers", provider.Loggers.Count == 2 && provider.Loggers.Keys.Contains("other"));
Check("provider CreateLogger reuses", ReferenceEquals(provider.CreateLogger("other"), provider.Loggers["other"]));
provider.Dispose();

// The argument-null paths: constructing ArgumentNullException is a classic trim casualty.
try
{
    _ = new InMemoryLogger(null!);
    Fail("InMemoryLogger(null) did not throw");
}
catch (ArgumentNullException ex) when (string.Equals(ex.ParamName, "category", StringComparison.Ordinal))
{
    // expected
}

if (failures != 0)
{
    return 1;
}

Console.WriteLine("AOT smoke OK.");
return 0;



void Check(string label, bool ok)
{
    if (!ok)
    {
        Fail(label);
    }
}



void Fail(string message)
{
    Console.Error.WriteLine($"AOT smoke FAILED: {message}");
    failures++;
}



namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.AotSmoke
{
    internal sealed record Payload(int Value);
}
