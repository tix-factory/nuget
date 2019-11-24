using System;
using System.Collections.Generic;

namespace TixFactory.Http
{
	/// <summary>
	/// An object representing HTTP headers
	/// </summary>
	public interface IHttpHeaders
	{
		/// <summary>
		/// All header names currently in the collection
		/// </summary>
		IReadOnlyList<string> Keys { get; }

		/// <summary>
		/// Adds a header to the collection
		/// </summary>
		/// <remarks>
		/// Will add values even if they are already in the collection.
		/// </remarks>
		/// <param name="name">The header name.</param>
		/// <param name="value">The header value.</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> is null or whitespace.</exception>
		void Add(string name, string value);

		/// <summary>
		/// Adds or replaces a header in the collection
		/// </summary>
		/// <remarks>
		/// Will not create duplicate values, will replace all existing values with <paramref name="value"/>
		/// </remarks>
		/// <param name="name">The header name.</param>
		/// <param name="value">The header value.</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> is null or whitespace.</exception>
		void AddOrUpdate(string name, string value);

		/// <summary>
		/// Gets an <see cref="ICollection{T}"/> of header values by name from the collection
		/// </summary>
		/// <param name="name">The header name.</param>
		/// <returns>A collection of header values.</returns>
		/// <exception cref="ArgumentException"><paramref name="name"/> is null or whitespace.</exception>
		ICollection<string> Get(string name);

		/// <summary>
		/// Removes all headers with the given name from the collection.
		/// </summary>
		/// <param name="name">The header name.</param>
		/// <returns><c>true</c> if the header name was present in the collection, and was removed.</returns>
		/// <exception cref="ArgumentException"><paramref name="name"/> is null or whitespace.</exception>
		bool Remove(string name);
	}
}
