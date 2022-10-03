namespace TixFactory.Logging
{
    /// <summary>
    /// Settings for loggers.
    /// </summary>
    public interface ILoggerSettings
    {
        /// <summary>
        /// The minimum <see cref="LogLevel"/> to actually write.
        /// </summary>
        LogLevel MinimumLogLevel { get; }
    }
}
