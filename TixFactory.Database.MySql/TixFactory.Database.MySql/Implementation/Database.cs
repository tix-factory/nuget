using System;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabase"/>
	internal class Database : IDatabase
	{
		private readonly IDatabaseServerConnection _DatabaseServerConnection;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;

		/// <inheritdoc cref="IDatabase.Name"/>
		public string Name { get; }

		/// <summary>
		/// Initializes a new <see cref="Database"/>.
		/// </summary>
		/// <param name="databaseServerConnection">An <see cref="IDatabaseServerConnection"/>.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <param name="databaseName">The database name.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseServerConnection"/>
		/// - <paramref name="databaseNameValidator"/>
		/// </exception>
		public Database(IDatabaseServerConnection databaseServerConnection, IDatabaseNameValidator databaseNameValidator, string databaseName)
		{
			_DatabaseServerConnection = databaseServerConnection ?? throw new ArgumentNullException(nameof(databaseServerConnection));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));
			Name = databaseName;
		}
	}
}
