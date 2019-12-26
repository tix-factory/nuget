using System;
using System.Linq;
using System.Reflection;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildCreateTableQuery{TRow}"/>
		public ISqlQuery BuildCreateTableQuery<TRow>()
			where TRow : class
		{
			var (tableName, databaseName) = GetTableNameAndDatabaseName<TRow>(nameof(TRow));
			var columns = typeof(TRow).GetProperties().Select(TranslateToCreateTableColumn).ToArray();
			var variables = new CreateTableQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				Columns = columns
			};

			var query = CompileTemplate<CreateTableQuery>(variables);
			return new SqlQuery(query, Array.Empty<SqlQueryParameter>());
		}

		private TableColumn TranslateToCreateTableColumn(PropertyInfo property)
		{
			return new TableColumn(property, _DatabaseTypeParser);
		}
	}
}
