using System.Collections.Generic;

namespace TixFactory.Configuration
{
	/// <summary>
	/// An <see cref="IList{T}"/> that provides a method for checking when the count changes (<see cref="CountSetting"/>).
	/// </summary>
	/// <typeparam name="T">The list item type.</typeparam>
	public interface ITrackedList<T> : IList<T>, IReadOnlyList<T>
	{
		/// <summary>
		/// The list size.
		/// </summary>
		IReadOnlySetting<int> CountSetting { get; }
	}
}
