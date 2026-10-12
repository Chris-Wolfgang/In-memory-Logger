namespace Wolfgang.Extensions.Logging.InMemoryLogger.Tests.Unit;

/// <summary>
/// Formatters shared by tests that log a plain <see cref="string"/> state.
/// </summary>
internal static class TestFormatters
{
    /// <summary>
    /// Returns the state unchanged, ignoring the exception.
    /// </summary>
    internal static string Identity(string state, Exception? _) => state;
}
