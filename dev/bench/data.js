window.BENCHMARK_DATA = {
  "lastUpdate": 1780771097581,
  "repoUrl": "https://github.com/Chris-Wolfgang/In-memory-Logger",
  "entries": {
    "BenchmarkDotNet": [
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
          "id": "b7a4d6cc3c8243c831e2685f91317ae7c5d7ed25",
          "message": "Merge pull request #98 from Chris-Wolfgang/vNext\n\nRelease v0.1.1: canonical maintenance round + AssemblyVersion fix",
          "timestamp": "2026-06-06T14:35:28-04:00",
          "tree_id": "89517d12f7aa01a40f941649aaf79f3db5529929",
          "url": "https://github.com/Chris-Wolfgang/In-memory-Logger/commit/b7a4d6cc3c8243c831e2685f91317ae7c5d7ed25"
        },
        "date": 1780771097094,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogInformation",
            "value": 191.93756143252054,
            "unit": "ns",
            "range": "± 62.166566193206044"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogBelowMinimum",
            "value": 0,
            "unit": "ns",
            "range": "± 0"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.BeginScopeRoundtrip",
            "value": 58.52710656325022,
            "unit": "ns",
            "range": "± 0.707385146481455"
          },
          {
            "name": "Wolfgang.Extensions.Logging.InMemoryLogger.Benchmarks.InMemoryLoggerBenchmarks.LogEntriesIndexer",
            "value": 7.242848818500836,
            "unit": "ns",
            "range": "± 0.025588675566224635"
          }
        ]
      }
    ]
  }
}