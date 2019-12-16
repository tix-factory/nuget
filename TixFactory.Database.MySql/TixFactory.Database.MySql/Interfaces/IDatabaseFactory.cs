using System;
using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A factory for <see cref="IDatabase"/>s.
	/// </summary>
	public interface IDatabaseFactory
	{
		/// <summary>
		/// Creates a database with the specified name.
		/// </summary>
		/// <remarks>
		/// If the database name is invalid according the the <see cref="IDatabaseNameValidator"/>
		/// the returned value will be <c>null</c>. If it's not valid it can't possibly exist, right?
		/// </remarks>
		/// <param name="databaseName">The database name.</param>
		/// <returns>The <see cref="IDatabase"/> (or <c>null</c> if it does not exist).</returns>
		IDatabase GetDatabase(string databaseName);

		/// <summary>
		/// Creates a database with the specified name.
		/// </summary>
		/// <remarks>
		/// Database names are validated using an <see cref="IDatabaseNameValidator"/>.
		/// </remarks>
		/// <param name="databaseName">The database name.</param>
		/// <returns>The created <see cref="IDatabase"/>.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="databaseName"/> is invalid.
		/// - <paramref name="databaseName"/> matches a database that already exists.
		/// </exception>
		IDatabase CreateDatabase(string databaseName);
		
		/// <summary>
		/// Attempts to get a database by name and creates it if it does not exist.
		/// </summary>
		/// <remarks>
		/// Database names are validated using an <see cref="IDatabaseNameValidator"/>.
		/// </remarks>
		/// <param name="databaseName">The database name.</param>
		/// <returns>The <see cref="IDatabase"/>.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="databaseName"/> is invalid.
		/// </exception>
		IDatabase GetOrCreateDatabase(string databaseName);

		/// <summary>
		/// Gets all the databases.
		/// </summary>
		/// <returns>A collection of all the databases on the server.</returns>
		IReadOnlyCollection<IDatabase> GetAllDatabases();
	}
}
