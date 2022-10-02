using System;

namespace TixFactory.Configuration
{
	/// <summary>
	/// Represents a setting that can be changed but is readonly.
	/// </summary>
	/// <typeparam name="T">The setting type.</typeparam>
	public interface IReadOnlySetting<T>
	{
		/// <summary>
		/// An event triggered when the <see cref="Value"/> changes.
		/// </summary>
		/// <remarks>
		/// The action first parameter is the new value, the second parameter is the old value.
		/// </remarks>
		event Action<T, T> Changed;

		/// <summary>
		/// The setting value.
		/// </summary>
		T Value { get; }
	}
}
