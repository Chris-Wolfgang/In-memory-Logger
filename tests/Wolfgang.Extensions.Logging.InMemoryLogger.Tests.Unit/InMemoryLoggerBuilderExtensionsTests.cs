using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Unit;

public class InMemoryLoggerBuilderExtensionsTests
{
    [Fact]
    public void AddInMemoryLogger_when_called_registers_provider()
    {
        var provider = new InMemoryLoggerProvider();
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddInMemoryLogger(provider));

        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<InMemoryLoggerBuilderExtensionsTests>>();

        logger.LogInformation("Test message");

        Assert.Single(provider.LogEntries);
    }


    [Fact]
    public void AddInMemoryLogger_when_builder_is_null_throws_ArgumentNullException()
    {
        var provider = new InMemoryLoggerProvider();

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => InMemoryLoggerBuilderExtensions.AddInMemoryLogger(null!, provider)
        );
        Assert.Equal("builder", ex.ParamName);
    }


    [Fact]
    public void AddInMemoryLogger_when_provider_is_null_throws_ArgumentNullException()
    {
        var services = new ServiceCollection();

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => services.AddLogging(builder => builder.AddInMemoryLogger(null!))
        );
        Assert.Equal("provider", ex.ParamName);
    }
}
