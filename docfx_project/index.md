---
_layout: landing
---

# Wolfgang.Extensions.Logging.InMemoryLogger Documentation

Welcome to the Wolfgang.Extensions.Logging.InMemoryLogger documentation. This site contains guides, API reference, and examples for asserting against `ILogger` / `ILogger<T>` output in your tests.

## Quick Links

- [Getting Started](docs/getting-started.md) - Install + first assertion
- [Introduction](docs/introduction.md) - Library overview and key types
- [API Reference](https://chris-wolfgang.github.io/In-memory-Logger/versions/latest/api/Wolfgang.Extensions.Logging.InMemoryLogger.html) - Complete API documentation
- [GitHub Repository](https://github.com/Chris-Wolfgang/In-memory-Logger) - View source code

## About Wolfgang.Extensions.Logging.InMemoryLogger

`ILogger` and `ILogger<T>` implementations that capture log entries in an in-memory buffer so unit and integration tests can assert against what was logged.

## Installation

```bash
dotnet add package Wolfgang.Extensions.Logging.InMemoryLogger
```

## Documentation Sections

### 📖 [Documentation](docs/getting-started.md)
Step-by-step guides for installation, the generic vs. non-generic logger shapes, scope capture, and registering with the standard `ILoggerFactory`.

### 📚 [API Reference](https://chris-wolfgang.github.io/In-memory-Logger/versions/latest/api/Wolfgang.Extensions.Logging.InMemoryLogger.html)
Complete API documentation automatically generated from the source code's XML comments.

## Additional Resources

- [Contributing Guidelines](https://github.com/Chris-Wolfgang/In-memory-Logger/blob/main/CONTRIBUTING.md)
- [Code of Conduct](https://github.com/Chris-Wolfgang/In-memory-Logger/blob/main/CODE_OF_CONDUCT.md)
- [License](https://github.com/Chris-Wolfgang/In-memory-Logger/blob/main/LICENSE)

---

*Documentation built with [DocFX](https://dotnet.github.io/docfx/)*
