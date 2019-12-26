using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> BuildSelectTopQueryTestCases
		{
			get
			{
				yield return new TestCaseData(null)
					.SetName("{m}_NullWhereClause_ReturnsSelectQuery")
					.Returns(GetQuery("SelectTopQuery"));

				yield return new TestCaseData(_WhereExpression)
					.SetName("{m}_WithWhereClause_ReturnsSelectQuery")
					.Returns(GetQuery("SelectTopQueryWithWhereClause"));
			}
		}

		[TestCaseSource(nameof(BuildSelectTopQueryTestCases))]
		public string BuildSelectTopQuery(LambdaExpression whereExpression)
		{
			var query = _SqlQueryBuilder.BuildSelectTopQuery<TestTable>(whereExpression);

			if (whereExpression == null)
			{
				Assert.That(query.Parameters.Select(p => p.Name), Is.EquivalentTo(new[] { "Count" }));
				Assert.That(query.Parameters.Select(p => p.DatabaseTypeName), Is.EquivalentTo(new[] { "INTEGER" }));
			}
			else
			{
				Assert.That(query.Parameters.Count, Is.EqualTo(whereExpression.Parameters.Count));
			}

			return query.Query;
		}
	}
}
