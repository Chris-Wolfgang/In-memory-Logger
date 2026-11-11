# Dependency license baseline

A baseline scan of the library's dependency licenses, measured on `main` at commit `998877c` on 2026-10-22.
It answers two questions: what does the shipped package depend on, and what licenses are in the build closure.

## What the shipped package depends on

Read from the `.nuspec` of a package packed from `main` (`dotnet pack -c Release`):

| Target framework | Declared dependencies |
|---|---|
| `net10.0`, `net8.0`, `netstandard2.1`, `netstandard2.0` | `Microsoft.Extensions.Logging` 10.0.13, `Microsoft.Extensions.Logging.Abstractions` 10.0.13 |
| `net462` | the two above, plus `Microsoft.Extensions.DependencyInjection` 10.0.13 |

All three are MIT. The extra `net462` reference is not used by any source file and is removed under #313; the package
still reaches `Microsoft.Extensions.DependencyInjection` transitively through `Microsoft.Extensions.Logging`.

## Full NuGet closure of the `src` project

The `License Audit` workflow (`.github/workflows/license-audit.yaml`, on every pull request, on push to `main` and
weekly) and `scripts/third-party-notices.ps1` look at the full NuGet closure of the `src` project across all of its
target frameworks, which includes packages used only to build it. Run on `main` with the workflow's own arguments,
the audit **passes** (exit code 0). Every license is on the allow-list in `.github/license-audit/allowed-licenses.json`.

"Build-only" packages are analyzers, reference assemblies and SourceLink (`PrivateAssets="all"` in
`Directory.Build.props` or the csproj, plus their own dependencies); they are not dependencies of the package.

| Package | Version | License | Role |
|---|---|---|---|
| Microsoft.Extensions.Logging | 10.0.13 | MIT | runtime (direct) |
| Microsoft.Extensions.Logging.Abstractions | 10.0.13 | MIT | runtime (direct) |
| Microsoft.Extensions.DependencyInjection | 10.0.13 | MIT | runtime (direct on `net462`, transitive elsewhere) |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.13 | MIT | runtime (transitive) |
| Microsoft.Extensions.Options | 10.0.13 | MIT | runtime (transitive) |
| Microsoft.Extensions.Primitives | 10.0.13 | MIT | runtime (transitive) |
| Microsoft.Bcl.AsyncInterfaces | 10.0.13 | MIT | runtime (transitive, `net462`, `netstandard2.0`) |
| System.Diagnostics.DiagnosticSource | 10.0.13 | MIT | runtime (transitive, `net462`, `netstandard2.x`, `net8.0`) |
| System.Buffers | 4.6.1 | MIT | runtime (transitive, `net462`, `netstandard2.0`) |
| System.ComponentModel.Annotations | 5.0.0 | MIT | runtime (transitive, `netstandard2.x`) |
| System.Memory | 4.6.3 | MIT | runtime (transitive, `net462`, `netstandard2.0`) |
| System.Numerics.Vectors | 4.6.1 | MIT | runtime (transitive, `net462`, `netstandard2.0`) |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 | MIT | runtime (transitive, `net462`, `netstandard2.x`) |
| System.Threading.Tasks.Extensions | 4.6.3 | MIT | runtime (transitive, `net462`, `netstandard2.0`) |
| System.ValueTuple | 4.6.2 | MIT | runtime (transitive, `net462`) |
| NETStandard.Library | 2.0.3 | MIT | platform (`netstandard2.0`) |
| Microsoft.NETCore.Platforms | 1.1.0 | MS-.NET-Library | platform (`netstandard2.0`) |
| AsyncFixer | 2.1.0 | Apache-2.0 | build-only |
| Meziantou.Analyzer | 3.0.303 | MIT | build-only |
| Microsoft.Build.Tasks.Git | 10.0.402 | MIT | build-only |
| Microsoft.CodeAnalysis.BannedApiAnalyzers | 5.6.0 | MIT | build-only |
| Microsoft.CodeAnalysis.PublicApiAnalyzers | 5.6.0 | MIT | build-only |
| Microsoft.NETFramework.ReferenceAssemblies | 1.0.3 | MIT | build-only |
| Microsoft.NETFramework.ReferenceAssemblies.net462 | 1.0.3 | MIT | build-only |
| Microsoft.SourceLink.Common | 10.0.402 | MIT | build-only |
| Microsoft.SourceLink.GitHub | 10.0.402 | MIT | build-only |
| Microsoft.VisualStudio.Threading.Analyzers | 18.12.1 | MIT | build-only |
| Roslynator.Analyzers | 5.0.1 | Apache-2.0 | build-only |
| System.IO.Hashing | 10.0.13 | MIT | build-only (SourceLink) |

`MS-.NET-Library` is the one non-OSI entry the policy allows: the Microsoft .NET Library license, which ships with
some older `System.*` packages and is freely redistributable. `SonarAnalyzer.CSharp` is excluded by
`ignored-packages.json` (a build-time analyzer under the SONAR Source-Available License that is never distributed).
Version numbers move with Dependabot; the license column is what this baseline records.

## Reproduce it

```bash
dotnet tool restore
dotnet restore src/Wolfgang.Extensions.Logging.InMemoryLogger
dotnet nuget-license -i src/Wolfgang.Extensions.Logging.InMemoryLogger/Wolfgang.Extensions.Logging.InMemoryLogger.csproj -t \
  -a .github/license-audit/allowed-licenses.json \
  -ignore .github/license-audit/ignored-packages.json \
  -mapping .github/license-audit/url-license-mappings.json \
  -override .github/license-audit/package-overrides.json \
  -o Table
```

A new package with a license outside the allow-list fails the workflow. Adding a license to
`allowed-licenses.json` is a protected-file change and needs an explicit maintainer decision recorded in the pull
request. Each release also renders the closure to `THIRD-PARTY-NOTICES.md` (`scripts/third-party-notices.ps1`,
run by `release.yaml`), which ships at the root of the package.
