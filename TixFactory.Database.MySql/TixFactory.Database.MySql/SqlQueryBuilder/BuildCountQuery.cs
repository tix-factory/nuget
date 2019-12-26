using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery"/>
		public ISqlQuery BuildCountQuery(string databaseName, string tableName)
		{
			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause: null,
				expressionParameters: Array.Empty<ParameterExpression>());
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow}(string,string,System.Linq.Expressions.Expression{System.Func{TRow,bool}})"/>
		public ISqlQuery BuildCountQuery<TRow>(string databaseName, string tableName, Expression<Func<TRow, bool>> whereExpression)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, entityColumnAliases);

			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow,TP1}"/>
		public ISqlQuery BuildCountQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, entityColumnAliases);

			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow,TP1,TP2}"/>
		public ISqlQuery BuildCountQuery<TRow, TP1, TP2>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, bool>> whereExpression)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, entityColumnAliases);

			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow,TP1,TP2,TP3}"/>
		public ISqlQuery BuildCountQuery<TRow, TP1, TP2, TP3>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, bool>> whereExpression)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, entityColumnAliases);

			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow,TP1,TP2,TP3,TP4}"/>
		public ISqlQuery BuildCountQuery<TRow, TP1, TP2, TP3, TP4>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> whereExpression)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, entityColumnAliases);

			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow,TP1,TP2,TP3,TP4,TP5}"/>
		public ISqlQuery BuildCountQuery<TRow, TP1, TP2, TP3, TP4, TP5>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> whereExpression)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, entityColumnAliases);

			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		private ISqlQuery BuildCountQuery(string databaseName, string tableName, string whereClause, IReadOnlyCollection<ParameterExpression> expressionParameters)
		{
			var templateVariables = new CountQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				WhereClause = whereClause
			};

			var query = CompileTemplate<CountQuery>(templateVariables);
			var parameters = expressionParameters.Skip(1).Select(TranslateParameter).ToList();

			return new SqlQuery(query, parameters);
		}
	}
}
