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
		/// <typeparam name="T">The class type to deserialize the returned rows into.</typeparam>
		/// <param name="query">The MySQL query being executed.</param>
		/// <param name="queryParameters">The MySQL query parameters.</param>
		/// <returns>The collection of rows returned from the query execution.</returns>
		IReadOnlyCollection<T> ExecuteQuery<T>(string query, IDictionary<string, object> queryParameters)
			where T : class;
	}
}
