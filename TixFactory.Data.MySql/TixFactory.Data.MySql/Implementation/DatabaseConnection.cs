using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using MySql.Data.MySqlClient;
using TixFactory.Configuration;

namespace TixFactory.Data.MySql
{
	/// <inheritdoc cref="IDatabaseConnection"/>
	public class DatabaseConnection : IDatabaseConnection
	{
		private readonly JsonSerializerOptions _JsonSerializerOptions;
		private readonly ISet<DatabaseConnectionWrapper> _ReadConnections;
		private readonly ISet<DatabaseConnectionWrapper> _WriteConnections;
		private int _ReadCount = 0;
		private int _WriteCount = 0;

		/// <summary>
		/// Initializes a new <see cref="DatabaseConnection"/>.
		/// </summary>
		/// <param name="connectionString">The connection string <see cref="IReadOnlySetting{T}"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="connectionString"/>
		/// </exception>
		public DatabaseConnection(IReadOnlySetting<string> connectionString)
			: this(connectionString, maxConnections: 5)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="DatabaseConnection"/>.
		/// </summary>
		/// <param name="connectionString">The connection string <see cref="IReadOnlySetting{T}"/>.</param>
		/// <param name="maxConnections">The max number of connections for per-read/write connection.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="connectionString"/>
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// - <paramref name="maxConnections"/> is below 1.
		/// </exception>
		public DatabaseConnection(IReadOnlySetting<string> connectionString, int maxConnections)
		{
			if (connectionString == null)
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			if (maxConnections < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(maxConnections));
			}

			var readConnections = _ReadConnections = new HashSet<DatabaseConnectionWrapper>();
			var writeConnections = _WriteConnections = new HashSet<DatabaseConnectionWrapper>();

			var jsonSerializerOptions = _JsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			jsonSerializerOptions.Converters.Add(new BooleanJsonConverter());
			jsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());

			for (var n = 0; n < maxConnections; n++)
			{
				readConnections.Add(new DatabaseConnectionWrapper(connectionString));
				writeConnections.Add(new DatabaseConnectionWrapper(connectionString));
			}
		}

		/// <inheritdoc cref="IDatabaseConnection.ExecuteInsertStoredProcedure{T}"/>
		public T ExecuteInsertStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
		{
			var insertResults = ExecuteReadStoredProcedure<InsertResult<T>>(storedProcedureName, mySqlParameters);
			var insertResult = insertResults.First();
			return insertResult.Id;
		}

		/// <inheritdoc cref="IDatabaseConnection.ExecuteCountStoredProcedure"/>
		public long ExecuteCountStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
		{
			var insertResults = ExecuteReadStoredProcedure<CountResult>(storedProcedureName, mySqlParameters);
			var insertResult = insertResults.First();
			return insertResult.Count;
		}

		/// <inheritdoc cref="IDatabaseConnection.ExecuteReadStoredProcedure{T}"/>
		public IReadOnlyCollection<T> ExecuteReadStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class
		{
			var index = Interlocked.Increment(ref _ReadCount);
			var dataConnectionWrapper = _ReadConnections.ElementAt(index % _ReadConnections.Count);
			dataConnectionWrapper.ConnectionLock.Wait();

			try
			{
				var command = new MySqlCommand(storedProcedureName, dataConnectionWrapper.Connection);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddRange(mySqlParameters.ToArray());
				return ExecuteCommand<T>(command);
			}
			finally
			{
				dataConnectionWrapper.ConnectionLock.Release();
			}
		}

		/// <inheritdoc cref="IDatabaseConnection.ExecuteWriteStoredProcedure"/>
		public int ExecuteWriteStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
		{
			var index = Interlocked.Increment(ref _WriteCount);
			var dataConnectionWrapper = _WriteConnections.ElementAt(index % _WriteConnections.Count);
			dataConnectionWrapper.ConnectionLock.Wait();

			try
			{
				var command = new MySqlCommand(storedProcedureName, dataConnectionWrapper.Connection);
				command.CommandType = CommandType.StoredProcedure;
				command.Parameters.AddRange(mySqlParameters.ToArray());

				return command.ExecuteNonQuery();
			}
			finally
			{
				dataConnectionWrapper.ConnectionLock.Release();
			}
		}

		private IReadOnlyCollection<T> ExecuteCommand<T>(MySqlCommand command)
			where T : class
		{
			var rows = new List<T>();

			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					var row = new Dictionary<string, object>();
					for (var i = 0; i < reader.FieldCount; i++)
					{
						row.Add(reader.GetName(i), reader.GetValue(i));
					}

					// TODO: Is there a better way to convert reader object -> T?
					var serializedRow = JsonSerializer.Serialize(row);
					var deserializedRow = JsonSerializer.Deserialize<T>(serializedRow, _JsonSerializerOptions);

					if (deserializedRow != default(T))
					{
						rows.Add(deserializedRow);
					}
				}
			}

			return rows;
		}
	}
}
