window.BENCHMARK_DATA = {
  "lastUpdate": 1781814168930,
  "repoUrl": "https://github.com/Chris-Wolfgang/In-memory-Logger",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "id": "ad3b8eb04cbeb50a71496f81c87d254b41e23cfe",
          "message": "docs(docfx): replace B/W halo with logo's purple glow\n\nThe W's perceived color drifted because the navbar logo has a purple\nglow (feDropShadow flood-color=#7c23bb opacity=0.5 in logo.svg) that\nadds luminance around the glyph, while the favicon was using B/W halos\nthat don't contribute purple to the perceived color.\n\nSwitching to the SAME purple glow logo.svg uses:\n  filter: drop-shadow(0 0 2px rgba(124,35,187,0.5));\n\nEquivalent in CSS to logo.svg's\n  feDropShadow dx='0' dy='0' stdDeviation='2'\n               flood-color='#7c23bb' flood-opacity='0.5'\n\nDrops the prefers-color-scheme media query — same glow on light and\ndark tabs, matching logo.svg's behavior on its dark navbar background.\n\nCo-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>",
          "timestamp": "2026-06-08T12:57:21Z",
          "url": "https://github.com/Chris-Wolfgang/In-memory-Logger/commit/ad3b8eb04cbeb50a71496f81c87d254b41e23cfe"
        },
        "date": 1780933397688,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogInformation",
            "value": 190.69174655278525,
            "unit": "ns",
            "range": "± 61.21306733136298"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogBelowMinimum",
            "value": 0.0005764458328485489,
            "unit": "ns",
            "range": "± 0.000616846100802947"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.BeginScopeRoundtrip",
            "value": 58.9481870730718,
            "unit": "ns",
            "range": "± 2.450267150224334"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogEntriesIndexer",
            "value": 7.196489120523135,
            "unit": "ns",
            "range": "± 0.0052951859891312986"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "9b05722970bbf71a3a5f2ad0d5765afb47765cbe",
          "message": "Bump the dotnet-dependencies group with 4 updates\n\nBumps Meziantou.Analyzer from 3.0.101 to 3.0.104\nBumps Microsoft.Extensions.DependencyInjection from 10.0.8 to 10.0.9\nBumps Microsoft.Extensions.Logging from 10.0.8 to 10.0.9\nBumps Microsoft.Extensions.Logging.Abstractions from 10.0.8 to 10.0.9\n\n---\nupdated-dependencies:\n- dependency-name: Meziantou.Analyzer\n  dependency-version: 3.0.104\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.DependencyInjection\n  dependency-version: 10.0.9\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging\n  dependency-version: 10.0.9\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging.Abstractions\n  dependency-version: 10.0.9\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>",
          "timestamp": "2026-06-18T16:17:15-04:00",
          "tree_id": "380b1d6809107605f36557afeb645d5a19035586",
          "url": "https://github.com/Chris-Wolfgang/In-memory-Logger/commit/9b05722970bbf71a3a5f2ad0d5765afb47765cbe"
        },
        "date": 1781814000056,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogInformation",
            "value": 198.78063686688742,
            "unit": "ns",
            "range": "± 67.82384643593944"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogBelowMinimum",
            "value": 0.00012584403157234192,
            "unit": "ns",
            "range": "± 0.00020359778948166187"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.BeginScopeRoundtrip",
            "value": 54.27594084541003,
            "unit": "ns",
            "range": "± 0.2728857410616078"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogEntriesIndexer",
            "value": 7.16002660493056,
            "unit": "ns",
            "range": "± 0.006332970832200923"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "914fb80783e11e095df0451043c121af6cbdb8d7",
          "message": "Bump Meziantou.Analyzer and 2 others\n\nBumps Meziantou.Analyzer from 3.0.101 to 3.0.104\nBumps Microsoft.Extensions.DependencyInjection from 10.0.8 to 10.0.9\nBumps Microsoft.Extensions.Logging from 10.0.8 to 10.0.9\n\n---\nupdated-dependencies:\n- dependency-name: Meziantou.Analyzer\n  dependency-version: 3.0.104\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.DependencyInjection\n  dependency-version: 10.0.9\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.DependencyInjection\n  dependency-version: 10.0.9\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.DependencyInjection\n  dependency-version: 10.0.9\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.Extensions.Logging\n  dependency-version: 10.0.9\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>",
          "timestamp": "2026-06-18T16:17:58-04:00",
          "tree_id": "6cbcdbc230db7b6c7d7f9315f13f7c7a70f456fd",
          "url": "https://github.com/Chris-Wolfgang/In-memory-Logger/commit/914fb80783e11e095df0451043c121af6cbdb8d7"
        },
        "date": 1781814167601,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogInformation",
            "value": 207.12099838256836,
            "unit": "ns",
            "range": "± 65.74816565131876"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogBelowMinimum",
            "value": 0.0007466400663057963,
            "unit": "ns",
            "range": "± 0.0012932185298082347"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.BeginScopeRoundtrip",
            "value": 60.48571294546127,
            "unit": "ns",
            "range": "± 0.14506896314849943"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogEntriesIndexer",
            "value": 8.162947431206703,
            "unit": "ns",
            "range": "± 0.02518004481164418"
          }
        ]
      }
    ]
  }
}