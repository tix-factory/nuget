using System;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildDropStoredProcedureQuery"/>
		public ISqlQuery BuildDropStoredProcedureQuery(string databaseName, string storedProcedureName)
		{
			return new SqlQuery($"DROP PROCEDURE `{databaseName}`.`{storedProcedureName}`;", Array.Empty<SqlQueryParameter>());
		}
	}
}
