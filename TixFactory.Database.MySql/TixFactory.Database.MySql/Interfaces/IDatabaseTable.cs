using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A table in a MySQL database.
	/// </summary>
	public interface IDatabaseTable
	{
		/// <summary>
		/// The table name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets an <see cref="IDatabaseTableColumn"/> with the specified name.
		/// </summary>
		/// <remarks>
		/// If the column name is invalid according the the <see cref="IDatabaseNameValidator"/>
		/// the returned value will be <c>null</c>. If it's not valid it can't possibly exist, right?
		/// </remarks>
		/// <param name="columnName">The column name.</param>
		/// <returns>The <see cref="IDatabaseTableColumn"/> (or <c>null</c> if it does not exist).</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database table columns.
		/// </exception>
		IDatabaseTableColumn GetColumn(string columnName);

		/// <summary>
		/// Gets all the <see cref="IDatabaseTableColumn"/>s for the table.
		/// </summary>
		/// <returns>The collection of <see cref="IDatabaseTableColumn"/>.</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database table columns.
		/// </exception>
		IReadOnlyCollection<IDatabaseTableColumn> GetAllColumns();

		/// <summary>
		/// Gets an <see cref="IDatabaseTableIndex"/> with the specified name.
		/// </summary>
		/// <remarks>
		/// If the index name is invalid according the the <see cref="IDatabaseNameValidator"/>
		/// the returned value will be <c>null</c>. If it's not valid it can't possibly exist, right?
		/// </remarks>
		/// <param name="indexName">The index name.</param>
		/// <returns>The <see cref="IDatabaseTableIndex"/> (or <c>null</c> if it does not exist).</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database table indexes.
		/// </exception>
		IDatabaseTableIndex GetIndex(string indexName);

		/// <summary>
		/// Gets all the <see cref="IDatabaseTableIndex"/>s for the table.
		/// </summary>
		/// <returns>The collection of <see cref="IDatabaseTableIndex"/>.</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error reading database table indexes.
		/// </exception>
		IReadOnlyCollection<IDatabaseTableIndex> GetAllIndexes();

		/// <summary>
		/// Creates an index on the table.
		/// </summary>
		/// <param name="indexName">The index name.</param>
		/// <param name="unique">Whether or not the index is unique.</param>
		/// <param name="columns">The <see cref="IDatabaseTableColumn"/>s in the index.</param>
		/// <returns><c>true</c> if the index did not already exist and was created (otherwise <c>false</c>).</returns>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="columns"/>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// - <paramref name="indexName"/> is invalid.
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error creating index.
		/// </exception>
		bool CreateIndex(string indexName, bool unique, IReadOnlyCollection<IDatabaseTableColumn> columns);

		/// <summary>
		/// Drops an index with the given name from the table.
		/// </summary>
		/// <param name="indexName">The name of the index to drop.</param>
		/// <returns><c>true</c> if the index existed and was dropped.</returns>
		/// <exception cref="MySqlException">
		/// - Unexpected error dropping index.
		/// </exception>
		bool DropIndex(string indexName);
	}
}
