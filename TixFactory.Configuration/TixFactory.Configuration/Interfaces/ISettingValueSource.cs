using System;

namespace TixFactory.Configuration
{
	/// <summary>
	/// Responsible for reading/writing setting values.
	/// </summary>
	public interface ISettingValueSource
	{
		/// <summary>
		/// An event triggered when a setting value changes.
		/// </summary>
		/// <remarks>
		/// The first parameter is the group name, the second is the setting name.
		/// Use <see cref="TryGetSettingValue{T}"/> to get the new value.
		/// </remarks>
		event Action<string, string> SettingValueChanged;

		/// <summary>
		/// Attempt to load a setting value by group name + setting name.
		/// </summary>
		/// <typeparam name="T">The setting value type.</typeparam>
		/// <param name="groupName">The group name.</param>
		/// <param name="settingName">The setting name.</param>
		/// <param name="value">The loaded value.</param>
		/// <returns><c>true</c> if the setting value was loaded (otherwise <c>false</c>).</returns>
		bool TryGetSettingValue<T>(string groupName, string settingName, out T value);

		/// <summary>
		/// Writes a setting value.
		/// </summary>
		/// <typeparam name="T">The setting value type.</typeparam>
		/// <param name="groupName">The group name.</param>
		/// <param name="settingName">The setting name.</param>
		/// <param name="value">The new setting value.</param>
		void WriteSettingValue<T>(string groupName, string settingName, T value);
	}
}
