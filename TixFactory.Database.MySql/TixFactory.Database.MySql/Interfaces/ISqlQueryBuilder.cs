using System;
using System.Linq.Expressions;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Builds SQL queries from C# concepts (e.g. <see cref="Expression"/>).
	/// </summary>
	public interface ISqlQueryBuilder
	{
		/// <summary>
		/// Builds a select query without a WHERE clause.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The query string.</returns>
		string BuildSelectTopQuery<TRow>(string databaseName, string tableName, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a select query with a WHERE clause and no query parameters.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The query string.</returns>
		string BuildSelectTopQuery<TRow>(string databaseName, string tableName, Expression<Func<TRow, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a select query with 1 query parameter.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The query string.</returns>
		string BuildSelectTopQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a select query with 2 query parameters.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The query string.</returns>
		string BuildSelectTopQuery<TRow, TP1, TP2>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a select query with 3 query parameters.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <typeparam name="TP3">The third query parameter.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The query string.</returns>
		string BuildSelectTopQuery<TRow, TP1, TP2, TP3>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a select query with 4 query parameters.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <typeparam name="TP3">The third query parameter.</typeparam>
		/// <typeparam name="TP4">The fourth query parameter.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The query string.</returns>
		string BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a select query with 5 query parameters.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <typeparam name="TP3">The third query parameter.</typeparam>
		/// <typeparam name="TP4">The fourth query parameter.</typeparam>
		/// <typeparam name="TP5">The fifth query parameter.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The query string.</returns>
		string BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4, TP5>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class;
	}
}
