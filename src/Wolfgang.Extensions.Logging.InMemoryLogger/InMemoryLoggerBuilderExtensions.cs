using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Extensions.Logging.InMemoryLogger;

/// <summary>
/// Extension methods for adding <see cref="InMemoryLoggerProvider"/> to an <see cref="ILoggingBuilder"/>.
/// </summary>
public static class InMemoryLoggerBuilderExtensions
{
	/// <summary>
	/// Adds an in-memory logger to the logging builder.
	/// </summary>
	/// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
	/// <param name="provider">The <see cref="InMemoryLoggerProvider"/> instance to register.</param>
	/// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="builder"/> or <paramref name="provider"/> is <see langword="null"/>.
	/// </exception>
	/// <example>
	/// <code>
	/// var provider = new InMemoryLoggerProvider();
	/// var serviceProvider = new ServiceCollection()
	///     .AddLogging(builder => builder.AddInMemoryLogger(provider))
	///     .BuildServiceProvider();
	/// </code>
	/// </example>
	public static ILoggingBuilder AddInMemoryLogger
	(
		this ILoggingBuilder builder,
		InMemoryLoggerProvider provider
	)
	{
		if (builder == null)
		{
			throw new ArgumentNullException(nameof(builder));
		}

		if (provider == null)
		{
			throw new ArgumentNullException(nameof(provider));
		}

		builder.AddProvider(provider);

		return builder;
	}
}
