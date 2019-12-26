using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;

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
				yield return new TestCaseData(_WhereExpressionWithoutParameters)
					.SetName("{m}_WhereWithNoParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _NoParameterWhereClause + ";");

				yield return new TestCaseData(_WhereExpressionWithOneParameter)
					.SetName("{m}_WhereWithOneParameter_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _OneParameterWhereClause + ";");

				yield return new TestCaseData(_WhereExpressionWithTwoParameters)
					.SetName("{m}_WhereWithTwoParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _TwoParameterWhereClause + ";");

				yield return new TestCaseData(_WhereExpressionWithThreeParameters)
					.SetName("{m}_WhereWithThreeParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _ThreeParameterWhereClause + ";");

				yield return new TestCaseData(_WhereExpressionWithFourParameters)
					.SetName("{m}_WhereWithFourParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _FourParameterWhereClause + ";");

				yield return new TestCaseData(_WhereExpressionWithFiveParameters)
					.SetName("{m}_WhereWithFiveParameters_ReturnsCountQuery")
					.Returns("SELECT COUNT(*) as `Count`\r\n\tFROM `" + _DatabaseName + "`.`" + _TableName + "`\r\n\tWHERE " + _FiveParameterWhereClause + ";");
			}
		}

		[TestCaseSource(nameof(BuildCountQueryTestCases))]
		public string BuildCountQuery(LambdaExpression whereExpression)
		{
			var query = _SqlQueryBuilder.BuildCountQuery<TestTable>(whereExpression);

			Assert.That(query.Parameters.Count, Is.EqualTo(whereExpression.Parameters.Count - 1));
			return query.Query;
		}
	}
}
