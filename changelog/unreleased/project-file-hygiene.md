type: fix

The package no longer declares an unused `Microsoft.Extensions.DependencyInjection` dependency on `net462` (it still arrives transitively through `Microsoft.Extensions.Logging`), no longer asks consumers to accept the MIT licence on install, gains NuGet search tags, and its project URL uses the repository's real name (`In-memory-Logger`).
