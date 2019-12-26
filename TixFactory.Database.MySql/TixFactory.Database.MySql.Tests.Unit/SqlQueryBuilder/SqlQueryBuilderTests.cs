using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[TestFixture, ExcludeFromCodeCoverage]
	public partial class SqlQueryBuilderTests
	{
		private const string _DatabaseName = "test_database";
		private const string _TableName = "test_table";
		private const string _ColumnName = "test_column";

		private const string _NoParameterWhereClause = "(`ID` > 0)";
		private const string _OneParameterWhereClause = "(`ID` > @id)";
		private const string _TwoParameterWhereClause = "((`ID` > @id) AND (`Value` = @value))";
		private const string _ThreeParameterWhereClause = "(((`ID` > @id) AND (`Value` = @value)) AND LOWER(`Name`) LIKE CONCAT(LOWER(@name), '%'))";
		private const string _FourParameterWhereClause = "((((`ID` > @id) AND (`Value` = @value)) AND LOWER(`Name`) LIKE CONCAT(LOWER(@name), '%')) AND LOWER(`Description`) LIKE CONCAT('%', LOWER(@description), '%'))";
		private const string _FiveParameterWhereClause = "(((((`ID` > @id) AND (`Value` = @value)) AND LOWER(`Name`) LIKE CONCAT(LOWER(@name), '%')) AND LOWER(`Description`) LIKE CONCAT('%', LOWER(@description), '%')) AND (`Created` > @created))";

		private static readonly Expression<Func<TestTable, bool>> _WhereExpressionWithoutParameters = (row) => row.Id > 0;
		private static readonly Expression<Func<TestTable, long, bool>> _WhereExpressionWithOneParameter = (row, id) => row.Id > id;
		private static readonly Expression<Func<TestTable, long, int?, bool>> _WhereExpressionWithTwoParameters = (row, id, value) => row.Id > id && row.Value == value;
		private static readonly Expression<Func<TestTable, long, int?, string, bool>> _WhereExpressionWithThreeParameters = (row, id, value, name) => row.Id > id && row.Value == value && row.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase);
		private static readonly Expression<Func<TestTable, long, int?, string, string, bool>> _WhereExpressionWithFourParameters = (row, id, value, name, description) => row.Id > id && row.Value == value && row.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase) && row.Description.Contains(description, StringComparison.OrdinalIgnoreCase);
		private static readonly Expression<Func<TestTable, long, int?, string, string, DateTime, bool>> _WhereExpressionWithFiveParameters = (row, id, value, name, description, created) => row.Id > id && row.Value == value && row.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase) && row.Description.Contains(description, StringComparison.OrdinalIgnoreCase) && row.Created > created;

		private IDatabaseTypeParser _DatabaseTypeParser;
		private SqlQueryBuilder _SqlQueryBuilder;

		[SetUp]
		public void SetUp()
		{
			_DatabaseTypeParser = new DatabaseTypeParser();
			_SqlQueryBuilder = new SqlQueryBuilder(_DatabaseTypeParser);
		}
	}
}
