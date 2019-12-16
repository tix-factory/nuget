using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TixFactory.Configuration;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseServerConnection"/>
	public class DatabaseServerConnection : IDatabaseServerConnection
	{
		private readonly ILazyWithRetry<MySqlConnection> _MySqlConnectionLazy;
		private readonly IDatabaseNameValidator _DatabaseNameValidator;
		private readonly SemaphoreSlim _ConnectionLock = new SemaphoreSlim(1, 1);

		/// <summary>
		/// Initializes a new <see cref="DatabaseServerConnection"/>.
		/// </summary>
		/// <param name="connectionString">An <see cref="IReadOnlySetting{T}"/> evaluating to the database server connection string.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="connectionString"/>
		/// - <paramref name="databaseNameValidator"/>
		/// </exception>
		public DatabaseServerConnection(IReadOnlySetting<string> connectionString, IDatabaseNameValidator databaseNameValidator)
		{
			if (connectionString == null)
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));
			_MySqlConnectionLazy = new LazyWithRetry<MySqlConnection>(() => new MySqlConnection(connectionString.Value));

			connectionString.Changed += UpdateConnectionString;
		}

		/// <summary>
		/// Initializes a new <see cref="DatabaseServerConnection"/>.
		/// </summary>
		/// <param name="mySqlConnectionLazy">An <see cref="ILazyWithRetry{T}"/> evaluating to the <see cref="MySqlConnection"/>.</param>
		/// <param name="databaseNameValidator">An <see cref="IDatabaseNameValidator"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="mySqlConnectionLazy"/>
		/// - <paramref name="databaseNameValidator"/>
		/// </exception>
		public DatabaseServerConnection(ILazyWithRetry<MySqlConnection> mySqlConnectionLazy, IDatabaseNameValidator databaseNameValidator)
		{
			_MySqlConnectionLazy = mySqlConnectionLazy ?? throw new ArgumentNullException(nameof(mySqlConnectionLazy));
			_DatabaseNameValidator = databaseNameValidator ?? throw new ArgumentNullException(nameof(databaseNameValidator));
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteQuery{T}"/>
		public IReadOnlyCollection<T> ExecuteQuery<T>(string query, IDictionary<string, object> queryParameters)
			where T : class
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));
			}

			var connection = GetMySqlConnection();
			var command = new MySqlCommand(query, connection);

			if (queryParameters != null)
			{
				foreach (var queryParameter in queryParameters)
				{
					// User variables are written as @var_name
					// https://dev.mysql.com/doc/refman/8.0/en/user-variables.html
					if (_DatabaseNameValidator.IsVariableNameValid(queryParameter.Key))
					{
						command.Parameters.AddWithValue($"@{queryParameter.Key}", queryParameter.Value);
					}
					else
					{
						throw new ArgumentException($"'{nameof(queryParameters)}' contains invalid variable name: '{queryParameter.Key}'", nameof(queryParameters));
					}
				}
			}
			
			var reader = command.ExecuteReader();
			var rows = new List<T>();

			while (reader.Read())
			{
				var row = new Dictionary<string, object>();
				for (var i = 0; i < reader.FieldCount; i++)
				{
					row.Add(reader.GetName(i), reader.GetValue(i));
				}

				// TODO: Is there a better way to convert reader object -> T?
				var serializedRow = JsonConvert.SerializeObject(row);
				var deserializedRow = JsonConvert.DeserializeObject<T>(serializedRow);

				if (deserializedRow != default(T))
				{
					rows.Add(deserializedRow);
				}
			}

			return rows;
		}

		private void UpdateConnectionString(string newConnectionString, string oldConnectionString)
		{
			if (_MySqlConnectionLazy.IsValueCreated)
			{
				_MySqlConnectionLazy.Value.ConnectionString = newConnectionString;
			}
		}

		private MySqlConnection GetMySqlConnection()
		{
			var mySqlConnection = _MySqlConnectionLazy.Value;

			if (mySqlConnection.State == ConnectionState.Open)
			{
				return mySqlConnection;
			}

			_ConnectionLock.Wait();

			try
			{
				if (mySqlConnection.State == ConnectionState.Open)
				{
					return mySqlConnection;
				}

				mySqlConnection.Open();
			}
			finally
			{
				_ConnectionLock.Release();
			}

			return mySqlConnection;
		}
	}
}
