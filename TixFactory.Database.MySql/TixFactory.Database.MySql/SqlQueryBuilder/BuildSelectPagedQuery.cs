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
		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectPagedQuery{TRow}(string,string,OrderBy{TRow})"/>
		public ISqlQuery BuildSelectPagedQuery<TRow>(string databaseName, string tableName, OrderBy<TRow> orderBy)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause: null,
				orderBy: orderBy,
				entityColumnAliases: entityColumnAliases,
				expressionParameters: Array.Empty<ParameterExpression>());
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectPagedQuery{TRow}(string,string,System.Linq.Expressions.Expression{System.Func{TRow,bool}},OrderBy{TRow})"/>
		public ISqlQuery BuildSelectPagedQuery<TRow>(string databaseName, string tableName, Expression<Func<TRow, bool>> whereExpression, OrderBy<TRow> orderBy)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause,
				orderBy,
				entityColumnAliases,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1}"/>
		public ISqlQuery BuildSelectPagedQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression, OrderBy<TRow> orderBy)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause,
				orderBy,
				entityColumnAliases,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2}"/>
		public ISqlQuery BuildSelectPagedQuery<TRow, TP1, TP2>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, bool>> whereExpression, OrderBy<TRow> orderBy)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause,
				orderBy,
				entityColumnAliases,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3}"/>
		public ISqlQuery BuildSelectPagedQuery<TRow, TP1, TP2, TP3>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, bool>> whereExpression, OrderBy<TRow> orderBy)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause,
				orderBy,
				entityColumnAliases,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3,TP4}"/>
		public ISqlQuery BuildSelectPagedQuery<TRow, TP1, TP2, TP3, TP4>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> whereExpression, OrderBy<TRow> orderBy)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause,
				orderBy,
				entityColumnAliases,
				whereExpression.Parameters);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3,TP4,TP5}"/>
		public ISqlQuery BuildSelectPagedQuery<TRow, TP1, TP2, TP3, TP4, TP5>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> whereExpression, OrderBy<TRow> orderBy)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause,
				orderBy,
				entityColumnAliases,
				whereExpression.Parameters);
		}

		private ISqlQuery BuildSelectPagedQuery<TRow>(string databaseName, string tableName, string whereClause, OrderBy<TRow> orderBy, IDictionary<string, string> entityColumnAliases, IReadOnlyCollection<ParameterExpression> expressionParameters)
			where TRow : class
		{
			var templateVariables = new SelectQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				WhereClause = whereClause,
				OrderBy = entityColumnAliases[orderBy.Property.Name]
			};

			var query = CompileTemplate<SelectPagedQuery>(templateVariables);
			var parameters = expressionParameters.Skip(1).Select(TranslateParameter).ToList();

			var exclusiveStartDatabaseType = _DatabaseTypeParser.GetMySqlType(orderBy.Property.PropertyType);
			parameters.Add(new SqlQueryParameter(_IsAscendingParameterName, _DatabaseTypeParser.GetDatabaseTypeName(MySqlDbType.Bit), length: null, parameterDirection: ParameterDirection.Input));
			parameters.Add(new SqlQueryParameter(_ExclusiveStartParameterName, _DatabaseTypeParser.GetDatabaseTypeName(exclusiveStartDatabaseType), length: null, parameterDirection: ParameterDirection.Input));
			parameters.Add(new SqlQueryParameter(_CountParameterName, _DatabaseTypeParser.GetDatabaseTypeName(MySqlDbType.Int32), length: null, parameterDirection: ParameterDirection.Input));

			return new SqlQuery(query, parameters);
		}
	}
}
