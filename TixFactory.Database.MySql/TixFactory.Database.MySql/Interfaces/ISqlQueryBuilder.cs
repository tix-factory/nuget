using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Builds SQL queries from C# concepts (e.g. <see cref="Expression"/>).
	/// </summary>
	public interface ISqlQueryBuilder
	{
		/// <summary>
		/// Builds a delete query.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <param name="databaseName">The database name to delete in.</param>
		/// <param name="tableName">The table name to delete in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildDeleteQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression)
			where TRow : class;

		/// <summary>
		/// Builds an insert query.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="databaseName">The database name to insert into.</param>
		/// <param name="tableName">The table name to insert into.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildInsertQuery<TRow>(string databaseName, string tableName)
			where TRow : class;

		/// <summary>
		/// Builds an update query.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <param name="databaseName">The database name to insert into.</param>
		/// <param name="tableName">The table name to insert into.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildUpdateQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression)
			where TRow : class;

		/// <summary>
		/// Builds a select query without a WHERE clause.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildSelectTopQuery<TRow>(string databaseName, string tableName, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a select query with a WHERE clause and no query parameters.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="databaseName">The database name to select in.</param>
		/// <param name="tableName">The table name to select in.</param>
		/// <param name="whereExpression">The <see cref="Expression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildSelectTopQuery<TRow>(string databaseName, string tableName, Expression<Func<TRow, bool>> whereExpression, OrderBy<TRow> orderBy = null)
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
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildSelectTopQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression, OrderBy<TRow> orderBy = null)
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
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, bool>> whereExpression, OrderBy<TRow> orderBy = null)
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
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2, TP3>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, bool>> whereExpression, OrderBy<TRow> orderBy = null)
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
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> whereExpression, OrderBy<TRow> orderBy = null)
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
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4, TP5>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class;

		/// <summary>
		/// Builds a paged select query.
		/// </summary>
		/// <remarks>
		/// - The table name is derived from the <see cref="DataContractAttribute.Name"/> on the <typeparamref name="TRow"/>.
		/// - The database name is derived from the <see cref="DataContractAttribute.Namespace"/> on the <typeparamref name="TRow"/>.
		/// - <see cref="OrderBy{TRow}.SortOrder"/> is ignored for this query creation (query parameter decides sort order).
		/// </remarks>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> containing the column that the paged results should be ordered by.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="orderBy"/>
		/// </exception>
		ISqlQuery BuildSelectPagedQuery<TRow>(OrderBy<TRow> orderBy)
			where TRow : class;
		
		/// <summary>
		/// Builds a paged select query with an additional WHERE clause and 5 query parameters.
		/// </summary>
		/// <remarks>
		/// - The table name is derived from the <see cref="DataContractAttribute.Name"/> on the <typeparamref name="TRow"/>.
		/// - The database name is derived from the <see cref="DataContractAttribute.Namespace"/> on the <typeparamref name="TRow"/>.
		/// - <see cref="OrderBy{TRow}.SortOrder"/> is ignored for this query creation (query parameter decides sort order).
		/// </remarks>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="whereExpression">The <see cref="LambdaExpression"/> to parse into the WHERE clause.</param>
		/// <param name="orderBy">The <see cref="OrderBy{TRow}"/> (or <c>null</c> if the results are unordered).</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="orderBy"/>
		/// </exception>
		ISqlQuery BuildSelectPagedQuery<TRow>(LambdaExpression whereExpression, OrderBy<TRow> orderBy)
			where TRow : class;

		/// <summary>
		/// Builds a query to count rows in a table.
		/// </summary>
		/// <remarks>
		/// - The table name is derived from the <see cref="DataContractAttribute.Name"/> on the <typeparamref name="TRow"/>.
		/// - The database name is derived from the <see cref="DataContractAttribute.Namespace"/> on the <typeparamref name="TRow"/>.
		/// </remarks>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildCountQuery<TRow>()
			where TRow : class;

		/// <summary>
		/// Builds a query to count rows in a table with a WHERE clause and 5 query parameters.
		/// </summary>
		/// <remarks>
		/// - The table name is derived from the <see cref="DataContractAttribute.Name"/> on the <typeparamref name="TRow"/>.
		/// - The database name is derived from the <see cref="DataContractAttribute.Namespace"/> on the <typeparamref name="TRow"/>.
		/// </remarks>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="whereExpression">The <see cref="LambdaExpression"/> to parse into the WHERE clause.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildCountQuery<TRow>(LambdaExpression whereExpression)
			where TRow : class;

		/// <summary>
		/// Builds a query that can be executed to create a new stored procedure.
		/// </summary>
		/// <remarks>
		/// - <paramref name="useDelimiter"/> context: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/817#issuecomment-527921639
		/// </remarks>
		/// <param name="databaseName">The database name.</param>
		/// <param name="storedProcedureName">The stored procedure name.</param>
		/// <param name="query">The embedded query in the stored procedure.</param>
		/// <param name="useDelimiter">Whether or not the query needs to set the delimiter.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildCreateStoredProcedureQuery(string databaseName, string storedProcedureName, ISqlQuery query, bool useDelimiter);

		/// <summary>
		/// Builds a query that drops a stored procedure from a database.
		/// </summary>
		/// <param name="databaseName">The database name to drop from.</param>
		/// <param name="storedProcedureName">The name of the stored procedure being dropped.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildDropStoredProcedureQuery(string databaseName, string storedProcedureName);

		/// <summary>
		/// Builds a query for creating a table in a database.
		/// </summary>
		/// <remarks>
		/// - The table name is derived from the <see cref="DataContractAttribute.Name"/> on the <typeparamref name="TRow"/>.
		/// - The database name is derived from the <see cref="DataContractAttribute.Namespace"/> on the <typeparamref name="TRow"/>.
		/// </remarks>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildCreateTableQuery<TRow>()
			where TRow : class;

		/// <summary>
		/// Builds a query for adding a column to a table.
		/// </summary>
		/// <remarks>
		/// - The table name is derived from the <see cref="DataContractAttribute.Name"/> on the <typeparamref name="TRow"/>.
		/// - The database name is derived from the <see cref="DataContractAttribute.Namespace"/> on the <typeparamref name="TRow"/>.
		/// </remarks>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <param name="propertyName">The name of the property in <typeparamref name="TRow"/> to create the column query from.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="propertyName"/> is not a valid property on <typeparamref name="TRow"/>.
		/// </exception>
		ISqlQuery BuildAddColumnQuery<TRow>(string propertyName)
			where TRow : class;

		/// <summary>
		/// Builds a query for dropping a column from a table.
		/// </summary>
		/// <param name="databaseName">The name of the database the table is in.</param>
		/// <param name="tableName">The name of the table to drop the column from.</param>
		/// <param name="columnName">The name of the column to drop.</param>
		/// <returns>The <see cref="ISqlQuery"/>.</returns>
		ISqlQuery BuildDropColumnQuery(string databaseName, string tableName, string columnName);

		/// <summary>
		/// Builds a <see cref="LambdaExpression"/> for a where clause from an <see cref="Expression{TDelegate}"/>.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <returns>The <see cref="LambdaExpression"/>.</returns>
		LambdaExpression BuildWhereClause<TRow>(Expression<Func<TRow, bool>> expression)
			where TRow : class;

		/// <summary>
		/// Builds a <see cref="LambdaExpression"/> for a where clause from an <see cref="Expression{TDelegate}"/>.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <returns>The <see cref="LambdaExpression"/>.</returns>
		LambdaExpression BuildWhereClause<TRow, TP1>(Expression<Func<TRow, TP1, bool>> expression)
			where TRow : class;

		/// <summary>
		/// Builds a <see cref="LambdaExpression"/> for a where clause from an <see cref="Expression{TDelegate}"/>.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <returns>The <see cref="LambdaExpression"/>.</returns>
		LambdaExpression BuildWhereClause<TRow, TP1, TP2>(Expression<Func<TRow, TP1, TP2, bool>> expression)
			where TRow : class;

		/// <summary>
		/// Builds a <see cref="LambdaExpression"/> for a where clause from an <see cref="Expression{TDelegate}"/>.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <typeparam name="TP3">The third query parameter.</typeparam>
		/// <returns>The <see cref="LambdaExpression"/>.</returns>
		LambdaExpression BuildWhereClause<TRow, TP1, TP2, TP3>(Expression<Func<TRow, TP1, TP2, TP3, bool>> expression)
			where TRow : class;

		/// <summary>
		/// Builds a <see cref="LambdaExpression"/> for a where clause from an <see cref="Expression{TDelegate}"/>.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <typeparam name="TP3">The third query parameter.</typeparam>
		/// <typeparam name="TP4">The fourth query parameter.</typeparam>
		/// <returns>The <see cref="LambdaExpression"/>.</returns>
		LambdaExpression BuildWhereClause<TRow, TP1, TP2, TP3, TP4>(Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> expression)
			where TRow : class;

		/// <summary>
		/// Builds a <see cref="LambdaExpression"/> for a where clause from an <see cref="Expression{TDelegate}"/>.
		/// </summary>
		/// <typeparam name="TRow">The a model of the expected table row.</typeparam>
		/// <typeparam name="TP1">The first query parameter.</typeparam>
		/// <typeparam name="TP2">The second query parameter.</typeparam>
		/// <typeparam name="TP3">The third query parameter.</typeparam>
		/// <typeparam name="TP4">The fourth query parameter.</typeparam>
		/// <typeparam name="TP5">The fifth query parameter.</typeparam>
		/// <returns>The <see cref="LambdaExpression"/>.</returns>
		LambdaExpression BuildWhereClause<TRow, TP1, TP2, TP3, TP4, TP5>(Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> expression)
			where TRow : class;
	}
}
