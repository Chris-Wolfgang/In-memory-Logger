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
  normalise either, so verify on the same OS family the release was built on. The package is
  packed by the `pack-and-validate` job of `release.yaml`, which runs on `windows-latest`; the
  `runnerOs` field of the release's `reproducible-build-manifest.json` (below) records it.
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

5. Instead of (or as well as) downloading the published package, compare against the expected
   hashes in the release's `reproducible-build-manifest.json` (see below):

   ```bash
   jq -r '.packages[] | select(.package == "Wolfgang.Extensions.Logging.InMemoryLogger.<version>.nupkg")
          | .entries[] | "\(.sha256)  \(.path)"' reproducible-build-manifest.json | sort -k2 > expected.sha256
   diff built.sha256 expected.sha256 && echo "assemblies match the release manifest"
   ```

6. Independently of the rebuild, confirm provenance with the attestation described in
   [SECURITY.md](../SECURITY.md#verifying-a-release).

## If something differs

Open an issue (not a security advisory, unless you believe the package was tampered with) with:
the tag, your OS and `dotnet --info` output, the `diff` output, and the entry names that differ. If
only a few bytes per assembly differ and the IL matches, say so; that is the expected residue above.

## Expected hashes: `reproducible-build-manifest.json`

Every GitHub Release made after the manifest step was added to `release.yaml` carries a `reproducible-build-manifest.json`
asset, written by the `pack-and-validate` job of `release.yaml` from the packages it just packed.
It is covered by the same build-provenance attestation as the `.nupkg` files, so check it first:

```bash
gh attestation verify reproducible-build-manifest.json --repo Chris-Wolfgang/In-memory-Logger \
  --signer-workflow Chris-Wolfgang/In-memory-Logger/.github/workflows/release.yaml
```

Fields:

| Field | Meaning |
|---|---|
| `schemaVersion` | `1` |
| `repository`, `tag`, `commit` | what was built: the repository, the release tag and the exact commit SHA |
| `runnerOs` | the OS the package was packed on (`Windows`); rebuild on the same OS family |
| `sdk` | `dotnet --version` on that runner; use the same SDK for a byte-identical rebuild |
| `packages[].package` | the `.nupkg` file name |
| `packages[].entries[]` | `path` and `sha256` (lower-case hex) of every non-empty `lib/` entry in that package |

Only `lib/` entries are listed because the container and its metadata entries (`.nuspec`,
`.psmdcp`, `[Content_Types].xml`) carry timestamps and NuGet-version details that never reproduce;
the assemblies and XML documentation files under `lib/` are what you run.

## Publishing a verification attestation

If your rebuild matches, you can publish that result so others can rely on it without rebuilding
themselves. This follows the [Reproducible Builds](https://reproducible-builds.org/) convention of
independent rebuilders publishing signed statements about what they reproduced.

1. Write a statement of what you checked, for example `verification.json`:

   ```json
   {
     "repository": "Chris-Wolfgang/In-memory-Logger",
     "tag": "v<version>",
     "commit": "<commit from the manifest>",
     "result": "reproduced",
     "rebuilder": "<your name or organisation>",
     "environment": "<OS, dotnet --version>",
     "entries": "<the contents of built.sha256>"
   }
   ```

2. Sign it with your own identity. Keyless [Sigstore](https://www.sigstore.dev/) signing needs no
   key management:

   ```bash
   cosign sign-blob verification.json --bundle verification.sigstore.json
   ```

   Any signature that others can check against an identity you publish (a GPG key, an SSH key) is
   fine too.

3. Open an issue in this repository titled `Reproducibility verification: v<version>` and attach
   `verification.json` and the signature bundle (or link to where you host them). A failed or
   partial reproduction is useful too: set `result` to `differs` and include the `diff` output, as
   described in [If something differs](#if-something-differs).

Anyone can then check your statement with
`cosign verify-blob verification.json --bundle verification.sigstore.json --certificate-identity <your identity> --certificate-oidc-issuer <your issuer>`.
