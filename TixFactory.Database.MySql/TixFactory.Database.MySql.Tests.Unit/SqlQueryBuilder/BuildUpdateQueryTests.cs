using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> BuildUpdateQueryTestCases
		{
			get
			{
				yield return new TestCaseData()
					.SetName("{m}_WithWhereClause_ReturnsUpdateQuery")
					.Returns(GetQuery("UpdateQuery"));
			}
		}

		[TestCaseSource(nameof(BuildUpdateQueryTestCases))]
		public string BuildUpdateQuery()
		{
			var query = _SqlQueryBuilder.BuildUpdateQuery<TestTable>(_WhereExpression);

			Assert.That(query.Parameters.Select(p => p.Name), Is.EquivalentTo(new[] { "Name", "Description", "Value", "id" }));
			Assert.That(query.Parameters.Select(p => p.DatabaseTypeName), Is.EquivalentTo(new[] { "VARBINARY(50)", "TEXT", "INTEGER NULL", "BIGINT" }));

			return query.Query;
		}
	}
}
