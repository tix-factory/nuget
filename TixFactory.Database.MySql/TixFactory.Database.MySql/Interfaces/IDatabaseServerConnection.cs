using System;
using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A database server connection.
	/// </summary>
	public interface IDatabaseServerConnection
	{
		/// <summary>
		/// Executes a query against the database server.
		/// </summary>
		/// <param name="query">The MySQL query being executed.</param>
		/// <param name="queryParameters">The MySQL query parameters.</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="query"/> is <c>null</c> or whitespace.
		/// </exception>
		int ExecuteQuery(string query, IDictionary<string, object> queryParameters);

		/// <summary>
		/// Executes a query against the database server and parses the returned rows.
		/// </summary>
		/// <typeparam name="T">The class type to deserialize the returned rows into.</typeparam>
		/// <param name="query">The MySQL query being executed.</param>
		/// <param name="queryParameters">The MySQL query parameters.</param>
		/// <returns>The collection of rows returned from the query execution.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="query"/> is <c>null</c> or whitespace.
		/// </exception>
		IReadOnlyCollection<T> ExecuteQuery<T>(string query, IDictionary<string, object> queryParameters)
			where T : class;
	}
}
