# Verifying the Build

This page explains how to check that a published `Wolfgang.Extensions.Logging.InMemoryLogger`
package was built from the source at its release tag. It describes what has been checked and where
the limits are; it does not promise a bit-identical `.nupkg`.

## What you can and cannot expect

- **The `.nupkg` file itself will never hash the same twice.** A package is a zip, and NuGet stamps
  every entry with its modification time. Compare the *entries* (name, order, content), not the
  container.
- **The assemblies inside should match when you build on the same OS with the same SDK.** The
  SDK compiles deterministically by default, and `Directory.Build.props` sets
  `ContinuousIntegrationBuild` when the `CI` environment variable is `true`. Set `CI=true` for your
  rebuild so source paths are normalised the same way.
- **A different SDK or OS leaves a small residue.** Expect a few dozen bytes of identity fields
  (module version ID, PDB checksum) to differ with identical IL. That is not evidence of tampering.
  Changed source changes IL.
- **Cross-OS byte identity is not guaranteed for this repository.** The SDK writes generated
  sources (`*.AssemblyInfo.cs`) with the OS newline, and the .NET Framework slice (`net462`) is
  compiled against different reference assemblies on Windows and Linux. This repository does not
  normalise either, so verify on the same OS family the release was built on (`ubuntu-latest`,
  see `release.yaml`).
- **The copyright year is part of the assembly.** `Directory.Build.props` builds the `Copyright`
  string from the current UTC year, so a rebuild in a later year differs. Pass the release year
  explicitly (below).

## Procedure

1. Install the SDK versions used by `release.yaml` (the current .NET 10 SDK is enough for the
   `src/` project; the older SDKs listed there are only needed to run the multi-target tests).
2. Check out the release tag in a **git worktree**, not a copied folder. A copy has no `.git`, so
   commit and SourceLink data are missing and the hashes differ for an unrelated reason.

   ```bash
   git clone https://github.com/Chris-Wolfgang/In-memory-Logger.git
   cd In-memory-Logger
   git worktree add ../verify v<version>
   cd ../verify
   ```

3. Build and pack, passing the release year taken from the published assembly's copyright:

   ```bash
   dotnet restore
   dotnet build --no-restore --configuration Release -p:Copyright="Copyright (c) <year> Chris Wolfgang"
   dotnet pack src/Wolfgang.Extensions.Logging.InMemoryLogger --no-build --configuration Release --output ./out
   ```

   If restore fails on a vulnerability advisory published after the release (`NU1902` and similar),
   add `-p:NuGetAudit=false`. The audit depends on today's advisories, not on the tag.

4. Download the published package from NuGet.org and compare **entries** with the one you built.
   NuGet.org adds a `.signature.p7s` entry after publishing, and the `.psmdcp` entry's file name can
   differ between NuGet versions; ignore those two.

   ```bash
   mkdir built published
   unzip -q out/*.nupkg -d built
   unzip -q Wolfgang.Extensions.Logging.InMemoryLogger.<version>.nupkg -d published
   (cd built && find lib -type f -exec sha256sum {} +) | sort -k2 > built.sha256
   (cd published && find lib -type f -exec sha256sum {} +) | sort -k2 > published.sha256
   diff built.sha256 published.sha256 && echo "assemblies match"
   ```

5. Independently of the rebuild, confirm provenance with the attestation described in
   [SECURITY.md](../SECURITY.md#verifying-a-release).

## If something differs

Open an issue (not a security advisory, unless you believe the package was tampered with) with:
the tag, your OS and `dotnet --info` output, the `diff` output, and the entry names that differ. If
only a few bytes per assembly differ and the IL matches, say so; that is the expected residue above.

## Not yet provided

The follow-up items in
[#134](https://github.com/Chris-Wolfgang/In-memory-Logger/issues/134) that are not done here:
a per-release `reproducible-build-manifest.json` published by `release.yaml` (a workflow change,
which needs its own protected-file PR) and a third-party attestation procedure.
