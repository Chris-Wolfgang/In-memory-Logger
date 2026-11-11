using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

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
    // Shared by the producer tests that log a plain string state.
    private static string Identity(string state, Exception? _) => state;



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
                        formatter: Identity
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
                        formatter: Identity
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
    public async Task LogEntries_when_read_during_active_writes_returns_growing_prefixes_of_the_log()
    {
        // The contract: LogEntries is safe to read while another thread is
        // still appending, and each snapshot is a prefix of the eventual log.
        // We don't pin a specific snapshot count — that depends on scheduling —
        // but successive snapshots can never shrink, each one's entries are the
        // first N entries in order, and the final log holds at least as many.
        const int total = 20_000;
        var sut = new InMemoryLogger
        (
            category: "concurrent.snapshot",
            minLogLevel: LogLevel.Trace,
            capacity: 16
        );

        // A bounded producer: the reader below races it, but the test's cost does not
        // depend on how fast the machine is.
        var producer = Task.Run(() =>
        {
            for (var i = 0; i < total; i++)
            {
                sut.Log(LogLevel.Information, new EventId(i), i, null, static (s, _) => s.ToString(CultureInfo.InvariantCulture));
            }
        });

        // Take snapshots while the producer is going. Each one is read through .Count and
        // the indexer, the routes that would race if the underlying collection were not
        // copied under the lock, and sampled at up to 17 positions: entry n of every
        // snapshot must be the n-th value the producer logged.
        var counts = new List<int>();
        var outOfOrder = 0;
        for (var i = 0; i < 50; i++)
        {
            var snapshot = sut.LogEntries;
            counts.Add(snapshot.Count);
            for (var n = 0; n < snapshot.Count; n += Math.Max(1, snapshot.Count / 16))
            {
                outOfOrder += snapshot[n].EventId.Id == n ? 0 : 1;
            }

            await Task.Delay(1);
        }

        await producer;

        var final = sut.LogEntries;

        Assert.Equal(0, outOfOrder);
        Assert.Equal(counts.OrderBy(c => c), counts);
        Assert.Equal(total, final.Count);
        Assert.Equal(Enumerable.Range(0, total), final.Select(e => e.EventId.Id));
    }
}
