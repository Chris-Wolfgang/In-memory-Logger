using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Integration;

/// <summary>
/// Stress tests for the logger's shared mutable state, run on dedicated threads released together by
/// a <see cref="Barrier"/> so the calls genuinely overlap. Assertions are structural (nothing lost,
/// nothing duplicated, one logger per category, per-thread scope stacks never leak), so they do not
/// depend on timing. <see cref="ConcurrentProducersTests"/> covers Task-based producers and readers.
/// </summary>
/// <remarks>
/// Systematic schedule exploration (Coyote) and day-long soak runs are deliberately not used. The
/// shared state is one <see cref="List{T}"/> behind a single lock, a <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>
/// in the provider and an <see cref="AsyncLocal{T}"/> scope stack; there are no awaits inside the
/// library, no handles and no background threads, so there are no interleavings a model checker
/// would find that these overlapping-call tests do not exercise, and nothing for a soak run to leak.
/// </remarks>
public class ConcurrencyTests
{
    private const int Threads = 8;

    private const int CallsPerThread = 2_000;



    [Fact]
    public async Task Log_when_called_from_dedicated_threads_at_once_keeps_every_entry_once_and_in_per_thread_order()
    {
        var sut = new InMemoryLogger("stress.log", LogLevel.Trace, capacity: 1);

        await RunOnDedicatedThreadsAsync
        (
            thread =>
            {
                for (var i = 0; i < CallsPerThread; i++)
                {
                    sut.Log(LogLevel.Information, new EventId(thread * CallsPerThread + i), thread, exception: null, static (_, _) => string.Empty);
                }
            }
        );

        var entries = sut.LogEntries;
        Assert.Equal(Threads * CallsPerThread, entries.Count);
        Assert.Equal(Threads * CallsPerThread, entries.Select(e => e.EventId.Id).Distinct().Count());
        for (var thread = 0; thread < Threads; thread++)
        {
            var ids = entries.Where(e => (int)e.State == thread).Select(e => e.EventId.Id).ToArray();
            Assert.Equal(Enumerable.Range(thread * CallsPerThread, CallsPerThread), ids);
        }
    }



    [Fact]
    public async Task CreateLogger_when_called_for_the_same_categories_from_dedicated_threads_returns_one_logger_per_category()
    {
        using var sut = new InMemoryLoggerProvider();
        var seen = new ILogger[Threads][];

        await RunOnDedicatedThreadsAsync
        (
            thread =>
            {
                seen[thread] = Enumerable.Range(0, 16).Select(c => sut.CreateLogger("category-" + c)).ToArray();
                foreach (var logger in seen[thread])
                {
                    logger.LogWarning("from {Thread}", thread);
                }
            }
        );

        Assert.Equal(16, sut.Loggers.Count);
        for (var c = 0; c < 16; c++)
        {
            var first = seen[0][c];
            Assert.All(seen, perThread => Assert.Same(first, perThread[c]));
        }

        Assert.Equal(Threads * 16, sut.LogEntries.Count);
    }



    [Fact]
    public async Task BeginScope_when_used_from_dedicated_threads_at_once_keeps_each_thread_s_stack_separate()
    {
        var sut = new InMemoryLogger("stress.scopes");
        var mismatches = 0;

        await RunOnDedicatedThreadsAsync
        (
            thread =>
            {
                for (var i = 0; i < CallsPerThread; i++)
                {
                    using (sut.BeginScope(thread))
                    using (sut.BeginScope(i))
                    {
                        var scopes = sut.Scopes;
                        // One counted statement, not a branch that only runs when the test fails.
                        Interlocked.Add(ref mismatches, scopes.Length == 2 && Equals(scopes[0], thread) && Equals(scopes[1], i) ? 0 : 1);
                    }
                }

                Interlocked.Add(ref mismatches, sut.Scopes.Length);
            }
        );

        Assert.Equal(0, mismatches);
        Assert.Empty(sut.Scopes);
    }



    // Starts Threads dedicated threads, releases them together with a barrier, and completes when all
    // have finished; an exception on any thread faults the returned task.
    private static async Task RunOnDedicatedThreadsAsync(Action<int> body)
    {
        using var barrier = new Barrier(Threads);
        var tasks = Enumerable.Range(0, Threads).Select
        (
            thread => Task.Factory.StartNew
            (
                () =>
                {
                    barrier.SignalAndWait();
                    body(thread);
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            )
        ).ToArray();

        await Task.WhenAll(tasks);
    }
}
