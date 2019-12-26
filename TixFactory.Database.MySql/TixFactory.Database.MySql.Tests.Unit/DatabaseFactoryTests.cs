using System;
using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using Newtonsoft.Json;
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
			var testTable = testDatabase.GetTable("test_table");
			var storedProcedures = testDatabase.GetStoredProcedureNames();
			
			var sqlQueryBuilder = new SqlQueryBuilder(new DatabaseTypeParser());
			var createTableQuery = sqlQueryBuilder.BuildCreateTableQuery<TestTable>(testDatabase.Name);
			var selectAllQuery = sqlQueryBuilder.BuildSelectTopQuery(testDatabase.Name, testTable.Name, (TestTable row, long id) => row.Id > id, new OrderBy<TestTable>(nameof(TestTable.Id), SortOrder.Ascending));
			var selectedPagedQuery = sqlQueryBuilder.BuildSelectPagedQuery(testDatabase.Name, testTable.Name, (TestTable row, long id) => row.Id < id, new OrderBy<TestTable>(nameof(TestTable.Id)));
			var deleteQuery = sqlQueryBuilder.BuildDeleteQuery(testDatabase.Name, testTable.Name, (TestTable row, long id) => row.Id == id);
			var insertQuery = sqlQueryBuilder.BuildInsertQuery<TestTable>(testDatabase.Name, testTable.Name);
			var updateQuery = sqlQueryBuilder.BuildUpdateQuery(testDatabase.Name, testTable.Name, (TestTable row, long id) => row.Id == id);
			var countQuery = sqlQueryBuilder.BuildCountQuery(testDatabase.Name, testTable.Name);
			var countWhereQuery = sqlQueryBuilder.BuildCountQuery<TestTable>(testDatabase.Name, testTable.Name, (Expression<Func<TestTable, long, bool>>)((row, id) => row.Id > id));
			var countWhereQuery2 = sqlQueryBuilder.BuildCountQuery<TestTable>(testDatabase.Name, testTable.Name, sqlQueryBuilder.BuildWhereClause((TestTable row, long id) => row.Id > id));
			var addColumnQuery = sqlQueryBuilder.BuildAddColumnQuery<TestTable>(testDatabase.Name, nameof(TestTable.Name));
			var dropColumnQuery = sqlQueryBuilder.BuildDropColumnQuery(testDatabase.Name, testTable.Name, "Name");

			/*var indexColumns = new[]
			{
				testTable.GetColumn("name"),
				testTable.GetColumn("otherName")
			};
			var createIndexResult = testTable.CreateIndex("test_unique_index", unique: true, columns: indexColumns);*/

			var queryResult = databaseServerConnection.ExecuteQuery<TestTable>(selectAllQuery.Query, new Dictionary<string, object>
			{
				{ "id", 0 },
				{ "Count", int.MaxValue }
			});

			var serializedResult = JsonConvert.SerializeObject(queryResult);

			var registered = testDatabase.RegisterStoredProcedure("test_paged_procedure", selectedPagedQuery);
			//var registered = testDatabase.RegisterStoredProcedure("test_stored_procedure", selectAllQuery);
			//var dropped = testDatabase.DropStoredProcedure("test_stored_procedure");

			var mySqlParameters = new Dictionary<string, object>
			{
				{ "_id", int.MaxValue },
				{ "_ExclusiveStart", 2 },
				{ "_IsAscending", false },
				{ "_Count", 1 }
			};

			var result = databaseServerConnection.ExecuteStoredProcedure<object>("test_paged_procedure", mySqlParameters);

		}
	}
}
