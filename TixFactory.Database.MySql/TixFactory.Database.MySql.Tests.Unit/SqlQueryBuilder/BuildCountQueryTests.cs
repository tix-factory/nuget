using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> BuildCountQueryTestCases
		{
			get
			{
				yield return new TestCaseData(null)
					.SetName("{m}_NullWhereClause_ReturnsCountQuery")
					.Returns(GetQuery("CountQuery"));

				yield return new TestCaseData(_WhereExpression)
					.SetName("{m}_WithWhereClause_ReturnsCountQuery")
					.Returns(GetQuery("CountQueryWithWhereClause"));
			}
		}

		[TestCaseSource(nameof(BuildCountQueryTestCases))]
		public string BuildCountQuery(LambdaExpression whereExpression)
		{
			var query = _SqlQueryBuilder.BuildCountQuery<TestTable>(whereExpression);

			if (whereExpression == null)
			{
				Assert.That(query.Parameters, Is.Empty, "Expected no parameters for query without WHERE clause.");
			}
			else
			{
				Assert.That(query.Parameters.Count, Is.EqualTo(whereExpression.Parameters.Count - 1));
			}

			return query.Query;
		}
	}
}
