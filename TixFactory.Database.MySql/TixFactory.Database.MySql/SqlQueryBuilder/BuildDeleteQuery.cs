using System;
using System.Linq;
using System.Linq.Expressions;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildDeleteQuery{TRow}"/>
		public ISqlQuery BuildDeleteQuery<TRow>(LambdaExpression whereExpression)
			where TRow : class
		{
			if (whereExpression == null)
			{
				throw new ArgumentNullException(nameof(whereExpression));
			}

			var (tableName, databaseName) = GetTableNameAndDatabaseName<TRow>(nameof(TRow));
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var (whereClause, expressionParameters) = ParseWhereExpression<TRow>(whereExpression, nameof(whereExpression), entityColumnAliases);

			var templateVariables = new DeleteQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				WhereClause = whereClause
			};

			var query = CompileTemplate<DeleteQuery>(templateVariables);
			var parameters = expressionParameters.Skip(1).Select(TranslateParameter).ToList();

			return new SqlQuery(query, parameters);
		}
	}
}
