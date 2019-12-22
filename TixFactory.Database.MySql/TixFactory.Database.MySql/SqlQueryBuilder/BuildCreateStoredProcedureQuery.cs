using System;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildCreateStoredProcedureQuery"/>
		public ISqlQuery BuildCreateStoredProcedureQuery(string databaseName, string storedProcedureName, ISqlQuery query, bool useDelimiter)
		{
			var templateVariables = new CreateProcedureVariables
			{
				DatabaseName = databaseName,
				StoredProcedureName = storedProcedureName,
				Query = query.Query,
				Parameters = query.Parameters,
				Delimiter = useDelimiter ? "$$" : ";"
			};

			var createQuery = CompileTemplate<CreateProcedureQuery>(templateVariables);
			return new SqlQuery(createQuery, Array.Empty<SqlQueryParameter>());
		}
	}
}
