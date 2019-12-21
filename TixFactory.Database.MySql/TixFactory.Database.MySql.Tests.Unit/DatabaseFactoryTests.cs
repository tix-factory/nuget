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
		private DatabaseFactory _DatabaseFactory;

		[SetUp]
		public void SetUp()
		{
			_DatabaseServerConnection = A.Fake<IDatabaseServerConnection>();
			_DatabaseNameValidator = A.Fake<IDatabaseNameValidator>();
			_DatabaseFactory = new DatabaseFactory(_DatabaseServerConnection, _DatabaseNameValidator);
		}

		[Test]
		[Ignore("TODO: Implement real tests.")]
		public void FakeTest()
		{
			var databaseNameValidator = new DatabaseNameValidator();
			var connectionString = File.ReadAllText("testconnectionstring.txt");
			var databaseServerConnection = new DatabaseServerConnection(new Setting<string>($"{connectionString};database=test1234"), databaseNameValidator);
			var databaseFactory = new DatabaseFactory(databaseServerConnection, databaseNameValidator);

			/*var testDatabase = databaseFactory.GetOrCreateDatabase("teamcity");
			var usersTable = testDatabase.GetTable("vcs_username");
			var columns = usersTable.GetAllColumns();
			var indexes = usersTable.GetAllIndexes();*/

			var testDatabase = databaseFactory.GetOrCreateDatabase("test1234");
			var storedProcedures = testDatabase.GetStoredProcedureNames();
			var result = databaseServerConnection.ExecuteStoredProcedure<object>(storedProcedures.First(), queryParameters: null);

		}
	}
}
