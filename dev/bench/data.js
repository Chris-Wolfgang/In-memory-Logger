window.BENCHMARK_DATA = {
  "lastUpdate": 1780933399785,
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
      }
    ]
  }
}