using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseFactory"/>
	public class DatabaseFactory : IDatabaseFactory
	{
		private readonly IDatabaseServerConnection _DatabaseServerConnection;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;
		private readonly IDatabaseTypeParser _DatabaseTypeParser;
		private readonly ConcurrentDictionary<string, IDatabase> _Databases;

		/// <summary>
		/// Initializes a new <see cref="DatabaseFactory"/>.
		/// </summary>
		/// <param name="databaseServerConnection">An <see cref="IDatabaseServerConnection"/>.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <param name="databaseTypeParser">An <see cref="IDatabaseTypeParser"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseServerConnection"/>
		/// - <paramref name="databaseNameValidator"/>
		/// - <paramref name="databaseTypeParser"/>
		/// </exception>
		public DatabaseFactory(IDatabaseServerConnection databaseServerConnection, IDatabaseNameValidator databaseNameValidator, IDatabaseTypeParser databaseTypeParser)
		{
			_DatabaseServerConnection = databaseServerConnection ?? throw new ArgumentNullException(nameof(databaseServerConnection));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));
			_DatabaseTypeParser = databaseTypeParser ?? throw new ArgumentNullException(nameof(databaseTypeParser));
			_Databases = new ConcurrentDictionary<string, IDatabase>(StringComparer.OrdinalIgnoreCase);
		}

		/// <inheritdoc cref="IDatabaseFactory.GetDatabase"/>
		public IDatabase GetDatabase(string databaseName)
		{
			if (!_DatabaseNameValidator.IsDatabaseNameValid(databaseName))
			{
				return null;
			}

			SyncDatabases();
			_Databases.TryGetValue(databaseName, out var database);

			return database;
		}

		/// <inheritdoc cref="IDatabaseFactory.CreateDatabase"/>
		public IDatabase CreateDatabase(string databaseName)
		{
			if (!_DatabaseNameValidator.IsDatabaseNameValid(databaseName))
			{
				throw new ArgumentException($"'{nameof(databaseName)}' does not pass database naming validation.", nameof(databaseName));
			}

			var database = GetDatabase(databaseName);
			if (database != null)
			{
				throw new ArgumentException($"Database '{databaseName}' already exists.", nameof(databaseName));
			}

			try
			{
				var rowsAffected = _DatabaseServerConnection.ExecuteQuery($"CREATE SCHEMA `{databaseName}`;", queryParameters: null);
				if (rowsAffected != 1)
				{
					throw new ApplicationException($"Expected one row to be affected when creating the database.\n\rRows affected: {rowsAffected}\n\tDatabase name: {databaseName}");
				}
			}
			catch (MySqlException e) when (e.Code == (int)MySqlErrorCode.DatabaseCreateExists)
			{
				throw new ArgumentException($"Database '{databaseName}' already exists.", nameof(databaseName));
			}

			database = GetDatabase(databaseName);
			if (database == null)
			{
				throw new ApplicationException($"Database did not exist after create attempt.\n\rDatabase name: {databaseName}");
			}

			return database;
		}

		/// <inheritdoc cref="IDatabaseFactory.GetOrCreateDatabase"/>
		public IDatabase GetOrCreateDatabase(string databaseName)
		{
			if (!_DatabaseNameValidator.IsDatabaseNameValid(databaseName))
			{
				throw new ArgumentException($"'{nameof(databaseName)}' does not pass database naming validation.", nameof(databaseName));
			}

			return GetDatabase(databaseName) ?? CreateDatabase(databaseName);
		}

		/// <inheritdoc cref="IDatabaseFactory.GetAllDatabases"/>
		public IReadOnlyCollection<IDatabase> GetAllDatabases()
		{
			SyncDatabases();
			return _Databases.Values.ToArray();
		}

		private void SyncDatabases()
		{
			var queryResult = _DatabaseServerConnection.ExecuteQuery<ShowDatabaseResult>("SHOW DATABASES;", queryParameters: null);
			var databaseNames = new HashSet<string>(queryResult.Select(d => d.DatabaseName), StringComparer.OrdinalIgnoreCase);

			foreach (var databaseName in _Databases.Keys)
			{
				if (!databaseNames.Contains(databaseName))
				{
					_Databases.TryRemove(databaseName, out _);
				}
			}

			foreach (var databaseName in databaseNames)
			{
				if (!_Databases.ContainsKey(databaseName))
				{
					_Databases[databaseName] = new Database(_DatabaseServerConnection, _DatabaseNameValidator, _DatabaseTypeParser, databaseName);
				}
			}
		}
	}
}
