using System;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FakeItEasy;
using TixFactory.Configuration;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[TestFixture, ExcludeFromCodeCoverage]
	public class DatabaseFactoryTests
	{
		private IDatabaseServerConnection _DatabaseServerConnection;
		private IDatabaseNameValidator _DatabaseNameValidator;
		private IDatabaseTypeParser _DatabaseTypeParser;
		private DatabaseFactory _DatabaseFactory;

		[SetUp]
		public void SetUp()
		{
			_DatabaseServerConnection = A.Fake<IDatabaseServerConnection>();
			_DatabaseNameValidator = A.Fake<IDatabaseNameValidator>();
			_DatabaseTypeParser = A.Fake<IDatabaseTypeParser>();
			_DatabaseFactory = new DatabaseFactory(_DatabaseServerConnection, _DatabaseNameValidator, _DatabaseTypeParser);
		}

		[Test]
		[Ignore("TODO: Implement real tests.")]
		public void FakeTest()
		{
			var connectionString = File.ReadAllText("testconnectionstring.txt");
			var databaseServerConnection = new DatabaseServerConnection(new Setting<string>($"{connectionString};database=test1234"));
			var databaseFactory = databaseServerConnection.BuildDatabaseFactory();

			/*var testDatabase = databaseFactory.GetOrCreateDatabase("teamcity");
			var usersTable = testDatabase.GetTable("vcs_username");
			var columns = usersTable.GetAllColumns();
			var indexes = usersTable.GetAllIndexes();*/

			var testDatabase = databaseServerConnection.GetConnectedDatabase();
			var testTable = testDatabase.GetTable("new_table");
			var storedProcedures = testDatabase.GetStoredProcedureNames();
			var result = databaseServerConnection.ExecuteStoredProcedure<object>(storedProcedures.First(), queryParameters: null);

			var sqlQueryBuilder = new SqlQueryBuilder();
			var selectAllQuery = sqlQueryBuilder.BuildSelectAllQuery(testDatabase.Name, testTable.Name, (TestTable row, long id) => row.Id > id, new OrderBy<TestTable>(nameof(TestTable.Id), SortOrder.Ascending));
		}
	}
}
