namespace TixFactory.Logging
{
    /// <inheritdoc cref="ILoggerSettings"/>
    public class LoggerSettings : ILoggerSettings
    {
        /// <inheritdoc cref="ILoggerSettings.MinimumLogLevel"/>
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Verbose;
    }
}
