using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> BuildDeleteQueryTestCases
		{
			get
			{
				yield return new TestCaseData()
					.SetName("{m}_WithWhereClause_ReturnsDeleteQuery")
					.Returns(GetQuery("DeleteQuery"));
			}
		}

		[TestCaseSource(nameof(BuildDeleteQueryTestCases))]
		public string BuildDeleteQuery()
		{
			var query = _SqlQueryBuilder.BuildDeleteQuery<TestTable>(_WhereExpression);

			Assert.That(query.Parameters.Select(p => p.Name), Is.EquivalentTo(new[] { "id" }));
			Assert.That(query.Parameters.Select(p => p.DatabaseTypeName), Is.EquivalentTo(new[] { "BIGINT" }));

			return query.Query;
		}
	}
}
