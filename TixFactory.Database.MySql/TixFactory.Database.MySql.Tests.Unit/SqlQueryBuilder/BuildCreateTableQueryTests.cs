using NUnit.Framework;
using System.Collections.Generic;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> BuildCreateTableQueryTestCases
		{
			get
			{
				yield return new TestCaseData()
					.SetName("{m}_ValidTableRowClass_ReturnsCreateTableQuery")
					.Returns(GetQuery("CreateTableQuery"));
			}
		}

		[TestCaseSource(nameof(BuildCreateTableQueryTestCases))]
		public string BuildCreateTableQuery()
		{
			var query = _SqlQueryBuilder.BuildCreateTableQuery<TestTable>();

			Assert.That(query.Parameters, Is.Empty);
			return query.Query;
		}
	}
}
