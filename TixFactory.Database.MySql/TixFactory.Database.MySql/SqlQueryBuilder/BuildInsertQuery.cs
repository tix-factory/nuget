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
		public ISqlQuery BuildInsertQuery<TRow>(string databaseName, string tableName)
			where TRow : class
		{
			var insertColumns = GetInsertColumns<TRow>();
			var templateVariables = new InsertQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				Columns = insertColumns.Where(c => !string.IsNullOrWhiteSpace(c.InsertValue)).ToArray()
			};

			var query = CompileTemplate<InsertQuery>(templateVariables);

			var parameters = insertColumns.Where(c => !string.IsNullOrWhiteSpace(c.ParameterName)).Select(c =>
			{
				var mySqlType = _DatabaseTypeParser.GetMySqlType(c.Property.PropertyType);
				var databaseTypeName = _DatabaseTypeParser.GetDatabaseTypeName(mySqlType);
				var parameter = new SqlQueryParameter(c.ParameterName, databaseTypeName, length: null, parameterDirection: ParameterDirection.Input);

				return parameter;
			}).ToList();

			return new SqlQuery(query, parameters);
		}

		private IReadOnlyCollection<InsertColumn> GetInsertColumns<TRow>()
			where TRow : class
		{
			var properties = typeof(TRow).GetProperties();
			return properties.Select(p => new InsertColumn(p)).ToArray();
		}
	}
}
