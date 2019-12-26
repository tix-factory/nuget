using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> BuildSelectPagedQueryTestCases
		{
			get
			{
				yield return new TestCaseData(null)
					.SetName("{m}_NullWhereClause_ReturnsSelectQuery")
					.Returns(GetQuery("SelectPagedQuery"));

				yield return new TestCaseData(_WhereExpression)
					.SetName("{m}_WithWhereClause_ReturnsSelectQuery")
					.Returns(GetQuery("SelectPagedQueryWithWhereClause"));
			}
		}

		[TestCaseSource(nameof(BuildSelectPagedQueryTestCases))]
		public string BuildSelectPagedQuery(LambdaExpression whereExpression)
		{
			var query = _SqlQueryBuilder.BuildSelectPagedQuery(new OrderBy<TestTable>(nameof(TestTable.Id)), whereExpression);

			if (whereExpression == null)
			{
				Assert.That(query.Parameters.Select(p => p.Name), Is.EquivalentTo(new[] { "IsAscending", "ExclusiveStart", "Count" }));
				Assert.That(query.Parameters.Select(p => p.DatabaseTypeName), Is.EquivalentTo(new[] { "BIT", "BIGINT", "INTEGER" }));
			}
			else
			{
				Assert.That(query.Parameters.Count, Is.EqualTo(whereExpression.Parameters.Count + 2));
			}

			return query.Query;
		}

		private static IEnumerable<TestCaseData> BuildSelectPagedStordProcedureTestCases
		{
			get
			{
				yield return new TestCaseData(_WhereExpression, false)
					.SetName("{m}_WithWhereClause_ReturnsSelectQuery")
					.Returns(GetQuery("SelectPagedWithWhereClauseStoredProcedure"));
			}
		}

		[TestCaseSource(nameof(BuildSelectPagedStordProcedureTestCases))]
		public string BuildSelectPagedStordProcedure(LambdaExpression whereExpression, bool useDelimiter)
		{
			var query = _SqlQueryBuilder.BuildSelectPagedQuery(new OrderBy<TestTable>(nameof(TestTable.Id)), whereExpression);
			var storedProcedure = _SqlQueryBuilder.BuildCreateStoredProcedureQuery(_DatabaseName, "test_table_select_paged", query, useDelimiter);

			Assert.That(storedProcedure.Parameters, Is.Empty);
			return storedProcedure.Query;
		}
	}
}
