using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow}(string,string,OrderBy{TRow})"/>
		public ISqlQuery BuildSelectTopQuery<TRow>(string databaseName, string tableName, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause: null,
				orderByStatement: ParseOrderBy(orderBy, entityColumnAliases),
				expressionParameters: Array.Empty<ParameterExpression>());
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow}(string,string,System.Linq.Expressions.Expression{System.Func{TRow,bool}},OrderBy{TRow})"/>
		public ISqlQuery BuildSelectTopQuery<TRow>(string databaseName, string tableName, Expression<Func<TRow, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1}"/>
		public ISqlQuery BuildSelectTopQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2}"/>
		public ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3}"/>
		public ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2, TP3>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3,TP4}"/>
		public ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3,TP4,TP5}"/>
		public ISqlQuery BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4, TP5>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement,
				whereExpression.Parameters);
		}

		private ISqlQuery BuildSelectAllQuery(string databaseName, string tableName, string whereClause, string orderByStatement, IReadOnlyCollection<ParameterExpression> expressionParameters)
		{
			var templateVariables = new SelectQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				WhereClause = whereClause,
				OrderBy = orderByStatement
			};

			var query = CompileTemplate<SelectTopQuery>(templateVariables);

			var parameters = expressionParameters.Skip(1).Select(p =>
			{
				var mySqlType = _DatabaseTypeParser.GetMySqlType(p.Type);
				var databaseTypeName = _DatabaseTypeParser.GetDatabaseTypeName(mySqlType);
				var parameter = new SqlQueryParameter(p.Name, databaseTypeName, length: null, parameterDirection: ParameterDirection.Input);

				return parameter;
			}).ToList();

			parameters.Add(new SqlQueryParameter(_CountParameterName, _DatabaseTypeParser.GetDatabaseTypeName(MySqlDbType.Int32), length: null, parameterDirection: ParameterDirection.Input));

			return new SqlQuery(query, parameters);
		}
	}
}
