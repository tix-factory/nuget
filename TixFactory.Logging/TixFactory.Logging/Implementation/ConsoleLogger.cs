using System;

namespace TixFactory.Logging
{
	/// <summary>
	/// An implementation of <see cref="ILogger"/> that writes to the console.
	/// </summary>
	public class ConsoleLogger : ILogger
	{
		private readonly ILoggerSettings _LoggerSettings;

		/// <summary>
		/// Initializes a new <see cref="ConsoleLogger"/>.
		/// </summary>
		public ConsoleLogger()
			: this(new LoggerSettings())
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ConsoleLogger"/>.
		/// </summary>
		/// <param name="loggerSettings">An <see cref="ILoggerSettings"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="loggerSettings"/></exception>
		public ConsoleLogger(ILoggerSettings loggerSettings)
		{
			_LoggerSettings = loggerSettings ?? throw new ArgumentNullException(nameof(loggerSettings));
		}

		/// <inheritdoc cref="ILogger.Verbose"/>
		public void Verbose(string message) => Write(LogLevel.Verbose, message);

		/// <inheritdoc cref="ILogger.Info"/>
		public void Info(string message) => Write(LogLevel.Information, message);

		/// <inheritdoc cref="ILogger.Warn"/>
		public void Warn(string message) => Write(LogLevel.Warning, message);

		/// <inheritdoc cref="ILogger.Error(Exception)"/>
		public void Error(Exception ex) => Write(LogLevel.Error, $"{ex}");

		/// <inheritdoc cref="ILogger.Error(string)"/>
		public void Error(string message) => Write(LogLevel.Error, message);

		/// <inheritdoc cref="ILogger.Write"/>
		public void Write(LogLevel logLevel, string message)
		{
			if (logLevel < _LoggerSettings.MinimumLogLevel)
			{
				return;
			}

			if (logLevel == LogLevel.Error)
			{
				Console.Error.WriteLine(message);
			}
			else
			{
				Console.WriteLine(message);
			}
		}
	}
}
