using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		[TestCase(ExpectedResult = "SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`;")]
		public string BuildCountQuery_NoParameters_ReturnsCountQuery()
		{
			var query = _SqlQueryBuilder.BuildCountQuery<TestTable>();

			Assert.That(query.Parameters, Is.Empty, "Expected COUNT query to not have parameters.");
			return query.Query;
		}

		private static IEnumerable<TestCaseData> BuildCountQueryTestCases
		{
			get
			{
				yield return new TestCaseData((Func<ISqlQueryBuilder, ISqlQuery>)(sqlQueryBuilder => sqlQueryBuilder.BuildCountQuery<TestTable>(_WhereExpressionWithoutParameters)))
					.SetName("BuildCountQuery_WhereWithNoParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _NoParameterWhereClause + ";");

				yield return new TestCaseData((Func<ISqlQueryBuilder, ISqlQuery>)(sqlQueryBuilder => sqlQueryBuilder.BuildCountQuery<TestTable>(_WhereExpressionWithOneParameter)))
					.SetName("BuildCountQuery_WhereWithOneParameter_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _OneParameterWhereClause + ";");

				yield return new TestCaseData((Func<ISqlQueryBuilder, ISqlQuery>)(sqlQueryBuilder => sqlQueryBuilder.BuildCountQuery<TestTable>(_WhereExpressionWithTwoParameters)))
					.SetName("BuildCountQuery_WhereWithTwoParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _TwoParameterWhereClause + ";");

				yield return new TestCaseData((Func<ISqlQueryBuilder, ISqlQuery>)(sqlQueryBuilder => sqlQueryBuilder.BuildCountQuery<TestTable>(_WhereExpressionWithThreeParameters)))
					.SetName("BuildCountQuery_WhereWithThreeParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _ThreeParameterWhereClause + ";");

				yield return new TestCaseData((Func<ISqlQueryBuilder, ISqlQuery>)(sqlQueryBuilder => sqlQueryBuilder.BuildCountQuery<TestTable>(_WhereExpressionWithFourParameters)))
					.SetName("BuildCountQuery_WhereWithFourParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _FourParameterWhereClause + ";");

				yield return new TestCaseData((Func<ISqlQueryBuilder, ISqlQuery>)(sqlQueryBuilder => sqlQueryBuilder.BuildCountQuery<TestTable>(_WhereExpressionWithFiveParameters)))
					.SetName("BuildCountQuery_WhereWithFiveParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _FiveParameterWhereClause + ";");
			}
		}

		[TestCaseSource(nameof(BuildCountQueryTestCases))]
		public string BuildCountQuery(Func<ISqlQueryBuilder, ISqlQuery> buildCountQuery)
		{
			var query = buildCountQuery(_SqlQueryBuilder);
			return query.Query;
		}
	}
}
