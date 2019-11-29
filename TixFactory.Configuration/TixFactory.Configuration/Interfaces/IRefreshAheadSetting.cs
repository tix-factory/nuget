using System;

namespace TixFactory.Configuration
{
	/// <summary>
	/// A setting that is updated ahead of time on an interval.
	/// </summary>
	/// <typeparam name="T">The setting value type.</typeparam>
	public interface IRefreshAheadSetting<T> : IManufacturedSetting<T>
	{
		/// <summary>
		/// An event that is triggered when refreshing the setting value results in error.
		/// </summary>
		event Action<Exception> RefreshException;

		/// <summary>
		/// How often the value is refreshed.
		/// </summary>
		ISetting<TimeSpan> RefreshInterval { get; }

		/// <summary>
		/// The last time the setting value was refreshed successfully.
		/// </summary>
		IReadOnlySetting<DateTime?> LastRefresh { get; }
	}
}
