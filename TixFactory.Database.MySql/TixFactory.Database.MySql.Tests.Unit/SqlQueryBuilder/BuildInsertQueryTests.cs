using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> BuildInsertQueryTestCases
		{
			get
			{
				yield return new TestCaseData()
					.SetName("{m}_ValidTableClass_ReturnsInsertQuery")
					.Returns(GetQuery("InsertQuery"));
			}
		}

		[TestCaseSource(nameof(BuildInsertQueryTestCases))]
		public string BuildInsertQuery()
		{
			var query = _SqlQueryBuilder.BuildInsertQuery<TestTable>();

			Assert.That(query.Parameters.Select(p => p.Name), Is.EquivalentTo(new[] { "Name", "Description", "Value" }));
			Assert.That(query.Parameters.Select(p => p.DatabaseTypeName), Is.EquivalentTo(new[] { "VARBINARY(50)", "TEXT", "INTEGER NULL" }));

			return query.Query;
		}

		private static IEnumerable<TestCaseData> BuildInsertStoredProcedureTestCases
		{
			get
			{
				yield return new TestCaseData(false)
					.SetName("{m}_ValidTableClass_ReturnsInsertStoredProcedure")
					.Returns(GetQuery("InsertStoredProcedure"));
			}
		}

		[TestCaseSource(nameof(BuildInsertStoredProcedureTestCases))]
		public string BuildInsertStoredProcedure(bool useDelimiter)
		{
			var query = _SqlQueryBuilder.BuildInsertQuery<TestTable>();
			var insertStoredProcedure = _SqlQueryBuilder.BuildCreateStoredProcedureQuery(_DatabaseName, "test_table_insert", query, useDelimiter);

			Assert.That(insertStoredProcedure.Parameters, Is.Empty);
			return insertStoredProcedure.Query;
		}
	}
}
