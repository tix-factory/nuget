using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[TestFixture, ExcludeFromCodeCoverage]
	public partial class SqlQueryBuilderTests
	{
		private const string _DatabaseName = "test_database";
		private const string _TableName = "test_table";
		private const string _ColumnName = "test_column";

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
