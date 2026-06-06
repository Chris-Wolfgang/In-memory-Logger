# Introduction

Welcome to Wolfgang.Extensions.Logging.InMemoryLogger.

## Overview

Wolfgang.Extensions.Logging.InMemoryLogger is a .NET library that provides `ILogger` and `ILogger<T>` implementations which capture log entries in an in-memory buffer. It is designed for unit and integration tests where you need to assert that production code logged the messages you expect.

## Key Features

- **Two logger shapes.** `InMemoryLogger` (non-generic, category passed as a string) and `InMemoryLogger<T>` (generic, category derived from `typeof(T)`) — matches the standard `Microsoft.Extensions.Logging` `ILogger` / `ILogger<T>` pair.
- **Captured entries.** `LogEntries` returns `IReadOnlyList<LogEntry<object>>` from `Microsoft.Extensions.Logging.Abstractions` so the captured entries plug directly into LINQ and xUnit asserts.
- **Scope capture.** `BeginScope` records the supplied state object into the `Scopes` property so scope-aware code paths can be asserted on.
- **Configurable.** Constructor parameters control the minimum log level (defaults to `Trace`) and the initial buffer capacity (defaults to `16`).
- **Thread-safe.** Concurrent `Log` calls append safely.
- **Multi-targeted.** Ships for `net462`, `netstandard2.0`, `netstandard2.1`, `net8.0`, and `net10.0`.

## Getting Help

If you need help with Wolfgang.Extensions.Logging.InMemoryLogger, please:

- Browse the [API Reference](../api/Wolfgang.Extensions.Logging.InMemoryLogger.html)
- Read the [Getting Started](getting-started.md) guide
- File an issue at <https://github.com/Chris-Wolfgang/In-memory-Logger/issues>
