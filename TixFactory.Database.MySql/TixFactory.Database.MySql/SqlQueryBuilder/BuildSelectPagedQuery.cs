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
		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectPagedQuery{TRow}"/>
		public ISqlQuery BuildSelectPagedQuery<TRow>(OrderBy<TRow> orderBy, LambdaExpression whereExpression = null)
			where TRow : class
		{
			if (orderBy == null)
			{
				throw new ArgumentNullException(nameof(orderBy));
			}

			var (tableName, databaseName) = GetTableNameAndDatabaseName<TRow>(nameof(TRow));
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var (whereClause, expressionParameters) = ParseWhereExpression<TRow>(whereExpression, nameof(whereExpression), entityColumnAliases);

			return BuildSelectPagedQuery(
				databaseName,
				tableName,
				whereClause,
				orderBy,
				entityColumnAliases,
				expressionParameters);
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
