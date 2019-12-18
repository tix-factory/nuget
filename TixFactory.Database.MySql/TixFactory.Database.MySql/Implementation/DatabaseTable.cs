using System;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseTable"/>
	internal class DatabaseTable : IDatabaseTable
	{
		private readonly IDatabaseServerConnection _DatabaseServerConnection;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;

		/// <inheritdoc cref="IDatabaseTable.Name"/>
		public string Name { get; }

		/// <summary>
		/// Initializes a new <see cref="DatabaseTable"/>.
		/// </summary>
		/// <param name="databaseServerConnection">An <see cref="IDatabaseServerConnection"/>.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <param name="tableName">The table name.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseServerConnection"/>
		/// - <paramref name="databaseNameValidator"/>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// - <paramref name="tableName"/> is invalid.
		/// </exception>
		public DatabaseTable(IDatabaseServerConnection databaseServerConnection, IDatabaseNameValidator databaseNameValidator, string tableName)
		{
			_DatabaseServerConnection = databaseServerConnection ?? throw new ArgumentNullException(nameof(databaseServerConnection));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));

			if (databaseNameValidator.IsTableNameValid(tableName))
			{
				throw new ArgumentException($"'{nameof(tableName)}' is invalid.", nameof(tableName));
			}

			Name = tableName;
		}
	}
}
