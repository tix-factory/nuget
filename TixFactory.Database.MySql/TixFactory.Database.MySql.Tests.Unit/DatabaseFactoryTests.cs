using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

			var sqlQueryBuilder = new SqlQueryBuilder(new DatabaseTypeParser());
			var selectAllQuery = sqlQueryBuilder.BuildSelectTopQuery(testDatabase.Name, testTable.Name, (TestTable row, long id) => row.Id > id, new OrderBy<TestTable>(nameof(TestTable.Id), SortOrder.Ascending));

			var registered = testDatabase.RegisterStoredProcedure("test_stored_procedure", selectAllQuery);
			//var dropped = testDatabase.DropStoredProcedure("test_stored_procedure");

			var mySqlParameters = new Dictionary<string, object>
			{
				{ "_id", 0 },
				{ "_Count", 1 }
			};

			var result = databaseServerConnection.ExecuteStoredProcedure<object>("test_stored_procedure", mySqlParameters);

			var query = databaseServerConnection.ExecuteQuery<object>("SELECT @_id, @_Count;", mySqlParameters);

		}
	}
}
