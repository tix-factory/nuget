using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseFactory"/>
	public class DatabaseFactory : IDatabaseFactory
	{

		public IDatabase GetDatabase(string databaseName)
		{
			throw new System.NotImplementedException();
		}

		public IDatabase CreateDatabase(string databaseName)
		{
			throw new System.NotImplementedException();
		}

		public IDatabase GetOrCreateDatabase(string databaseName)
		{
			throw new System.NotImplementedException();
		}

		public IReadOnlyCollection<IDatabase> GetAllDatabases()
		{
			throw new System.NotImplementedException();
		}
	}
}
