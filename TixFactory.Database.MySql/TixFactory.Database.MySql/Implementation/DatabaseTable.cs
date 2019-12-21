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
		private readonly IDatabaseTypeParser _DatabaseTypeParser;
		private readonly IDatabase _Database;
		private readonly ConcurrentDictionary<string, IDatabaseTableColumn> _DatabaseTableColumns;
		private readonly ConcurrentDictionary<string, IDatabaseTableIndex> _DatabaseTableIndexes;
		private ISet<string> _OrderedDatabaseColumnNames;

		/// <inheritdoc cref="IDatabaseTable.Name"/>
		public string Name { get; }

		/// <summary>
		/// Initializes a new <see cref="DatabaseTable"/>.
		/// </summary>
		/// <param name="databaseServerConnection">An <see cref="IDatabaseServerConnection"/>.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <param name="databaseTypeParser">An <see cref="IDatabaseTypeParser"/>.</param>
		/// <param name="database">The <see cref="IDatabase"/> the table belongs to.</param>
		/// <param name="tableName">The table name.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseServerConnection"/>
		/// - <paramref name="databaseNameValidator"/>
		/// - <paramref name="databaseTypeParser"/>
		/// - <paramref name="database"/>
		/// </exception>
		public DatabaseTable(IDatabaseServerConnection databaseServerConnection, IDatabaseNameValidator databaseNameValidator, IDatabaseTypeParser databaseTypeParser, IDatabase database, string tableName)
		{
			_DatabaseServerConnection = databaseServerConnection ?? throw new ArgumentNullException(nameof(databaseServerConnection));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));
			_DatabaseTypeParser = databaseTypeParser ?? throw new ArgumentNullException(nameof(databaseTypeParser));
			_Database = database ?? throw new ArgumentNullException(nameof(database));
			Name = tableName;

			_OrderedDatabaseColumnNames = new HashSet<string>();
			_DatabaseTableColumns = new ConcurrentDictionary<string, IDatabaseTableColumn>(StringComparer.OrdinalIgnoreCase);
			_DatabaseTableIndexes = new ConcurrentDictionary<string, IDatabaseTableIndex>(StringComparer.OrdinalIgnoreCase);
		}

		/// <inheritdoc cref="IDatabaseTable.GetColumn"/>
		public IDatabaseTableColumn GetColumn(string columnName)
		{
			if (!_DatabaseNameValidator.IsColumnNameValid(columnName))
			{
				return null;
			}

			SyncColumns();
			_DatabaseTableColumns.TryGetValue(columnName, out var column);

			return column;
		}

		/// <inheritdoc cref="IDatabaseTable.GetAllColumns"/>
		public IReadOnlyCollection<IDatabaseTableColumn> GetAllColumns()
		{
			SyncColumns();
			return _OrderedDatabaseColumnNames.Select(c => _DatabaseTableColumns[c]).ToArray();
		}

		/// <inheritdoc cref="IDatabaseTable.GetIndex"/>
		public IDatabaseTableIndex GetIndex(string indexName)
		{
			if (!_DatabaseNameValidator.IsIndexNameValid(indexName))
			{
				return null;
			}

			SyncIndexes();
			_DatabaseTableIndexes.TryGetValue(indexName, out var index);

			return index;
		}

		/// <inheritdoc cref="IDatabaseTable.GetAllIndexes"/>
		public IReadOnlyCollection<IDatabaseTableIndex> GetAllIndexes()
		{
			SyncIndexes();
			return _DatabaseTableIndexes.Values.ToArray();
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
					_DatabaseTableColumns[columnName] = new DatabaseTableColumn(_DatabaseTypeParser, queryResult.First(c => c.Name == columnName));
				}
			}

			_OrderedDatabaseColumnNames = columnNames;
		}

		private void SyncIndexes()
		{
			SyncColumns();

			var queryResult = _DatabaseServerConnection.ExecuteQuery<ShowIndexResult>($"SHOW INDEX FROM `{_Database.Name}`.`{Name}`;", queryParameters: null);
			var indexNames = new HashSet<string>(queryResult.Select(c => c.IndexName), StringComparer.OrdinalIgnoreCase);

			foreach (var indexName in _DatabaseTableIndexes.Keys)
			{
				if (!indexNames.Contains(indexName))
				{
					_DatabaseTableIndexes.TryRemove(indexName, out _);
				}
			}

			foreach (var indexName in indexNames)
			{
				var showIndexResults = queryResult.Where(i => i.IndexName == indexName).ToList();
				showIndexResults.Sort((a, b) => a.Order - b.Order);

				var decidingColumn = showIndexResults.First();
				var indexColumns = showIndexResults.Select(c => new DatabaseTableIndexColumn(_DatabaseTableColumns[c.ColumnName], c)).ToArray();

				_DatabaseTableIndexes[indexName] = new DatabaseTableIndex(indexName, !decidingColumn.NonUnique, indexColumns);
			}
		}
	}
}
