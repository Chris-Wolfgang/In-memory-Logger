using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Unit;

public class InMemoryLoggerOfTTests
{
    [Fact]
    public void Ctor_when_default_parameters_creates_instance()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

        Assert.NotNull(sut);
    }


    [Fact]
    public void LogEntries_when_nothing_is_logged_returns_empty_list()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

        Assert.Empty(sut.LogEntries);
    }


    [Fact]
    public void Log_when_called_adds_entry_to_LogEntries()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();
        sut.Log<string>
        (
            LogLevel.Information,
            new EventId(1, "TestEvent"),
            "Test message",
            exception: null,
            (state, _) => state
        );

        Assert.Single(sut.LogEntries);
        var logEntry = sut.LogEntries[0];
        Assert.Equal(LogLevel.Information, logEntry.LogLevel);
        Assert.Equal(1, logEntry.EventId.Id);
        Assert.Equal("TestEvent", logEntry.EventId.Name);

        // Produce the formatted message the same way the logger would
        var formatted = logEntry.Formatter.Invoke(logEntry.State!, logEntry.Exception);
        Assert.Equal("Test message", formatted);

        Assert.Null(logEntry.Exception);
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
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>(minLogLevel);

        Assert.Equal(minLogLevel, sut.MinimumLogLevel);
    }



    [Fact]
    public void Ctor_when_default_parameters_MinimumLogLevel_is_Trace()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

        Assert.Equal(LogLevel.Trace, sut.MinimumLogLevel);
    }



    [Fact]
    public void Log_when_MinimumLogLevel_is_None_does_not_add_entry()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>(LogLevel.None);

        sut.LogTrace("Test message");

        Assert.Empty(sut.LogEntries);
    }


    [Fact]
    public void Log_when_MinimumLogLevel_is_Trace_adds_entry()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>(LogLevel.Trace);

        sut.LogTrace("Test message");

        Assert.NotEmpty(sut.LogEntries);
    }


    [Fact]
    public void IsEnabled_when_logLevel_is_None_returns_false()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>(LogLevel.Trace);

        Assert.False(sut.IsEnabled(LogLevel.None));
    }



    [Fact]
    public void Ctor_when_capacity_is_less_than_1_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new InMemoryLogger<InMemoryLoggerOfTTests>(capacity: 0)
        );
        Assert.Equal("capacity", ex.ParamName);
    }


    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void Ctor_when_capacity_specified_initializes_to_specified_value(int capacity)
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>(capacity: capacity);

        Assert.Equal(capacity, sut.Capacity);
    }


    [Fact]
    public void Ctor_when_capacity_not_specified_initializes_to_16()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

        Assert.Equal(16, sut.Capacity);
    }


    [Fact]
    public void BeginScope_returns_non_null_disposable()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

        var scope = sut.BeginScope("test scope");

        Assert.NotNull(scope);
        scope.Dispose();
    }


    [Fact]
    public void BeginScope_when_active_Scopes_contains_scope_state()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

        using (sut.BeginScope("outer"))
        {
            Assert.Single(sut.Scopes);
            Assert.Equal("outer", sut.Scopes[0]);
        }
    }


    [Fact]
    public void BeginScope_when_nested_Scopes_contains_outermost_to_innermost()
    {
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

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
        var sut = new InMemoryLogger<InMemoryLoggerOfTTests>();

        using (sut.BeginScope("scope"))
        {
            Assert.Single(sut.Scopes);
        }

        Assert.Empty(sut.Scopes);
    }
}
