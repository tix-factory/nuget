using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildCreateTableQuery{TRow}"/>
		public ISqlQuery BuildCreateTableQuery<TRow>(string databaseName)
			where TRow : class
		{
			var tableName = GetTableName<TRow>();
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentException($"'{nameof(TRow)}' must have '{nameof(DataContractAttribute)}' with '{nameof(DataContractAttribute.Name)}' set.", nameof(TRow));
			}

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

		private string GetTableName<TRow>()
		{
			var tableRowType = typeof(TRow);
			if (tableRowType.GetCustomAttribute(typeof(DataContractAttribute)) is DataContractAttribute dataContractAttribute)
			{
				return dataContractAttribute.Name;
			}

			return null;
		}

		private TableColumn TranslateToCreateTableColumn(PropertyInfo property)
		{
			return new TableColumn(property, _DatabaseTypeParser);
		}
	}
}
