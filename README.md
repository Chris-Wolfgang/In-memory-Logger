# Wolfgang.Extensions.Logging.InMemoryLogger

An implementation of `ILogger` and `ILogger<T>` from `Microsoft.Extensions.Logging` that writes log entries to an in-memory collection. Designed for unit and integration tests where you need to assert against logged messages.

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.Logging.InMemoryLogger.svg?logo=nuget&label=NuGet)](https://www.nuget.org/packages/Wolfgang.Extensions.Logging.InMemoryLogger)
[![NuGet downloads](https://img.shields.io/nuget/dt/Wolfgang.Extensions.Logging.InMemoryLogger.svg?logo=nuget&label=downloads)](https://www.nuget.org/packages/Wolfgang.Extensions.Logging.InMemoryLogger)
[![PR build](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/In-memory-Logger/pr.yaml?event=pull_request_target&label=PR%20build&logo=github)](https://github.com/Chris-Wolfgang/In-memory-Logger/actions/workflows/pr.yaml)
[![Release](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/In-memory-Logger/release.yaml?label=release&logo=github)](https://github.com/Chris-Wolfgang/In-memory-Logger/actions/workflows/release.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/In-memory-Logger)

---

## 📦 Installation

```bash
dotnet add package Wolfgang.Extensions.Logging.InMemoryLogger
```

**NuGet Package:** [Wolfgang.Extensions.Logging.InMemoryLogger](https://www.nuget.org/packages/Wolfgang.Extensions.Logging.InMemoryLogger)

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/In-memory-Logger](https://github.com/Chris-Wolfgang/In-memory-Logger)
- **API Documentation:** https://Chris-Wolfgang.github.io/In-memory-Logger/
- **CHANGELOG:** [CHANGELOG.md](CHANGELOG.md)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)
- **DocFX Version Picker Troubleshooting:** [docs/DOCFX-VERSION-PICKER.md](docs/DOCFX-VERSION-PICKER.md)

---

## ✨ Features

- Thread-safe in-memory log entry storage.
- Configurable minimum log level (passed to the constructor; defaults to `LogLevel.Trace`).
- Configurable initial capacity (defaults to `16`).
- Supports all .NET log levels (`Trace` through `Critical`).
- Captures scope state via `BeginScope` for assertion in tests.
- Returns log entries as `IReadOnlyList<LogEntry<object>>` for easy LINQ + assertion patterns.
- Ships both a non-generic `InMemoryLogger` (constructed with a category string) and a generic `InMemoryLogger<T>` (category derived from `typeof(T)`), matching the standard `ILogger` / `ILogger<T>` shape.

---

## 🚀 Quick Start

```csharp
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.InMemoryLogger;

// Generic form — category is "MyApp.MyService"
var logger = new InMemoryLogger<MyService>(LogLevel.Information);

// Use the logger in code under test
var service = new MyService(logger);
service.DoWork();

// Assert against captured log entries
Assert.Single(logger.LogEntries);
Assert.Equal(LogLevel.Information, logger.LogEntries[0].LogLevel);
```

The non-generic form takes a category string directly:

```csharp
var logger = new InMemoryLogger("MyCategory", LogLevel.Warning, capacity: 32);
```

---

## 🎯 Target Frameworks

| TFM | Status |
|---|---|
| `net462` | ✅ |
| `netstandard2.0` | ✅ |
| `netstandard2.1` | ✅ |
| `net8.0` | ✅ |
| `net10.0` | ✅ |

---

## 🔍 Code Quality & Static Analysis

This project is held to the canonical analyzer set used across all `Chris-Wolfgang` .NET libraries. Analyzers run on every build and are treated as errors in `Release`.

### Analyzers in Use

1. **Microsoft.CodeAnalysis.NetAnalyzers** — built-in .NET analyzers (correctness and performance)
2. **Roslynator.Analyzers** — refactoring and code quality
3. **AsyncFixer** — async/await best practices
4. **Microsoft.VisualStudio.Threading.Analyzers** — thread safety
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** — enforces the `BannedSymbols.txt` policy
6. **Meziantou.Analyzer** — broad code-quality rules
7. **SonarAnalyzer.CSharp** — industry-standard analysis
8. **Microsoft.CodeAnalysis.PublicApiAnalyzers** — public-API surface tracking via `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt`

### Banned-API policy

`BannedSymbols.txt` is the canonical fleet baseline. Banned categories include blocking sync-over-async (`Task.Result`, `Task.Wait()`), `Thread.Sleep`, synchronous file I/O, and several legacy / deprecated APIs.

---

## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Code quality standards
- Build and test instructions
- Pull request guidelines
- Analyzer configuration details
