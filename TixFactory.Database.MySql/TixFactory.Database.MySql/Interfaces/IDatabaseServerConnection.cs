﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A database server connection.
	/// </summary>
	public interface IDatabaseServerConnection
	{
		/// <summary>
		/// Gets the database associated in the connection string.
		/// </summary>
		/// <returns>The <see cref="IDatabase"/> selected in the connection string (or <c>null</c> if there wasn't one specified).</returns>
		IDatabase GetConnectedDatabase();

		/// <summary>
		/// Builds an <see cref="IDatabaseFactory"/> with the connection.
		/// </summary>
		/// <returns>The <see cref="IDatabaseFactory"/>.</returns>
		IDatabaseFactory BuildDatabaseFactory();

		/// <summary>
		/// Executes a query against the database server.
		/// </summary>
		/// <param name="query">The MySQL query being executed.</param>
		/// <param name="queryParameters">The MySQL query parameters.</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="query"/> is <c>null</c> or whitespace.
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error executing query.
		/// </exception>
		int ExecuteQuery(string query, IDictionary<string, object> queryParameters);

		/// <summary>
		/// Executes a query against the database server.
		/// </summary>
		/// <remarks>
		/// NOTE: If your <see cref="MySqlParameter.Value"/> is explicitly zero you will have to make it an "object": https://stackoverflow.com/a/18789917/1663648 
		/// </remarks>
		/// <param name="query">The MySQL query being executed.</param>
		/// <param name="mySqlParameters">The MySQL query parameters.</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="query"/> is <c>null</c> or whitespace.
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error executing stored procedure.
		/// </exception>
		int ExecuteQuery(string query, IReadOnlyCollection<MySqlParameter> mySqlParameters);

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
		/// <exception cref="MySqlException">
		/// - Unexpected error executing query.
		/// </exception>
		IReadOnlyCollection<T> ExecuteQuery<T>(string query, IDictionary<string, object> queryParameters)
			where T : class;

		/// <summary>
		/// Executes a query against the database server and parses the returned rows.
		/// </summary>
		/// <remarks>
		/// NOTE: If your <see cref="MySqlParameter.Value"/> is explicitly zero you will have to make it an "object": https://stackoverflow.com/a/18789917/1663648 
		/// </remarks>
		/// <typeparam name="T">The class type to deserialize the returned rows into.</typeparam>
		/// <param name="query">The MySQL query being executed.</param>
		/// <param name="mySqlParameters">The MySQL query parameters.</param>
		/// <returns>The collection of rows returned from the query execution.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="query"/> is <c>null</c> or whitespace.
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error executing query.
		/// </exception>
		IReadOnlyCollection<T> ExecuteQuery<T>(string query, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class;

		/// <summary>
		/// Executes a stored procedure against the database server.
		/// </summary>
		/// <param name="storedProcedureName">The MySQL query being executed.</param>
		/// <param name="queryParameters">The MySQL parameters passed to the stored procedure.</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="storedProcedureName"/> if the stored procedure by that name does not exist.
		/// </exception>
		/// <exception cref="ApplicationException">
		/// - connection string must specify database
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error executing stored procedure.
		/// </exception>
		int ExecuteStoredProcedure(string storedProcedureName, IDictionary<string, object> queryParameters);

		/// <summary>
		/// Executes a query against the database server.
		/// </summary>
		/// <remarks>
		/// NOTE: If your <see cref="MySqlParameter.Value"/> is explicitly zero you will have to make it an "object": https://stackoverflow.com/a/18789917/1663648 
		/// </remarks>
		/// <param name="storedProcedureName">The MySQL query being executed.</param>
		/// <param name="mySqlParameters">The MySQL parameters passed to the stored procedure.</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="storedProcedureName"/> if the stored procedure by that name does not exist.
		/// </exception>
		/// <exception cref="ApplicationException">
		/// - connection string must specify database
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error executing stored procedure.
		/// </exception>
		int ExecuteStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters);

		/// <summary>
		/// Executes a stored procedure against the database server and parses the returned rows.
		/// </summary>
		/// <typeparam name="T">The class type to deserialize the returned rows into.</typeparam>
		/// <param name="storedProcedureName">The MySQL query being executed.</param>
		/// <param name="queryParameters">The MySQL parameters passed to the stored procedure.</param>
		/// <returns>The collection of rows returned from the query execution.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="storedProcedureName"/> if the stored procedure by that name does not exist.
		/// </exception>
		/// <exception cref="ApplicationException">
		/// - connection string must specify database
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error executing stored procedure.
		/// </exception>
		IReadOnlyCollection<T> ExecuteStoredProcedure<T>(string storedProcedureName, IDictionary<string, object> queryParameters)
			where T : class;

		/// <summary>
		/// Executes a stored procedure against the database server and parses the returned rows.
		/// </summary>
		/// <remarks>
		/// NOTE: If your <see cref="MySqlParameter.Value"/> is explicitly zero you will have to make it an "object": https://stackoverflow.com/a/18789917/1663648 
		/// </remarks>
		/// <typeparam name="T">The class type to deserialize the returned rows into.</typeparam>
		/// <param name="storedProcedureName">The MySQL query being executed.</param>
		/// <param name="mySqlParameters">The MySQL parameters passed to the stored procedure.</param>
		/// <returns>The collection of rows returned from the query execution.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="storedProcedureName"/> if the stored procedure by that name does not exist.
		/// </exception>
		/// <exception cref="ApplicationException">
		/// - connection string must specify database
		/// </exception>
		/// <exception cref="MySqlException">
		/// - Unexpected error executing stored procedure.
		/// </exception>
		IReadOnlyCollection<T> ExecuteStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class;
	}
}