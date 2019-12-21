using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
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

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteQuery(string, IDictionary{string,object})"/>
		public int ExecuteQuery(string query, IDictionary<string, object> queryParameters)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));
			}

			var mySqlParameters = GetMySqlParameters(queryParameters);
			return ExecuteQuery(query, mySqlParameters);
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteQuery{T}(string, IReadOnlyCollection{MySqlParameter})"/>
		public int ExecuteQuery(string query, IReadOnlyCollection<MySqlParameter> mySqlParameters)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));
			}

			var command = GetMySqlCommand(query, mySqlParameters);
			return command.ExecuteNonQuery();
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteQuery{T}(string, IDictionary{string,object})"/>
		public IReadOnlyCollection<T> ExecuteQuery<T>(string query, IDictionary<string, object> queryParameters)
			where T : class
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));
			}

			var mySqlParameters = GetMySqlParameters(queryParameters);
			return ExecuteQuery<T>(query, mySqlParameters);
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteQuery{T}(string, IReadOnlyCollection{MySqlParameter})"/>
		public IReadOnlyCollection<T> ExecuteQuery<T>(string query, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));
			}

			var command = GetMySqlCommand(query, mySqlParameters);
			return ExecuteCommand<T>(command);
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteStoredProcedure(string, IDictionary{string,object})"/>
		public int ExecuteStoredProcedure(string storedProcedureName, IDictionary<string, object> queryParameters)
		{
			if (string.IsNullOrWhiteSpace(storedProcedureName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(storedProcedureName));
			}

			var mySqlParameters = GetMySqlParameters(queryParameters);
			return ExecuteStoredProcedure(storedProcedureName, mySqlParameters);
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteStoredProcedure(string, IReadOnlyCollection{MySqlParameter})"/>
		public int ExecuteStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
		{
			if (string.IsNullOrWhiteSpace(storedProcedureName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(storedProcedureName));
			}

			var connection = GetMySqlConnection();
			if (string.IsNullOrWhiteSpace(connection.Database))
			{
				throw new ApplicationException("Database must selected in the connection string with the proper casing in order to execute stored procedures.");
			}

			try
			{
				var command = GetMySqlCommandForStoredProcedure(storedProcedureName, mySqlParameters, connection);
				return command.ExecuteNonQuery();
			}
			catch (MySqlException e) when (e.Code == (int)MySqlErrorCode.NoDatabaseSelected)
			{
				// BUG: MySqlException code can actually come back as zero so these catch scopes do nothing right now.
				throw new ApplicationException("Database must selected in the connection string with the proper casing in order to execute stored procedures.", e);
			}
			catch (MySqlException e) when (e.Code == (int)MySqlErrorCode.StoredProcedureDoesNotExist || (e.Message.Contains("Procedure or function") && e.Message.Contains("cannot be found")))
			{
				throw new ArgumentException("Could not find stored procedure in database. If you're sure it exists check that your connection string contains the database name with the proper casing.", nameof(storedProcedureName), e);
			}
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteStoredProcedure{T}(string, IDictionary{string,object})"/>
		public IReadOnlyCollection<T> ExecuteStoredProcedure<T>(string storedProcedureName, IDictionary<string, object> queryParameters) 
			where T : class
		{
			if (string.IsNullOrWhiteSpace(storedProcedureName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(storedProcedureName));
			}

			var mySqlParameters = GetMySqlParameters(queryParameters);
			return ExecuteStoredProcedure<T>(storedProcedureName, mySqlParameters);
		}

		/// <inheritdoc cref="IDatabaseServerConnection.ExecuteStoredProcedure{T}(string, IReadOnlyCollection{MySqlParameter})"/>
		public IReadOnlyCollection<T> ExecuteStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters) 
			where T : class
		{
			if (string.IsNullOrWhiteSpace(storedProcedureName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(storedProcedureName));
			}

			var connection = GetMySqlConnection();
			if (string.IsNullOrWhiteSpace(connection.Database))
			{
				throw new ApplicationException("Database must selected in the connection string with the proper casing in order to execute stored procedures.");
			}

			try
			{
				var command = GetMySqlCommandForStoredProcedure(storedProcedureName, mySqlParameters, connection);
				return ExecuteCommand<T>(command);
			}
			catch (MySqlException e) when (e.Code == (int)MySqlErrorCode.NoDatabaseSelected)
			{
				// BUG: MySqlException code can actually come back as zero so these catch scopes do nothing right now.
				throw new ApplicationException("Database must selected in the connection string with the proper casing in order to execute stored procedures.", e);
			}
			catch (MySqlException e) when (e.Code == (int)MySqlErrorCode.StoredProcedureDoesNotExist || (e.Message.Contains("Procedure or function") && e.Message.Contains("cannot be found")))
			{
				throw new ArgumentException("Could not find stored procedure in database. If you're sure it exists check that your connection string contains the database name with the proper casing.", nameof(storedProcedureName), e);
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
					var serializedRow = JsonConvert.SerializeObject(row);
					var deserializedRow = JsonConvert.DeserializeObject<T>(serializedRow);

					if (deserializedRow != default(T))
					{
						rows.Add(deserializedRow);
					}
				}
			}

			return rows;
		}

		private IReadOnlyCollection<MySqlParameter> GetMySqlParameters(IDictionary<string, object> queryParameters)
		{
			var mySqlParameters = new List<MySqlParameter>();
			if (queryParameters == null)
			{
				return mySqlParameters;
			}

			foreach (var queryParameter in queryParameters)
			{
				// User variables are written as @var_name
				// https://dev.mysql.com/doc/refman/8.0/en/user-variables.html
				if (_DatabaseNameValidator.IsVariableNameValid(queryParameter.Key))
				{
					mySqlParameters.Add(new MySqlParameter($"@{queryParameter.Key}", queryParameter.Value));
				}
				else
				{
					throw new ArgumentException($"'{nameof(queryParameters)}' contains invalid variable name: '{queryParameter.Key}'", nameof(queryParameters));
				}
			}

			return mySqlParameters;
		}

		private void UpdateConnectionString(string newConnectionString, string oldConnectionString)
		{
			if (_MySqlConnectionLazy.IsValueCreated)
			{
				// TODO: Do we need to reset the connection or anything after the connection string changes?
				_MySqlConnectionLazy.Value.ConnectionString = newConnectionString;
			}
		}

		private MySqlCommand GetMySqlCommand(string query, IReadOnlyCollection<MySqlParameter> mySqlParameters)
		{
			var connection = GetMySqlConnection();
			var command = new MySqlCommand(query, connection);

			if (mySqlParameters != null)
			{
				command.Parameters.AddRange(mySqlParameters.ToArray());
			}

			return command;
		}

		private MySqlCommand GetMySqlCommandForStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, MySqlConnection connection)
		{
			var command = new MySqlCommand(storedProcedureName, connection);

			command.CommandType = CommandType.StoredProcedure;

			if (mySqlParameters != null)
			{
				command.Parameters.AddRange(mySqlParameters.ToArray());
			}

			return command;
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
