using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseFactory"/>
	public class DatabaseFactory : IDatabaseFactory
	{
		private readonly IDatabaseServerConnection _DatabaseServerConnection;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;
		private readonly ConcurrentDictionary<string, IDatabase> _Databases;

		/// <summary>
		/// Initializes a new <see cref="DatabaseFactory"/>.
		/// </summary>
		/// <param name="databaseServerConnection">An <see cref="IDatabaseServerConnection"/>.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseServerConnection"/>
		/// - <paramref name="databaseNameValidator"/>
		/// </exception>
		public DatabaseFactory(IDatabaseServerConnection databaseServerConnection, IDatabaseNameValidator databaseNameValidator)
		{
			_DatabaseServerConnection = databaseServerConnection ?? throw new ArgumentNullException(nameof(databaseServerConnection));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));
			_Databases = new ConcurrentDictionary<string, IDatabase>(StringComparer.OrdinalIgnoreCase);
		}

		/// <inheritdoc cref="IDatabaseFactory.GetDatabase"/>
		public IDatabase GetDatabase(string databaseName)
		{
			if (!_DatabaseNameValidator.IsDatabaseNameValid(databaseName))
			{
				return null;
			}

			throw new System.NotImplementedException();
		}

		/// <inheritdoc cref="IDatabaseFactory.CreateDatabase"/>
		public IDatabase CreateDatabase(string databaseName)
		{
			if (!_DatabaseNameValidator.IsDatabaseNameValid(databaseName))
			{
				throw new ArgumentException($"'{nameof(databaseName)}' does not pass database naming validation.", nameof(databaseName));
			}

			throw new System.NotImplementedException();
		}

		/// <inheritdoc cref="IDatabaseFactory.GetOrCreateDatabase"/>
		public IDatabase GetOrCreateDatabase(string databaseName)
		{
			if (!_DatabaseNameValidator.IsDatabaseNameValid(databaseName))
			{
				throw new ArgumentException($"'{nameof(databaseName)}' does not pass database naming validation.", nameof(databaseName));
			}

			throw new System.NotImplementedException();
		}

		/// <inheritdoc cref="IDatabaseFactory.GetAllDatabases"/>
		public IReadOnlyCollection<IDatabase> GetAllDatabases()
		{
			throw new System.NotImplementedException();
		}
	}
}
