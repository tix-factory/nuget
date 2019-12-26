using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[TestFixture, ExcludeFromCodeCoverage]
	public partial class SqlQueryBuilderTests
	{
		private const string _DatabaseName = TestTable._DatabaseName;
		private const string _TableName = TestTable._TableName;
		private const string _ColumnName = "test_column";
		
		private static readonly Expression<Func<TestTable, long, bool>> _WhereExpression = (row, id) => row.Id > id;

		private IDatabaseTypeParser _DatabaseTypeParser;
		private SqlQueryBuilder _SqlQueryBuilder;

		[SetUp]
		public void SetUp()
		{
			_DatabaseTypeParser = new DatabaseTypeParser();
			_SqlQueryBuilder = new SqlQueryBuilder(_DatabaseTypeParser);
		}

		private static string GetQuery(string queryName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = $"{typeof(SqlQueryBuilderTests).Namespace}.Queries.{queryName}.sql";

			using (var stream = assembly.GetManifestResourceStream(resourceName))
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
