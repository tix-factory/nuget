using System;
using System.Linq;
using System.Reflection;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildAddColumnQuery{TRow}"/>
		public ISqlQuery BuildAddColumnQuery<TRow>(string databaseName, string propertyName)
			where TRow : class
		{
			var tableType = typeof(TRow);
			var property = tableType.GetProperty(propertyName);
			if (property == null)
			{
				throw new ArgumentException($"'{propertyName}' is not valid property on '{tableType.Name}' ({nameof(TRow)})", nameof(propertyName));
			}

			var tableName = GetTableName<TRow>();
			var beforeProperty = GetPropertyBefore<TRow>(property);
			var afterQuery = string.Empty;

			if (beforeProperty != null)
			{
				var beforeColumn = new CreateTableColumn(beforeProperty, _DatabaseTypeParser);
				afterQuery += $"\n\tAFTER `{beforeColumn.ColumnName}`";
			}

			var createColumn = new CreateTableColumn(property, _DatabaseTypeParser);
			var query = $"ALTER TABLE `{databaseName}`.`{tableName}`\n\tADD `{createColumn.ColumnName}` {createColumn.DatabaseType}";
			query += $"{afterQuery};";

			return new SqlQuery(query, Array.Empty<SqlQueryParameter>());
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildDropColumnQuery"/>
		public ISqlQuery BuildDropColumnQuery(string databaseName, string tableName, string columnName)
		{
			var query = $"ALTER TABLE `{databaseName}`.`{tableName}`\n\tDROP COLUMN `{columnName}`;";
			return new SqlQuery(query, Array.Empty<SqlQueryParameter>());
		}

		private PropertyInfo GetPropertyBefore<TRow>(PropertyInfo property)
			where TRow : class
		{
			var allProperties = typeof(TRow).GetProperties().ToList();
			var index = allProperties.IndexOf(property);
			if (index > 0)
			{
				return allProperties[index - 1];
			}

			return null;
		}
	}
}
