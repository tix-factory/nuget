namespace TixFactory.Logging.Windows
{
	/// <summary>
	/// <see cref="ILoggerSettings"/> specific for <see cref="WindowsEventLogger"/>.
	/// </summary>
	public interface IWindowsEventLoggerSettings : ILoggerSettings
	{
		/// <summary>
		/// The log source.
		/// </summary>
		string LogSource { get; }

		/// <summary>
		/// The log name.
		/// </summary>
		string LogName { get; }

		/// <summary>
		/// The machine the log is on.
		/// </summary>
		string MachineName { get; }
	}
}
