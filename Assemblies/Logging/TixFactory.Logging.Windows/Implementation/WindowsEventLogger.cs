using System;
using System.Diagnostics;

namespace TixFactory.Logging.Windows
{
    /// <summary>
    /// An <see cref="ILogger"/> that writes to the Windows event logs.
    /// </summary>
    public class WindowsEventLogger : ILogger
    {
        private const int _MaxLogLength = 16384;
        private const int _TruncateReservation = 128;

        private readonly IWindowsEventLoggerSettings _LoggerSettings;
        private readonly EventLog _EventLog;

        /// <summary>
        /// Initializes a new <see cref="ConsoleLogger"/>.
        /// </summary>
        /// <param name="loggerSettings">An <see cref="ILoggerSettings"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="loggerSettings"/></exception>
        public WindowsEventLogger(IWindowsEventLoggerSettings loggerSettings)
        {
            _LoggerSettings = loggerSettings ?? throw new ArgumentNullException(nameof(loggerSettings));

            if (loggerSettings.MachineName != null)
            {
                _EventLog = new EventLog(loggerSettings.LogName, loggerSettings.MachineName, loggerSettings.LogSource);
            }
            else
            {
                _EventLog = new EventLog(loggerSettings.LogName);
                _EventLog.Source = loggerSettings.LogSource;
            }
        }

        /// <inheritdoc cref="ILogger.Verbose"/>
        public void Verbose(string message)
        {
            Write(LogLevel.Verbose, message);
        }

        /// <inheritdoc cref="ILogger.Info"/>
        public void Info(string message)
        {
            Write(LogLevel.Information, message);
        }

        /// <inheritdoc cref="ILogger.Warn"/>
        public void Warn(string message)
        {
            Write(LogLevel.Warning, message);
        }

        /// <inheritdoc cref="ILogger.Error(Exception)"/>
        public void Error(Exception ex)
        {
            Write(LogLevel.Error, $"{ex}");
        }

        /// <inheritdoc cref="ILogger.Error(string)"/>
        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        /// <inheritdoc cref="ILogger.Write"/>
        public void Write(LogLevel logLevel, string message)
        {
            if (logLevel < _LoggerSettings.MinimumLogLevel)
            {
                return;
            }

            if (message.Length > _MaxLogLength)
            {
                var truncateAmount = message.Length - _MaxLogLength - _TruncateReservation;
                message = message.Substring(0, _MaxLogLength - _TruncateReservation) + $"{Environment.NewLine}{Environment.NewLine}[ Truncated {truncateAmount} characters ]";
            }

            _EventLog.WriteEntry(message, TranslateLogLevel(logLevel));
        }

        private EventLogEntryType TranslateLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Warning:
                    return EventLogEntryType.Warning;
                case LogLevel.Error:
                    return EventLogEntryType.Error;
                default:
                    return EventLogEntryType.Information;
            }
        }
    }
}
