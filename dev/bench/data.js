window.BENCHMARK_DATA = {
  "lastUpdate": 1785191097818,
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
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "88071d9cdbd4e4122a2614fc40644bdb56f1cbd6",
          "message": "Merge pull request #162 from Chris-Wolfgang/dependabot/github_actions/github-actions-39b8605068\n\nbuild(deps): bump the github-actions group across 1 directory with 2 updates",
          "timestamp": "2026-06-28T12:07:16-04:00",
          "tree_id": "f49b35d0d1603026db8725342dfc85276e5a494e",
          "url": "https://github.com/Chris-Wolfgang/In-memory-Logger/commit/88071d9cdbd4e4122a2614fc40644bdb56f1cbd6"
        },
        "date": 1782662995151,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogInformation",
            "value": 204.7412760257721,
            "unit": "ns",
            "range": "± 71.07367588795918"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogBelowMinimum",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.BeginScopeRoundtrip",
            "value": 56.5405735373497,
            "unit": "ns",
            "range": "± 0.2355634703240846"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogEntriesIndexer",
            "value": 7.170595208803813,
            "unit": "ns",
            "range": "± 0.009452001506797182"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a0e68f3de1ac6c7b01a959f67fca86a0ba4c38b5",
          "message": "Merge pull request #172 from Chris-Wolfgang/chore/release-v0.1.3\n\nchore: release v0.1.3",
          "timestamp": "2026-07-11T20:55:10-04:00",
          "tree_id": "76f9af3b42d22f7c94160464aba0f5f21f0b8ab4",
          "url": "https://github.com/Chris-Wolfgang/In-memory-Logger/commit/a0e68f3de1ac6c7b01a959f67fca86a0ba4c38b5"
        },
        "date": 1783817881183,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogInformation",
            "value": 206.9873147805532,
            "unit": "ns",
            "range": "± 70.51024667188719"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogBelowMinimum",
            "value": 0.000711208830277125,
            "unit": "ns",
            "range": "± 0.0012318498288316111"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.BeginScopeRoundtrip",
            "value": 59.21414856115977,
            "unit": "ns",
            "range": "± 0.6731660710683712"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogEntriesIndexer",
            "value": 10.466623932123184,
            "unit": "ns",
            "range": "± 0.026182792536803536"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "9028d1f0f1b384fdbe3c5a22639e50ea58711dd8",
          "message": "Merge pull request #177 from Chris-Wolfgang/dependabot/github_actions/github-actions-b93e283e24\n\nbuild(deps): bump the github-actions group across 1 directory with 2 updates",
          "timestamp": "2026-07-27T18:22:15-04:00",
          "tree_id": "ff29b13d772d1d3d41a8edc204f3859f2be62b33",
          "url": "https://github.com/Chris-Wolfgang/In-memory-Logger/commit/9028d1f0f1b384fdbe3c5a22639e50ea58711dd8"
        },
        "date": 1785191096413,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogInformation",
            "value": 206.69237581888834,
            "unit": "ns",
            "range": "± 64.61926812614898"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogBelowMinimum",
            "value": 0.00015994285543759665,
            "unit": "ns",
            "range": "± 0.0002757316272609933"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.BeginScopeRoundtrip",
            "value": 59.16577204068502,
            "unit": "ns",
            "range": "± 0.4443522499593025"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogEntriesIndexer",
            "value": 8.41994939247767,
            "unit": "ns",
            "range": "± 0.192457161812574"
          }
        ]
      }
    ]
  }
}