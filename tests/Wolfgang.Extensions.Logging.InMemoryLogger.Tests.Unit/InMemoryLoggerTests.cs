using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Unit;

public class InMemoryLoggerTests
{
    [Fact]
    public void Ctor_when_default_parameters_creates_instance()
    {
        var sut = new InMemoryLogger("TestCategory");

        Assert.Equal("TestCategory", sut.Category);
    }



    [Fact]
    public void Ctor_when_category_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new InMemoryLogger(null!)
        );
        Assert.Equal("category", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_capacity_is_less_than_1_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new InMemoryLogger("TestCategory", capacity: 0)
        );
        Assert.Equal("capacity", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_default_parameters_MinimumLogLevel_is_Trace()
    {
        var sut = new InMemoryLogger("TestCategory");

        Assert.Equal(LogLevel.Trace, sut.MinimumLogLevel);
    }



    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    [InlineData(LogLevel.None)]
    public void Ctor_when_setting_minimumLogLevel_sets_for_the_instance(LogLevel minLogLevel)
    {
        var sut = new InMemoryLogger("TestCategory", minLogLevel);

        Assert.Equal(minLogLevel, sut.MinimumLogLevel);
    }



    [Fact]
    public void LogEntries_when_nothing_is_logged_returns_empty_list()
    {
        var sut = new InMemoryLogger("TestCategory");

        Assert.Empty(sut.LogEntries);
    }



    [Fact]
    public void Log_when_called_adds_entry_to_LogEntries()
    {
        var sut = new InMemoryLogger("TestCategory");
        sut.Log<string>
        (
            LogLevel.Information,
            new EventId(1, "TestEvent"),
            "Test message",
            exception: null,
            static (s, _) => s
        );

        Assert.Single(sut.LogEntries);
        var logEntry = sut.LogEntries[0];
        Assert.Equal(LogLevel.Information, logEntry.LogLevel);
        Assert.Equal("TestCategory", logEntry.Category);
        Assert.Equal(1, logEntry.EventId.Id);
        Assert.Equal("Test message", logEntry.Formatter(logEntry.State, logEntry.Exception));
    }



    [Fact]
    public void Log_when_MinimumLogLevel_is_None_does_not_add_entry()
    {
        var sut = new InMemoryLogger("TestCategory", LogLevel.None);

        sut.LogTrace("Test message");

        Assert.Empty(sut.LogEntries);
    }



    [Fact]
    public void IsEnabled_when_logLevel_is_None_returns_false()
    {
        var sut = new InMemoryLogger("TestCategory");

        Assert.False(sut.IsEnabled(LogLevel.None));
    }



    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void Ctor_when_capacity_specified_initializes_to_specified_value(int capacity)
    {
        var sut = new InMemoryLogger("TestCategory", capacity: capacity);

        Assert.Equal(capacity, sut.Capacity);
    }



    [Fact]
    public void BeginScope_when_active_Scopes_contains_scope_state()
    {
        var sut = new InMemoryLogger("TestCategory");

        using (sut.BeginScope("outer"))
        {
            Assert.Single(sut.Scopes);
            Assert.Equal("outer", sut.Scopes[0]);
        }
    }



    [Fact]
    public void BeginScope_when_nested_Scopes_contains_outermost_to_innermost()
    {
        var sut = new InMemoryLogger("TestCategory");

        using (sut.BeginScope("outer"))
        using (sut.BeginScope("inner"))
        {
            Assert.Equal(2, sut.Scopes.Length);
            Assert.Equal("outer", sut.Scopes[0]);
            Assert.Equal("inner", sut.Scopes[1]);
        }
    }



    [Fact]
    public void BeginScope_when_disposed_Scopes_is_empty()
    {
        var sut = new InMemoryLogger("TestCategory");

        using (sut.BeginScope("scope"))
        {
            Assert.Single(sut.Scopes);
        }

        Assert.Empty(sut.Scopes);
    }



    [Fact]
    public void Log_when_the_same_formatter_is_reused_stores_the_same_adapter()
    {
        var sut = new InMemoryLogger("TestCategory");
        Func<string, Exception?, string> formatter = static (s, _) => s;

        sut.Log(LogLevel.Information, new EventId(1), "first", exception: null, formatter);
        sut.Log(LogLevel.Information, new EventId(2), "second", exception: null, formatter);

        var entries = sut.LogEntries;
        Assert.Same(entries[0].Formatter, entries[1].Formatter);
    }



    [Fact]
    public void Log_when_a_different_formatter_is_passed_each_entry_formats_with_its_own_formatter()
    {
        var sut = new InMemoryLogger("TestCategory");

        sut.Log(LogLevel.Information, new EventId(1), "state", exception: null, static (s, _) => "upper:" + s.ToUpperInvariant());
        sut.Log(LogLevel.Information, new EventId(2), "state", exception: null, static (s, _) => "lower:" + s);

        var entries = sut.LogEntries;
        Assert.NotSame(entries[0].Formatter, entries[1].Formatter);
        Assert.Equal("upper:STATE", entries[0].Formatter(entries[0].State, entries[0].Exception));
        Assert.Equal("lower:state", entries[1].Formatter(entries[1].State, entries[1].Exception));
    }
}
