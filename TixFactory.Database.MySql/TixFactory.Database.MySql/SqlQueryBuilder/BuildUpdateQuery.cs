using System;
using System.Linq;
using System.Linq.Expressions;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildUpdateQuery{TRow}"/>
		public ISqlQuery BuildUpdateQuery<TRow>(LambdaExpression whereExpression)
			where TRow : class
		{
			if (whereExpression == null)
			{
				throw new ArgumentNullException(nameof(whereExpression));
			}

			var updateColumns = GetInsertColumns<TRow>(isUpdate: true);
			var (tableName, databaseName) = GetTableNameAndDatabaseName<TRow>(nameof(TRow));
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var (whereClause, expressionParameters) = ParseWhereExpression<TRow>(whereExpression, nameof(whereExpression), entityColumnAliases);

			var templateVariables = new UpdateQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				Columns = updateColumns.Where(c => !string.IsNullOrWhiteSpace(c.InsertValue)).ToArray(),
				WhereClause = whereClause
			};

			var query = CompileTemplate<UpdateQuery>(templateVariables);

			var parameters = updateColumns.Where(c => !string.IsNullOrWhiteSpace(c.ParameterName)).Select(TranslateParameter).ToList();
			var whereClauseParameters = expressionParameters.Skip(1).Where(p => parameters.All(existingParameter => !existingParameter.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
			parameters.AddRange(whereClauseParameters.Select(TranslateParameter));

			return new SqlQuery(query, parameters);
		}
	}
}
