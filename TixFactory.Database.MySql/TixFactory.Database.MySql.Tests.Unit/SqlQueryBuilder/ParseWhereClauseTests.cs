using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TixFactory.Database.MySql.Tests.Unit
{
	public partial class SqlQueryBuilderTests
	{
		private static IEnumerable<TestCaseData> ParseWhereClauseTestCases
		{
			get
			{
				yield return new TestCaseData((Expression<Func<TestTable, bool>>)((row) => row.Id > 123))
					.Returns("(`ID` > 123)")
					.SetName("{m}_NoParameterExpression_ReturnsParameterlessWhereClause");
			}
		}

		[TestCaseSource(nameof(ParseWhereClauseTestCases))]
		public string ParseWhereClause(LambdaExpression expression)
		{
			var entityColumnAliases = _SqlQueryBuilder.GetEntityColumnAliases<TestTable>();
			return _SqlQueryBuilder.ParseWhereClause(expression, entityColumnAliases);
		}
	}
}
