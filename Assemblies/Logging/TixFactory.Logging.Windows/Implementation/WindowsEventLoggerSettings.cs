namespace TixFactory.Logging.Windows
{
    /// <inheritdoc cref="IWindowsEventLoggerSettings"/>
    public class WindowsEventLoggerSettings : LoggerSettings, IWindowsEventLoggerSettings
    {
        /// <inheritdoc cref="IWindowsEventLoggerSettings.LogSource"/>
        public string LogSource { get; set; } = "TixFactory";

        /// <inheritdoc cref="IWindowsEventLoggerSettings.LogName"/>
        public string LogName { get; set; } = "Application";

        /// <inheritdoc cref="IWindowsEventLoggerSettings.MachineName"/>
        public string MachineName { get; set; } = null;
    }
}
