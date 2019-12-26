using System.Collections.Generic;
using System.Data;
using System.Linq;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildInsertQuery{TRow}"/>
		public ISqlQuery BuildInsertQuery<TRow>()
			where TRow : class
		{
			var insertColumns = GetInsertColumns<TRow>(isUpdate: false);
			var (tableName, databaseName) = GetTableNameAndDatabaseName<TRow>(nameof(TRow));

			var templateVariables = new InsertQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				Columns = insertColumns.Where(c => !string.IsNullOrWhiteSpace(c.InsertValue)).ToArray()
			};

			var primaryColumn = insertColumns.FirstOrDefault(c => c.Primary);
			if (primaryColumn != null)
			{
				templateVariables.PrimaryColumnName = primaryColumn.Name;
			}

			var query = CompileTemplate<InsertQuery>(templateVariables);
			var parameters = insertColumns.Where(c => !string.IsNullOrWhiteSpace(c.ParameterName)).Select(TranslateParameter).ToList();

			return new SqlQuery(query, parameters);
		}

		private IReadOnlyCollection<InsertColumn> GetInsertColumns<TRow>(bool isUpdate)
			where TRow : class
		{
			var properties = typeof(TRow).GetProperties();
			return properties.Select(p => new InsertColumn(p, isUpdate, _DatabaseTypeParser)).ToArray();
		}

		private SqlQueryParameter TranslateParameter(InsertColumn column)
		{
			return new SqlQueryParameter(column.ParameterName, column.DatabaseType, length: null, parameterDirection: ParameterDirection.Input);
		}
	}
}
