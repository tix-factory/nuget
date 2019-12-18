using System;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabase"/>
	internal class Database : IDatabase
	{
		private readonly IDatabaseServerConnection _DatabaseServerConnection;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;
		private readonly string _DatabaseName;

		public Database(IDatabaseServerConnection databaseServerConnection, IDatabaseNameValidator databaseNameValidator, string databaseName)
		{
			_DatabaseServerConnection = databaseServerConnection ?? throw new ArgumentNullException(nameof(databaseServerConnection));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));

			if (databaseNameValidator.IsDatabaseNameValid(databaseName))
			{
				throw new ArgumentException($"'{nameof(databaseName)}' is invalid.", nameof(databaseName));
			}

			_DatabaseName = databaseName;
		}
	}
}
