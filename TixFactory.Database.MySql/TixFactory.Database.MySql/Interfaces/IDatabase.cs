using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A MySQL database.
	/// </summary>
	public interface IDatabase
	{
		/// <summary>
		/// The database name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets an <see cref="IDatabaseTable"/> with the specified name.
		/// </summary>
		/// <remarks>
		/// If the table name is invalid according the the <see cref="IDatabaseNameValidator"/>
		/// the returned value will be <c>null</c>. If it's not valid it can't possibly exist, right?
		/// </remarks>
		/// <param name="tableName">The table name.</param>
		/// <returns>The <see cref="IDatabaseTable"/> (or <c>null</c> if it does not exist).</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database tables.
		/// </exception>
		IDatabaseTable GetTable(string tableName);

		/// <summary>
		/// Gets all the <see cref="IDatabaseTable"/>s in the database.
		/// </summary>
		/// <returns>The collection of <see cref="IDatabaseTable"/>s.</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database tables.
		/// </exception>
		IReadOnlyCollection<IDatabaseTable> GetAllTables();

		/// <summary>
		/// Gets the names of all the stored procedures in the database.
		/// </summary>
		/// <returns>The collection of stored procedure names.</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database stored procedures.
		/// </exception>
		IReadOnlyCollection<string> GetStoredProcedureNames();
	}
}
