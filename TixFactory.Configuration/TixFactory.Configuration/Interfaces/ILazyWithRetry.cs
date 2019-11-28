using System;

namespace TixFactory.Configuration
{
	/// <summary>
	/// Similar to <see cref="Lazy{T}"/> but will reuse the value factory until it returns a value.
	/// </summary>
	/// <remarks>
	/// If the provided value factory throws an unhandled exception it will be retries on next <see cref="Value"/> read.
	/// </remarks>
	/// <typeparam name="T">The value type.</typeparam>
	public interface ILazyWithRetry<T>
	{
		/// <summary>
		/// The loaded value.
		/// </summary>
		T Value { get; }

		/// <summary>
		/// Whether or not the value has been successfully loaded.
		/// </summary>
		bool IsValueCreated { get; }

		/// <summary>
		/// Clears the <see cref="Value"/> to be refetched.
		/// </summary>
		void Refresh();
	}
}
