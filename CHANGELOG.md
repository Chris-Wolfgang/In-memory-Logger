# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.3] - 2026-07-12

### Changed

- Dependabot bump: dotnet-dependencies group (4 packages).

## [0.1.2] - 2026-06-06

### Fixed

- `release.yaml`: removed a duplicate `verify-docs-build` job introduced
  during the vNext merge that made the workflow YAML invalid and
  prevented the v0.1.1 publish from firing on NuGet.org. Releasing as
  0.1.2 to avoid republishing under the 0.1.1 tag.

## [0.1.1] - 2026-06-06

Canonical maintenance round: PublicAPI baseline, AssemblyVersion pinning,
benchmarks scaffold, integration test project, CHANGELOG creation,
docfx D8 inline version picker, Stryker config, and the standard
template-sync follow-ups. See PR
[#98](https://github.com/Chris-Wolfgang/In-memory-Logger/pull/98) for
the full set of `Closes #N` references.

Note: v0.1.1 was tagged and a GitHub Release was published, but the
`publish-nuget` job never ran because of the workflow bug fixed in
0.1.2. No `0.1.1` package exists on NuGet.org — install `0.1.2` or
later.

## [0.1.0] - 2026-05-02

Initial release: `InMemoryLogger` (`ILogger` with a category string) and
`InMemoryLogger<T>` (`ILogger<T>`, category from `typeof(T)`), each
writing entries to a thread-safe in-memory collection with a configurable
minimum log level and initial capacity, plus scope capture via
`BeginScope`; `InMemoryLoggerProvider` and the `AddInMemoryLogger`
`ILoggingBuilder` extension for use through `ILoggerFactory` / DI.
Targets net462, netstandard2.0, netstandard2.1, net8.0 and net10.0.

[Unreleased]: https://github.com/Chris-Wolfgang/In-memory-Logger/compare/v0.1.3...HEAD
[0.1.3]: https://github.com/Chris-Wolfgang/In-memory-Logger/compare/v0.1.2...v0.1.3
[0.1.2]: https://github.com/Chris-Wolfgang/In-memory-Logger/compare/v0.1.1...v0.1.2
[0.1.1]: https://github.com/Chris-Wolfgang/In-memory-Logger/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/Chris-Wolfgang/In-memory-Logger/releases/tag/v0.1.0
