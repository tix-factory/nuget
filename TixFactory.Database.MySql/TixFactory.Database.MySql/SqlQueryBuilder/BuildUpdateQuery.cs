using System;
using System.Linq;
using System.Linq.Expressions;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildUpdateQuery{TRow,TP1}"/>
		public ISqlQuery BuildUpdateQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression)
			where TRow : class
		{
			var updateColumns = GetInsertColumns<TRow>(isUpdate: true);
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			var templateVariables = new UpdateQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				Columns = updateColumns.Where(c => !string.IsNullOrWhiteSpace(c.InsertValue)).ToArray(),
				WhereClause = whereClause
			};

			var query = CompileTemplate<UpdateQuery>(templateVariables);

			var parameters = updateColumns.Where(c => !string.IsNullOrWhiteSpace(c.ParameterName)).Select(TranslateParameter).ToList();
			var whereClauseParameters = whereExpression.Parameters.Skip(1).Where(p => parameters.All(existingParameter => !existingParameter.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
			parameters.AddRange(whereClauseParameters.Select(TranslateParameter));

			return new SqlQuery(query, parameters);
		}
	}
}
