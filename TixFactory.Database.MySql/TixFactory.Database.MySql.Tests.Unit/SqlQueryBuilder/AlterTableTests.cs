using NUnit.Framework;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		[TestCase("NOT A PROPERTY NAME")]
		public void BuildAddColumnQuery_InvalidPropertyName_Throws(string propertyName)
		{
			Assert.That(() => _SqlQueryBuilder.BuildAddColumnQuery<TestTable>(propertyName),
				Throws.ArgumentException.With.Property("ParamName").EqualTo(nameof(propertyName)));
		}

		[TestCase(nameof(TestTable.Id), ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tADD `ID` BIGINT AUTO_INCREMENT PRIMARY KEY;")]
		[TestCase(nameof(TestTable.Name), ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tADD `Name` VARBINARY(50)\n\tAFTER `ID`;")]
		[TestCase(nameof(TestTable.Description), ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tADD `Description` TEXT\n\tAFTER `Name`;")]
		[TestCase(nameof(TestTable.Value), ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tADD `Value` INTEGER NULL\n\tAFTER `Description`;")]
		[TestCase(nameof(TestTable.Created), ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tADD `Created` DATETIME\n\tAFTER `Value`;")]
		[TestCase(nameof(TestTable.Updated), ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tADD `Updated` DATETIME\n\tAFTER `Created`;")]
		public string BuildAddColumnQuery_ValidPropertyName_ReturnsAddColumnQuery(string propertyName)
		{
			var query = _SqlQueryBuilder.BuildAddColumnQuery<TestTable>(propertyName);

			Assert.That(query.Parameters, Is.Empty, "Expected ADD COLUMN query to not have parameters.");
			return query.Query;
		}

		[TestCase(_ColumnName, ExpectedResult = "ALTER TABLE `" + _DatabaseName + "`.`" + _TableName + "`\n\tDROP COLUMN `" + _ColumnName + "`;")]
		public string BuildDropColumnQuery_ValidColumnName_ReturnsDropColumnQuery(string columnName)
		{
			var query = _SqlQueryBuilder.BuildDropColumnQuery(_DatabaseName, _TableName, columnName);

			Assert.That(query.Parameters, Is.Empty, "Expected DROP COLUMN query to not have parameters.");
			return query.Query;
		}
	}
}
