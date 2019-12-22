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
		/// <inheritdoc cref="ISqlQueryBuilder.BuildDeleteQuery{TRow,TP1}"/>
		public ISqlQuery BuildDeleteQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, whereExpression.Parameters, entityColumnAliases);

			return BuildDeleteQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		private ISqlQuery BuildDeleteQuery(string databaseName, string tableName, string whereClause, IReadOnlyCollection<ParameterExpression> expressionParameters)
		{
			var templateVariables = new DeleteQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				WhereClause = whereClause
			};

			var query = CompileTemplate<DeleteQuery>(templateVariables);

			var parameters = expressionParameters.Skip(1).Select(p =>
			{
				var mySqlType = _DatabaseTypeParser.GetMySqlType(p.Type);
				var databaseTypeName = _DatabaseTypeParser.GetDatabaseTypeName(mySqlType);
				var parameter = new SqlQueryParameter(p.Name, databaseTypeName, length: null, parameterDirection: ParameterDirection.Input);

				return parameter;
			}).ToList();

			return new SqlQuery(query, parameters);
		}
	}
}
