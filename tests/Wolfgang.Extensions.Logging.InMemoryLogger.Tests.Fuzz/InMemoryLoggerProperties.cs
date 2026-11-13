using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Fuzz;

/// <summary>
/// Property-based fuzz tests for <see cref="InMemoryLogger"/>, <see cref="InMemoryLogger{T}"/>
/// and <see cref="InMemoryLoggerProvider"/>. Each property drives the logger with a generated
/// sequence of operations and compares the result with a small reference model written out by
/// hand: a filtered list for <c>Log</c>, a comparison for <c>IsEnabled</c>, a stack for
/// <c>BeginScope</c> / <c>Dispose</c>. A disagreement means the logger gained or lost a
/// behaviour that a hand-picked unit test did not pin down.
/// </summary>
/// <remarks>
/// Log levels are generated as raw <see cref="byte"/> values and mapped onto 0-6
/// (<see cref="LogLevel.Trace"/> to <see cref="LogLevel.None"/>) so every level, including
/// <see cref="LogLevel.None"/>, is reached. Each property runs
/// <see cref="FuzzPropertyAttribute.DefaultMaxTest"/> (100) cases by default; the weekly
/// <c>fuzz.yaml</c> run sets <c>FSCHECK_MAX_TESTS</c> to 100,000 (#105).
/// </remarks>
public class InMemoryLoggerProperties
{
    private static LogLevel ToLevel(byte value) => (LogLevel)(value % 7);



    private static string Identity(string state, Exception? _) => state;



    [FuzzProperty]
    public bool IsEnabled_matches_the_level_comparison(byte minimum, byte level)
    {
        var min = ToLevel(minimum);
        var lvl = ToLevel(level);
        var sut = new InMemoryLogger("fuzz", min);

        var expected = lvl != LogLevel.None && min != LogLevel.None && lvl >= min;

        return sut.IsEnabled(lvl) == expected
            && new InMemoryLogger<InMemoryLoggerProperties>(min).IsEnabled(lvl) == expected;
    }



    [FuzzProperty]
    public bool Log_keeps_exactly_the_enabled_entries_in_order(byte minimum, byte[] levels, string?[] messages)
    {
        ArgumentNullException.ThrowIfNull(levels);
        ArgumentNullException.ThrowIfNull(messages);
        var min = ToLevel(minimum);
        var sut = new InMemoryLogger("fuzz", min, capacity: 1);
        var count = Math.Min(levels.Length, messages.Length);
        var expected = new List<(LogLevel Level, int Id, string Message)>();

        for (var i = 0; i < count; i++)
        {
            var level = ToLevel(levels[i]);
            var message = messages[i] ?? string.Empty;
            sut.Log(level, new EventId(i), message, exception: null, Identity);
            if (level != LogLevel.None && min != LogLevel.None && level >= min)
            {
                expected.Add((level, i, message));
            }
        }

        var actual = sut.LogEntries;
        return actual.Count == expected.Count
            && actual.Select(e => (e.LogLevel, e.EventId.Id, e.Formatter(e.State, e.Exception))).SequenceEqual(expected)
            && actual.All(e => string.Equals(e.Category, "fuzz", StringComparison.Ordinal));
    }



    [FuzzProperty]
    public bool LogEntries_is_a_snapshot_that_later_writes_do_not_change(string[] first, string[] second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        var sut = new InMemoryLogger("fuzz");
        foreach (var message in first)
        {
            sut.LogInformation("{Message}", message);
        }

        var snapshot = sut.LogEntries;
        foreach (var message in second)
        {
            sut.LogInformation("{Message}", message);
        }

        return snapshot.Count == first.Length
            && sut.LogEntries.Count == first.Length + second.Length;
    }



    /// <summary>
    /// Opens and disposes scopes in a generated order. Each operation byte either opens a new
    /// scope (even values) or disposes one of the handles opened so far (odd values, chosen by
    /// the byte), so out-of-order and repeated disposals are generated as often as in-order
    /// ones. The model is the documented contract: a dispose pops only when it is the first
    /// dispose of the scope currently on top; anything else is ignored.
    /// </summary>
    [FuzzProperty]
    public bool Scopes_follow_the_stack_model_for_any_open_and_dispose_order(byte[] operations)
    {
        ArgumentNullException.ThrowIfNull(operations);

        // Each case runs in a copy of the current execution context, so the AsyncLocal scope stacks
        // it leaves behind (a scope whose dispose was ignored because it came out of order stays
        // open, as documented) are discarded when the case ends instead of piling up in this
        // thread's context and slowing every later case down.
        var matches = false;
        ExecutionContext.Run
        (
            ExecutionContext.Capture()!,
            _ => matches = RunScopeModel(operations),
            state: null
        );

        return matches;
    }



    private static bool RunScopeModel(byte[] operations)
    {
        var sut = new InMemoryLogger("fuzz");
        var handles = new List<(IDisposable Handle, int State)>();
        var disposed = new HashSet<int>();
        var model = new Stack<int>();
        var next = 0;
        var matches = true;

        foreach (var op in operations)
        {
            if (op % 2 == 0 || handles.Count == 0)
            {
                var state = next++;
                handles.Add((sut.BeginScope(state), state));
                model.Push(state);
            }
            else
            {
                var (handle, state) = handles[(op / 2) % handles.Count];
                var first = disposed.Add(state);
                handle.Dispose();
                if (first && model.Count > 0 && model.Peek() == state)
                {
                    model.Pop();
                }
            }

            matches &= sut.Scopes.SequenceEqual(model.Reverse().Cast<object>());
        }

        return matches;
    }



    [FuzzProperty]
    public bool Provider_collects_every_entry_and_one_logger_per_category(byte[] categoryIndexes)
    {
        ArgumentNullException.ThrowIfNull(categoryIndexes);
        using var sut = new InMemoryLoggerProvider();
        var perCategory = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var index in categoryIndexes)
        {
            var category = "c" + (index % 5).ToString(System.Globalization.CultureInfo.InvariantCulture);
            sut.CreateLogger(category).LogWarning("entry");
            perCategory[category] = perCategory.TryGetValue(category, out var n) ? n + 1 : 1;
        }

        return sut.LogEntries.Count == categoryIndexes.Length
            && sut.Loggers.Count == perCategory.Count
            && perCategory.All(kv => sut.Loggers[kv.Key].LogEntries.Count == kv.Value);
    }



    [FuzzProperty]
    public bool Constructor_accepts_every_positive_capacity_and_rejects_the_rest(int capacity)
    {
        if (capacity < 1)
        {
            var ex = Record.Exception(() => new InMemoryLogger("fuzz", capacity: capacity));
            return ex is ArgumentOutOfRangeException { ParamName: "capacity" };
        }

        return new InMemoryLogger("fuzz", capacity: capacity).Capacity == capacity
            && new InMemoryLogger<InMemoryLoggerProperties>(capacity: capacity).Capacity == capacity;
    }
}
