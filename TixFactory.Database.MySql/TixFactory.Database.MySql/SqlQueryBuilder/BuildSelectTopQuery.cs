using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow}"/>
		public ISqlQuery BuildSelectTopQuery<TRow>(LambdaExpression whereExpression = null, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var (tableName, databaseName) = GetTableNameAndDatabaseName<TRow>(nameof(TRow));
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var (whereClause, expressionParameters) = ParseWhereExpression<TRow>(whereExpression, nameof(whereExpression), entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			var templateVariables = new SelectQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				WhereClause = whereClause,
				OrderBy = orderByStatement
			};

			var query = CompileTemplate<SelectTopQuery>(templateVariables);
			var parameters = expressionParameters.Skip(1).Select(TranslateParameter).ToList();

			parameters.Add(new SqlQueryParameter(_CountParameterName, _DatabaseTypeParser.GetDatabaseTypeName(MySqlDbType.Int32), length: null, parameterDirection: ParameterDirection.Input));

			return new SqlQuery(query, parameters);
		}
	}
}
