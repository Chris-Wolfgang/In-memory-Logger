using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Unit.Globalization;

/// <summary>
/// The logger stores whatever the caller's formatter produces and never formats, compares or
/// case-maps text using the current culture. These tests assert that under cultures known to expose
/// such bugs. The library has no intentionally culture-sensitive public members.
/// </summary>
public class CultureInvarianceTests
{
    public static IEnumerable<object[]> Cultures() =>
        new[]
        {
            new object[] { "en-US" },
            new object[] { "tr-TR" },
            new object[] { "de-DE" },
            new object[] { "zh-CN" },
            new object[] { "ar-SA" },
            new object[] { "ja-JP" }
        };



    [Theory]
    [MemberData(nameof(Cultures))]
    public void Log_when_formatter_returns_text_stores_formatter_output_verbatim(string cultureName)
    {
        using var scope = new CultureScope(cultureName);
        var sut = new InMemoryLogger("category");

        sut.Log
        (
            LogLevel.Information,
            new EventId(1),
            "İıIi 1234.5",
            null,
            TestFormatters.Identity
        );

        var entry = Assert.Single(sut.LogEntries);
        Assert.Equal
        (
            "İıIi 1234.5",
            entry.Formatter(entry.State, null)
        );
    }



    [Theory]
    [MemberData(nameof(Cultures))]
    public void Ctor_when_category_has_culture_sensitive_casing_preserves_category(string cultureName)
    {
        using var scope = new CultureScope(cultureName);

        var sut = new InMemoryLogger("TITLE-İI");

        Assert.Equal("TITLE-İI", sut.Category);
    }



    [Theory]
    [MemberData(nameof(Cultures))]
    public void CreateLogger_when_names_differ_only_by_dotted_i_casing_returns_distinct_loggers(string cultureName)
    {
        using var scope = new CultureScope(cultureName);
        using var provider = new InMemoryLoggerProvider();

        var upper = provider.CreateLogger("INFO");
        var lower = provider.CreateLogger("info");
        var upperAgain = provider.CreateLogger("INFO");

        Assert.NotSame(upper, lower);
        Assert.Same(upper, upperAgain);
    }



    [Theory]
    [MemberData(nameof(Cultures))]
    public void IsEnabled_when_level_is_below_minimum_is_false_in_every_culture(string cultureName)
    {
        using var scope = new CultureScope(cultureName);
        var sut = new InMemoryLogger("category", LogLevel.Warning);

        Assert.False(sut.IsEnabled(LogLevel.Information));
        Assert.True(sut.IsEnabled(LogLevel.Error));
    }



    [Theory]
    [MemberData(nameof(Cultures))]
    public void CurrentCulture_after_logging_is_unchanged(string cultureName)
    {
        using var scope = new CultureScope(cultureName);
        var before = System.Globalization.CultureInfo.CurrentCulture.Name;
        var sut = new InMemoryLogger("category");

        sut.Log
        (
            LogLevel.Information,
            new EventId(1),
            "message",
            null,
            TestFormatters.Identity
        );

        Assert.Equal
        (
            before,
            System.Globalization.CultureInfo.CurrentCulture.Name
        );
    }
}
