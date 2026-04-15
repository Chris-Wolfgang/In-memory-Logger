using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Unit;

public class InMemoryLoggerProviderTests
{
    [Fact]
    public void CreateLogger_when_called_returns_ILogger_instance()
    {
        using var sut = new InMemoryLoggerProvider();

        var logger = sut.CreateLogger("TestCategory");

        Assert.NotNull(logger);
        Assert.IsType<InMemoryLogger>(logger);
    }


    [Fact]
    public void CreateLogger_when_called_twice_with_same_category_returns_same_instance()
    {
        using var sut = new InMemoryLoggerProvider();

        var logger1 = sut.CreateLogger("TestCategory");
        var logger2 = sut.CreateLogger("TestCategory");

        Assert.Same(logger1, logger2);
    }


    [Fact]
    public void CreateLogger_when_called_with_different_categories_returns_different_instances()
    {
        using var sut = new InMemoryLoggerProvider();

        var logger1 = sut.CreateLogger("Category1");
        var logger2 = sut.CreateLogger("Category2");

        Assert.NotSame(logger1, logger2);
    }


    [Fact]
    public void LogEntries_when_multiple_loggers_log_contains_all_entries()
    {
        using var sut = new InMemoryLoggerProvider();

        var logger1 = sut.CreateLogger("Category1");
        var logger2 = sut.CreateLogger("Category2");
        logger1.LogInformation("Message 1");
        logger2.LogWarning("Message 2");

        Assert.Equal(2, sut.LogEntries.Count);
    }


    [Fact]
    public void Loggers_returns_all_created_loggers()
    {
        using var sut = new InMemoryLoggerProvider();

        sut.CreateLogger("Category1");
        sut.CreateLogger("Category2");

        Assert.Equal(2, sut.Loggers.Count);
        Assert.True(sut.Loggers.ContainsKey("Category1"));
        Assert.True(sut.Loggers.ContainsKey("Category2"));
    }


    [Fact]
    public void Ctor_when_minLogLevel_specified_loggers_use_that_level()
    {
        using var sut = new InMemoryLoggerProvider(LogLevel.Warning);

        var logger = sut.CreateLogger("TestCategory");
        logger.LogInformation("Should be filtered");
        logger.LogWarning("Should be logged");

        Assert.Single(sut.LogEntries);
    }


    [Fact]
    public void Dispose_does_not_throw()
    {
        var sut = new InMemoryLoggerProvider();

        sut.Dispose();
    }
}
