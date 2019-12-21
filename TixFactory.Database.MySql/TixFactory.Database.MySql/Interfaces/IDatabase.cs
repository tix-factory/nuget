using System;
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

		/// <summary>
		/// Creates a new stored procedure with the specified name and <see cref="ISqlQuery"/>.
		/// </summary>
		/// <param name="storedProcedureName">The stored procedure name.</param>
		/// <param name="query">The <see cref="ISqlQuery"/>.</param>
		/// <returns><c>true</c> if the stored procedure did not exist and was registered (otherwise <c>false</c>).</returns>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="query"/>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// - <paramref name="storedProcedureName"/> is invalid to be a stored procedure name.
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error registering stored procedure.
		/// </exception>
		bool RegisterStoredProcedure(string storedProcedureName, ISqlQuery query);

		/// <summary>
		/// Gets the names of all the stored procedures in the database.
		/// </summary>
		/// <param name="storedProcedureName">The stored procedure name.</param>
		/// <returns><c>true</c> if the stored procedure existed and was dropped (otherwise <c>false</c>).</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database stored procedures.
		/// </exception>
		bool DropStoredProcedure(string storedProcedureName);
	}
}
