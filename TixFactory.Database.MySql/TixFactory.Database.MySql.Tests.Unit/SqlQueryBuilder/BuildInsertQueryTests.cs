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
	}
}
