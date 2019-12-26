using NUnit.Framework;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		[TestCase(_ColumnName, ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tDROP COLUMN `" + _ColumnName + "`;")]
		public string BuildDropColumnQuery_ValidColumnName_ReturnsDropColumnQuery(string columnName)
		{
			var query = _SqlQueryBuilder.BuildDropColumnQuery(_DatabaseName, _TableName, columnName);

			Assert.That(query.Parameters, Is.Empty, "Expected DROP COLUMN query to not have parameters.");
			return query.Query;
		}
	}
}
