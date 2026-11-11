# 0001. Pin AssemblyVersion at 1.0.0.0

- Status: Accepted
- Date: 2026-10-20 (recorded retroactively)

## Context

.NET binds strong-name-sensitive references on `AssemblyVersion`. If it changes on every release,
consumers that depend on the library transitively can need binding redirects or a recompile for
each minor or patch bump. The package version still has to tell consumers which release they have.

## Decision

`AssemblyVersion` is pinned at `1.0.0.0` in
`src/Wolfgang.Extensions.Logging.InMemoryLogger/Wolfgang.Extensions.Logging.InMemoryLogger.csproj`.
`FileVersion` is derived from `<Version>` and `InformationalVersion` is derived from `<Version>` by
the SDK, so they carry the actual release number. `AssemblyVersion` is bumped only on a deliberate
breaking API change.

## Consequences

- Consumers can upgrade minor and patch releases without binding redirects.
- Tools that read `AssemblyVersion` to identify a release will see `1.0.0.0` for every build; they
  must read `FileVersion` or `InformationalVersion` instead.
- The pin is a manual act: a breaking change must remember to bump it (see the migration-guide
  convention in `docs/migrations/`).
