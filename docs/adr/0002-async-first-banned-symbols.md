# 0002. Ban blocking and legacy APIs with BannedSymbols.txt

- Status: Accepted
- Date: 2026-10-20 (recorded retroactively)

## Context

The library is used from async test and application code. Blocking calls such as `Task.Wait`,
`Task<T>.Result` and `GetAwaiter().GetResult()` can deadlock under a synchronisation context and
hide thread-pool starvation. Other APIs (`BinaryFormatter`, `WebClient`, `DateTime.Now`, synchronous
file I/O) have safer or async replacements.

## Decision

`BannedSymbols.txt` at the repository root lists those APIs, each with the preferred alternative as
its message. `Directory.Build.props` includes it as an `AdditionalFiles` item so the
BannedApiAnalyzers report any use as a build diagnostic, and warnings are treated as errors.

## Consequences

- A banned call fails the build with a message naming the replacement.
- Code that legitimately needs a banned API (for example a test that must block) needs a narrow,
  commented exclusion rather than removing the entry.
- The list must be maintained by hand as new blocking APIs are identified.
