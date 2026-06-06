# Copilot Instructions for Wolfgang.Extensions.Logging.InMemoryLogger

## Project Overview
- **Package:** Wolfgang.Extensions.Logging.InMemoryLogger
- **Namespace:** Wolfgang.Extensions.Logging.InMemoryLogger
- **Purpose:** In-memory `ILogger` / `ILogger<T>` implementation for asserting against logged messages in unit and integration tests.

## Key Types
- `InMemoryLogger` — non-generic logger; takes a category string in its constructor.
- `InMemoryLogger<T>` — generic logger; category derived from `typeof(T)`.
- `InMemoryLoggerProvider` — `ILoggerProvider` that hands out `InMemoryLogger` instances per category.
- `InMemoryLoggerBuilderExtensions` — `AddInMemoryLogger` extension for `ILoggingBuilder` registration.

## Public surface (per `PublicAPI.Shipped.txt`)
- Constructor: `InMemoryLogger(string category, LogLevel minLogLevel = Trace, int capacity = 16)`
- Constructor: `InMemoryLogger<T>(LogLevel minLogLevel = Trace, int capacity = 16)`
- Properties: `Category`, `MinimumLogLevel`, `Capacity`, `LogEntries`, `Scopes`
- Methods: `Log<TState>(...)`, `IsEnabled(LogLevel)`, `BeginScope<TState>(TState)`

## Important Notes
- Thread-safe append into the internal log-entry buffer.
- `LogEntries` returns `IReadOnlyList<LogEntry<object>>` from `Microsoft.Extensions.Logging.Abstractions` — usable directly with LINQ + xUnit asserts.
- `Scopes` captures objects passed to `BeginScope` for scope-aware assertions.
- Public API is tracked by `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` — additions surface as RS0016 at compile time.

## Code Style
- Allman brace style
- 3 blank lines between members
- File-scoped namespaces
- Warnings as errors in Release builds
- `var` when the type is obvious from the right-hand side

## Target Frameworks
- net462, netstandard2.0, netstandard2.1, net8.0, net10.0

## Build / Test
- `dotnet restore`
- `dotnet build -c Release`
- `dotnet test -c Release --no-build`
- CI gates: 90% line coverage threshold, DevSkim security scan, gitleaks secrets scan, CodeQL security-extended.

## Release
- Bump `<Version>` in `src/Wolfgang.Extensions.Logging.InMemoryLogger/Wolfgang.Extensions.Logging.InMemoryLogger.csproj`.
- Publish a GitHub Release with the matching `v<version>` tag — `release.yaml` validates the tag against the csproj, runs multi-TFM tests, packs the .nupkg + .snupkg, publishes to NuGet, and deploys the versioned docs site.
