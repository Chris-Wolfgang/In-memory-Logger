# Migrating from X.Y to (X+1).0

Copy this file to `docs/migrations/X-to-X+1.md` during release preparation, fill it in, and link it
from the GitHub Release notes. Delete these instructions.

## Summary

One paragraph: why the major version changed and who is affected.

## Breaking-change inventory

| Area | API / behaviour | Change | Replacement |
|---|---|---|---|
| Public API | `Namespace.Type.Member` | Removed / renamed / signature changed / behaviour changed | What to use instead |

Include every entry from `PublicAPI.Unshipped.txt` that removes or changes a shipped member.

## Before and after

For each row above that needs more than a rename, show the code.

```csharp
// Before (X.Y)

// After ((X+1).0)
```

## Deprecation timeline

| Version | What happened |
|---|---|
| X.Y | Member marked `[Obsolete]` with the replacement named in the message |
| (X+1).0 | Member removed |

## Other changes consumers will notice

Target-framework changes, dependency floor changes and `AssemblyVersion` bump
(see [ADR 0001](../adr/0001-pin-assembly-version.md)).
