using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabase"/>
	internal class Database : IDatabase
	{
		private readonly IDatabaseServerConnection _DatabaseServerConnection;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;
		private readonly ConcurrentDictionary<string, IDatabaseTable> _DatabaseTables;

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

			_DatabaseTables = new ConcurrentDictionary<string, IDatabaseTable>(StringComparer.OrdinalIgnoreCase);
		}

		/// <inheritdoc cref="IDatabase.GetTable"/>
		public IDatabaseTable GetTable(string tableName)
		{
			if (!_DatabaseNameValidator.IsTableNameValid(tableName))
			{
				return null;
			}

			SyncTables();
			_DatabaseTables.TryGetValue(tableName, out var databaseTable);

			return databaseTable;
		}

		/// <inheritdoc cref="IDatabase.GetAllTables"/>
		public IReadOnlyCollection<IDatabaseTable> GetAllTables()
		{
			SyncTables();
			return _DatabaseTables.Values.ToArray();
		}

		/// <inheritdoc cref="IDatabase.GetStoredProcedureNames"/>
		public IReadOnlyCollection<string> GetStoredProcedureNames()
		{
			var queryResult = _DatabaseServerConnection.ExecuteQuery<ShowProcedureStatusResult>("SHOW PROCEDURE STATUS", queryParameters: null);
			var storedProcedureNames = queryResult.Where(p => p.DatabaseName.Equals(Name, StringComparison.OrdinalIgnoreCase) && p.Type == "PROCEDURE").Select(p => p.Name);
			return new HashSet<string>(storedProcedureNames);
		}

		private void SyncTables()
		{
			var queryResult = _DatabaseServerConnection.ExecuteQuery<IDictionary<string, string>>($"SHOW TABLES FROM `{Name}`;", queryParameters: null);

			// The query returns exactly one with the name "Tables_in_{databaseName}" :(
			// Would have been better with a constant column name, not a crafted one so I could properly deserialize it into a model.. like SHOW DATABASES.
			var tableNames = new HashSet<string>(queryResult.Select(t => t.Values.First()), StringComparer.OrdinalIgnoreCase);

			foreach (var tableName in _DatabaseTables.Keys)
			{
				if (!tableNames.Contains(tableName))
				{
					_DatabaseTables.TryRemove(tableName, out _);
				}
			}

			foreach (var tableName in tableNames)
			{
				if (!_DatabaseTables.ContainsKey(tableName))
				{
					_DatabaseTables[tableName] = new DatabaseTable(_DatabaseServerConnection, _DatabaseNameValidator, this, tableName);
				}
			}
		}
	}
}
