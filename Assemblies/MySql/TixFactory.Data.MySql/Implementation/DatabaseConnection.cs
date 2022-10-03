using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TixFactory.Configuration;
using TixFactory.Logging;

namespace TixFactory.Data.MySql
{
    /// <inheritdoc cref="IDatabaseConnection"/>
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly IReadOnlySetting<string> _ConnectionString;
        private readonly JsonSerializerOptions _JsonSerializerOptions;
        private readonly SemaphoreSlim _ConnectionLock;

        /// <summary>
        /// Initializes a new <see cref="DatabaseConnection"/>.
        /// </summary>
        /// <param name="connectionString">The connection string <see cref="IReadOnlySetting{T}"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="connectionString"/>
        /// - <paramref name="logger"/>
        /// </exception>
        public DatabaseConnection(IReadOnlySetting<string> connectionString, ILogger logger)
            : this(connectionString, logger, maxConnections: 0)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="DatabaseConnection"/>.
        /// </summary>
        /// <param name="connectionString">The connection string <see cref="IReadOnlySetting{T}"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <param name="maxConnections">The max number of connections for per-read/write connection.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="connectionString"/>
        /// - <paramref name="logger"/>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// - <paramref name="maxConnections"/> is below 1.
        /// </exception>
        public DatabaseConnection(IReadOnlySetting<string> connectionString, ILogger logger, int maxConnections)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (maxConnections < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxConnections));
            }

            _ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            if (maxConnections > 0)
            {
                _ConnectionLock = new SemaphoreSlim(maxConnections, maxConnections);
            }
            else
            {
                _ConnectionLock = null;
            }

            var jsonSerializerOptions = _JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            jsonSerializerOptions.Converters.Add(new BooleanJsonConverter());
            jsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
        }

        /// <inheritdoc cref="IDatabaseConnection.ExecuteInsertStoredProcedure{T}"/>
        public T ExecuteInsertStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
        {
            var insertResults = ExecuteReadStoredProcedure<InsertResult<T>>(storedProcedureName, mySqlParameters);
            var insertResult = insertResults.First();
            return insertResult.Id;
        }

        /// <inheritdoc cref="IDatabaseConnection.ExecuteInsertStoredProcedureAsync{T}"/>
        public async Task<T> ExecuteInsertStoredProcedureAsync<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken)
        {
            var insertResults = await ExecuteReadStoredProcedureAsync<InsertResult<T>>(storedProcedureName, mySqlParameters, cancellationToken).ConfigureAwait(false);
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

        /// <inheritdoc cref="IDatabaseConnection.ExecuteCountStoredProcedureAsync"/>
        public async Task<long> ExecuteCountStoredProcedureAsync(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken)
        {
            var insertResults = await ExecuteReadStoredProcedureAsync<CountResult>(storedProcedureName, mySqlParameters, cancellationToken).ConfigureAwait(false);
            var insertResult = insertResults.First();
            return insertResult.Count;
        }

        /// <inheritdoc cref="IDatabaseConnection.ExecuteReadStoredProcedure{T}"/>
        public IReadOnlyCollection<T> ExecuteReadStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
            where T : class
        {
            IReadOnlyCollection<T> result;
            _ConnectionLock?.Wait();

            try
            {
                using (var connection = new MySqlConnection(_ConnectionString.Value))
                {
                    connection.Open();

                    var command = new MySqlCommand(storedProcedureName, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(mySqlParameters.ToArray());

                    result = ExecuteCommand<T>(command);
                }
            }
            finally
            {
                _ConnectionLock?.Release();
            }

            return result;
        }

        /// <inheritdoc cref="IDatabaseConnection.ExecuteReadStoredProcedureAsync{T}"/>
        public async Task<IReadOnlyCollection<T>> ExecuteReadStoredProcedureAsync<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken)
            where T : class
        {
            IReadOnlyCollection<T> result;

            if (_ConnectionLock != null)
            {
                await _ConnectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                using (var connection = new MySqlConnection(_ConnectionString.Value))
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    var command = new MySqlCommand(storedProcedureName, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(mySqlParameters.ToArray());

                    result = await ExecuteCommandAsync<T>(command, cancellationToken).ConfigureAwait(false);

                    await connection.CloseAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _ConnectionLock?.Release();
            }

            return result;
        }

        /// <inheritdoc cref="IDatabaseConnection.ExecuteWriteStoredProcedure"/>
        public int ExecuteWriteStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
        {
            int result;
            _ConnectionLock?.Wait();

            try
            {
                using (var connection = new MySqlConnection(_ConnectionString.Value))
                {
                    connection.Open();

                    var command = new MySqlCommand(storedProcedureName, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(mySqlParameters.ToArray());

                    result = command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ConnectionLock?.Release();
            }

            return result;
        }


        /// <inheritdoc cref="IDatabaseConnection.ExecuteWriteStoredProcedureAsync"/>
        public async Task<int> ExecuteWriteStoredProcedureAsync(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken)
        {
            int result;

            if (_ConnectionLock != null)
            {
                await _ConnectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                using (var connection = new MySqlConnection(_ConnectionString.Value))
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    var command = new MySqlCommand(storedProcedureName, connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(mySqlParameters.ToArray());

                    result = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                    await connection.CloseAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _ConnectionLock?.Release();
            }

            return result;
        }

        private IReadOnlyCollection<T> ExecuteCommand<T>(MySqlCommand command)
            where T : class
        {
            var rows = new List<T>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var parsedRow = ParseRow<T>(reader);
                    if (parsedRow != default(T))
                    {
                        rows.Add(parsedRow);
                    }
                }
            }

            return rows;
        }

        private async Task<IReadOnlyCollection<T>> ExecuteCommandAsync<T>(MySqlCommand command, CancellationToken cancellationToken)
            where T : class
        {
            var rows = new List<T>();

            using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var parsedRow = ParseRow<T>(reader);
                    if (parsedRow != default(T))
                    {
                        rows.Add(parsedRow);
                    }
                }
            }

            return rows;
        }

        private T ParseRow<T>(IDataRecord reader)
            where T : class
        {
            var row = new Dictionary<string, object>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.GetValue(i);
                if (value == DBNull.Value)
                {
                    value = null;
                }

                row.Add(reader.GetName(i), value);
            }

            // TODO: Is there a better way to convert reader object -> T?
            var serializedRow = JsonSerializer.Serialize(row, _JsonSerializerOptions);
            var deserializedRow = JsonSerializer.Deserialize<T>(serializedRow, _JsonSerializerOptions);

            return deserializedRow;
        }
    }
}
