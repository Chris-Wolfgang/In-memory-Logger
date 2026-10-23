#if NET5_0_OR_GREATER
using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Unit;

/// <summary>
/// Pins the call shapes that are documented to allocate nothing, measured with
/// <see cref="GC.GetAllocatedBytesForCurrentThread"/> around a loop of calls after a warm-up call
/// (which pays for JIT, static initialisation and the cached formatter adapter) and a full warm-up
/// run of the measured loop:
/// <list type="bullet">
///   <item><description><c>IsEnabled</c> for any level;</description></item>
///   <item><description><c>Log</c> below <c>MinimumLogLevel</c> (early return);</description></item>
///   <item><description><c>Log</c> above it with a reused formatter into a buffer that still has
///   room (the entry is a struct and the formatter adapter is cached per formatter);</description></item>
///   <item><description><c>Scopes</c> when no scope is open (returns <see cref="Array.Empty{T}"/>).</description></item>
/// </list>
/// <c>BeginScope</c> (a scope node and its handle per call), <c>LogEntries</c> (a snapshot array)
/// and a <c>Log</c> that grows the buffer allocate by design and are not asserted here.
/// </summary>
/// <remarks>
/// Compiled only for net5.0+: <see cref="GC.GetAllocatedBytesForCurrentThread"/> does not exist on
/// .NET Framework, and the measured paths are the same code on every target framework.
/// </remarks>
public class ZeroAllocationTests
{
    private const int Calls = 1_000;

    private static readonly Func<string, Exception?, string> Formatter = static (s, _) => s;

    // The formatter-adapter cache is one static slot per state type, shared by every test running in
    // parallel. The enabled-Log test logs a state type nothing else uses, so no other test's formatter
    // can replace its cached adapter in the middle of the measured loop.
    private static readonly AllocationProbe ProbeState = new();

    private static readonly Func<AllocationProbe, Exception?, string> ProbeFormatter = static (_, _) => "probe";



    [Fact]
    public void IsEnabled_when_called_allocates_nothing()
    {
        var sut = new InMemoryLogger("alloc", LogLevel.Warning);
        _ = sut.IsEnabled(LogLevel.Error);

        var allocated = Measure(() =>
        {
            for (var i = 0; i < Calls; i++)
            {
                _ = sut.IsEnabled((LogLevel)(i % 7));
            }
        });

        AssertNoPerCallAllocation(allocated);
    }



    [Fact]
    public void Log_when_level_is_below_minimum_allocates_nothing()
    {
        var sut = new InMemoryLogger("alloc", LogLevel.Critical);
        sut.Log(LogLevel.Information, default, "warm-up", null, Formatter);

        var allocated = Measure(() =>
        {
            for (var i = 0; i < Calls; i++)
            {
                sut.Log(LogLevel.Information, default, "filtered", null, Formatter);
            }
        });

        AssertNoPerCallAllocation(allocated);
        Assert.Empty(sut.LogEntries);
    }



    [Fact]
    public void Log_when_enabled_with_a_reused_formatter_and_room_in_the_buffer_allocates_nothing()
    {
        // Room for the warm-up call plus both runs of the loop, so the buffer never grows.
        var sut = new InMemoryLogger("alloc", LogLevel.Trace, capacity: (2 * Calls) + 1);
        sut.Log(LogLevel.Information, default, ProbeState, null, ProbeFormatter);

        var allocated = Measure(() =>
        {
            for (var i = 0; i < Calls; i++)
            {
                sut.Log(LogLevel.Information, default, ProbeState, null, ProbeFormatter);
            }
        });

        AssertNoPerCallAllocation(allocated);
        var entries = sut.LogEntries;
        Assert.Equal((2 * Calls) + 1, entries.Count);
        Assert.Equal("probe", entries[Calls].Formatter(entries[Calls].State, entries[Calls].Exception));
    }



    [Fact]
    public void Scopes_when_no_scope_is_open_allocates_nothing()
    {
        var sut = new InMemoryLogger("alloc");
        _ = sut.Scopes;

        var allocated = Measure(() =>
        {
            for (var i = 0; i < Calls; i++)
            {
                _ = sut.Scopes;
            }
        });

        AssertNoPerCallAllocation(allocated);
    }



    // Runs the loop once to let tiered compilation settle, then measures a second run.
    private static long Measure(Action action)
    {
        action();
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }



    // Even after the warm-up run the runtime itself can allocate a few dozen bytes once inside the
    // measured window on net5.0-net7.0 (tiered-JIT / on-stack-replacement bookkeeping; 40-80 bytes
    // seen locally). One allocation per call, the regression these tests exist to catch, costs at
    // least 24 bytes x Calls, so "fewer bytes than calls" separates the two without flakiness.
    private static void AssertNoPerCallAllocation(long allocated)
    {
        Assert.True
        (
            allocated < Calls,
            $"Expected no allocation per call; {allocated} bytes were allocated over {Calls} calls."
        );
    }



    private sealed class AllocationProbe
    {
    }
}
#endif
