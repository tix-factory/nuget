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
			var insertColumns = GetInsertColumns<TRow>(isUpdate: false);
			var templateVariables = new InsertQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				Columns = insertColumns.Where(c => !string.IsNullOrWhiteSpace(c.InsertValue)).ToArray()
			};

			var query = CompileTemplate<InsertQuery>(templateVariables);
			var parameters = insertColumns.Where(c => !string.IsNullOrWhiteSpace(c.ParameterName)).Select(TranslateParameter).ToList();

			return new SqlQuery(query, parameters);
		}

		private IReadOnlyCollection<InsertColumn> GetInsertColumns<TRow>(bool isUpdate)
			where TRow : class
		{
			var properties = typeof(TRow).GetProperties();
			return properties.Select(p => new InsertColumn(p, isUpdate)).ToArray();
		}

		private SqlQueryParameter TranslateParameter(InsertColumn column)
		{
			var mySqlType = _DatabaseTypeParser.GetMySqlType(column.Property.PropertyType);
			var databaseTypeName = _DatabaseTypeParser.GetDatabaseTypeName(mySqlType);
			return new SqlQueryParameter(column.ParameterName, databaseTypeName, length: null, parameterDirection: ParameterDirection.Input);
		}
	}
}
