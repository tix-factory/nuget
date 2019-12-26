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
		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow}"/>
		public ISqlQuery BuildCountQuery<TRow>(LambdaExpression whereExpression = null)
			where TRow : class
		{
			var (tableName, databaseName) = GetTableNameAndDatabaseName<TRow>(nameof(TRow));
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var (whereClause, expressionParameters) = ParseWhereExpression<TRow>(whereExpression, nameof(whereExpression), entityColumnAliases);
			
			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				expressionParameters);
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
