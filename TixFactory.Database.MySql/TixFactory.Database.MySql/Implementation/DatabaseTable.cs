using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseTable"/>
	internal class DatabaseTable : IDatabaseTable
	{
		private readonly IDatabaseServerConnection _DatabaseServerConnection;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;
		private readonly IDatabase _Database;
		private readonly ConcurrentDictionary<string, IDatabaseTableColumn> _DatabaseTableColumns;
		private ISet<string> _OrderedDatabaseColumnNames;

		/// <inheritdoc cref="IDatabaseTable.Name"/>
		public string Name { get; }

		/// <summary>
		/// Initializes a new <see cref="DatabaseTable"/>.
		/// </summary>
		/// <param name="databaseServerConnection">An <see cref="IDatabaseServerConnection"/>.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <param name="database">The <see cref="IDatabase"/> the table belongs to.</param>
		/// <param name="tableName">The table name.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseServerConnection"/>
		/// - <paramref name="databaseNameValidator"/>
		/// - <paramref name="database"/>
		/// </exception>
		public DatabaseTable(IDatabaseServerConnection databaseServerConnection, IDatabaseNameValidator databaseNameValidator, IDatabase database, string tableName)
		{
			_DatabaseServerConnection = databaseServerConnection ?? throw new ArgumentNullException(nameof(databaseServerConnection));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));
			_Database = database ?? throw new ArgumentNullException(nameof(database));
			Name = tableName;

			_OrderedDatabaseColumnNames = new HashSet<string>();
			_DatabaseTableColumns = new ConcurrentDictionary<string, IDatabaseTableColumn>(StringComparer.OrdinalIgnoreCase);
		}

		/// <inheritdoc cref="IDatabaseTable.GetAllColumns"/>
		public IReadOnlyCollection<IDatabaseTableColumn> GetAllColumns()
		{
			SyncColumns();
			return _OrderedDatabaseColumnNames.Select(c => _DatabaseTableColumns[c]).ToArray();
		}
		
		private void SyncColumns()
		{
			var queryResult = _DatabaseServerConnection.ExecuteQuery<ShowColumnsResult>($"SHOW COLUMNS FROM `{_Database.Name}`.`{Name}`;", queryParameters: null);
			var columnNames = new HashSet<string>(queryResult.Select(c => c.Name), StringComparer.OrdinalIgnoreCase);

			foreach (var columnName in _DatabaseTableColumns.Keys)
			{
				if (!columnNames.Contains(columnName))
				{
					_DatabaseTableColumns.TryRemove(columnName, out _);
				}
			}

			foreach (var columnName in columnNames)
			{
				if (!_DatabaseTableColumns.ContainsKey(columnName))
				{
					_DatabaseTableColumns[columnName] = new DatabaseTableColumn(queryResult.First(c => c.Name == columnName));
				}
			}

			_OrderedDatabaseColumnNames = columnNames;
		}
	}
}
