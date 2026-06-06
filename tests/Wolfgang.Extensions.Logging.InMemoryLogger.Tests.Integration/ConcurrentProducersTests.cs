using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Integration;

/// <summary>
/// End-to-end exercises of the InMemoryLogger thread-safety guarantee under
/// genuinely concurrent producers. These are intentionally heavier than the
/// unit tests: they spin up multiple Task.Run producers, await them all, and
/// then assert against the captured LogEntries collection.
///
/// The unit suite covers the per-method contract; this suite covers the
/// emergent behaviour under contention.
/// </summary>
public class ConcurrentProducersTests
{
    [Fact]
    public async Task Log_when_called_concurrently_from_multiple_tasks_captures_every_entry()
    {
        const int producerCount = 16;
        const int entriesPerProducer = 250;
        const int totalExpected = producerCount * entriesPerProducer;

        var sut = new InMemoryLogger
        (
            category: "concurrent.smoke",
            minLogLevel: LogLevel.Trace,
            capacity: totalExpected
        );

        // Fan out: each producer logs its own marker so we can prove no
        // entries were dropped and none were duplicated.
        var producers = Enumerable.Range(0, producerCount)
            .Select(producerId => Task.Run(() =>
            {
                for (var i = 0; i < entriesPerProducer; i++)
                {
                    sut.Log
                    (
                        LogLevel.Information,
                        new EventId(producerId, "concurrent"),
                        $"p{producerId}:{i}",
                        exception: null,
                        formatter: static (s, _) => s
                    );
                }
            }))
            .ToArray();

        await Task.WhenAll(producers);

        Assert.Equal(totalExpected, sut.LogEntries.Count);

        // Every producer/index pair should appear exactly once. If the
        // underlying buffer dropped or duplicated, the distinct-count would
        // diverge from totalExpected.
        var distinctMarkers = sut.LogEntries
            .Select(e => e.Formatter(e.State, e.Exception))
            .Distinct()
            .Count();

        Assert.Equal(totalExpected, distinctMarkers);
    }


    [Fact]
    public async Task Log_when_called_concurrently_each_producer_emits_all_of_its_own_entries()
    {
        const int producerCount = 8;
        const int entriesPerProducer = 100;

        var sut = new InMemoryLogger
        (
            category: "concurrent.per-producer",
            minLogLevel: LogLevel.Trace,
            capacity: producerCount * entriesPerProducer
        );

        var producers = Enumerable.Range(0, producerCount)
            .Select(producerId => Task.Run(() =>
            {
                for (var i = 0; i < entriesPerProducer; i++)
                {
                    sut.Log
                    (
                        LogLevel.Information,
                        new EventId(producerId, "p"),
                        $"p{producerId}:{i}",
                        exception: null,
                        formatter: static (s, _) => s
                    );
                }
            }))
            .ToArray();

        await Task.WhenAll(producers);

        // Bucket entries by producer (EventId.Id) and verify each bucket has
        // exactly entriesPerProducer items. This proves the buffer is
        // genuinely thread-safe and not just appearing to be by accident of
        // scheduling.
        var byProducer = sut.LogEntries
            .GroupBy(e => e.EventId.Id)
            .ToDictionary(g => g.Key, g => g.Count());

        Assert.Equal(producerCount, byProducer.Count);

        // KeyValuePair<TKey,TValue> on the .NET Framework TFMs (net462–net481)
        // doesn't have a built-in Deconstruct extension, so we iterate without
        // tuple deconstruction here.
        foreach (var kvp in byProducer)
        {
            Assert.Equal(entriesPerProducer, kvp.Value);
        }
    }


    [Fact]
    public async Task LogEntries_when_read_during_active_writes_does_not_throw()
    {
        // The contract: LogEntries is safe to enumerate while another thread
        // is still appending. We don't pin a specific snapshot count — that
        // depends on scheduling — but the snapshot must not throw and must
        // be a valid prefix of the eventual log.
        var sut = new InMemoryLogger
        (
            category: "concurrent.snapshot",
            minLogLevel: LogLevel.Trace,
            capacity: 10_000
        );

        using var cts = new System.Threading.CancellationTokenSource();
        var producer = Task.Run(() =>
        {
            var i = 0;
            while (!cts.IsCancellationRequested)
            {
                sut.Log(LogLevel.Information, new EventId(0), i++, null, (s, _) => s.ToString()!);
            }
        });

        // Take several snapshots while the producer is going.
        var snapshots = new ConcurrentBag<int>();
        for (var i = 0; i < 50; i++)
        {
            // Enumerate via .Count and .ElementAt — both routes the analyzer
            // tools warn would race if the underlying collection were not
            // thread-safe.
            var snapshot = sut.LogEntries;
            snapshots.Add(snapshot.Count);
            if (snapshot.Count > 0)
            {
                _ = snapshot[snapshot.Count - 1];
            }
            await Task.Delay(1);
        }

        cts.Cancel();
        await producer;

        // No exception is the assertion. (xunit would surface the throw.)
        Assert.NotEmpty(snapshots);
    }
}
