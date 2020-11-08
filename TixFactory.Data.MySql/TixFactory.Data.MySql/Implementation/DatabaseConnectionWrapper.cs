using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TixFactory.Configuration;
using TixFactory.Logging;

namespace TixFactory.Data.MySql
{
	internal class DatabaseConnectionWrapper
	{
		private readonly IReadOnlySetting<string> _ConnectionString;
		private readonly ILogger _Logger;
		private readonly SemaphoreSlim _GetConnectionLock;
		private MySqlConnection _MySqlConnection;

		public SemaphoreSlim ConnectionLock { get; }

		public DatabaseConnectionWrapper(IReadOnlySetting<string> connectionString, ILogger logger)
		{
			_ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_GetConnectionLock = new SemaphoreSlim(1, 1);
			ConnectionLock = new SemaphoreSlim(1, 1);
		}

		public MySqlConnection GetConnection()
		{
			_GetConnectionLock.Wait();

			try
			{
				if (_MySqlConnection != null)
				{
					return _MySqlConnection;
				}

				var connectionString = _ConnectionString.Value;
				var connection = new MySqlConnection(connectionString);
				connection.StateChange += ConnectionStateChange;
				connection.Open();

				return connection;
			}
			finally
			{
				_GetConnectionLock.Release();
			}
		}

		public async Task<MySqlConnection> GetConnectionAsync(CancellationToken cancellationToken)
		{
			await _GetConnectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				if (_MySqlConnection != null)
				{
					return _MySqlConnection;
				}

				var connectionString = _ConnectionString.Value;
				var connection = new MySqlConnection(connectionString);
				connection.StateChange += ConnectionStateChange;

				await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

				return connection;
			}
			finally
			{
				_GetConnectionLock.Release();
			}
		}

		public void Close()
		{
			_GetConnectionLock.Wait();

			try
			{
				var connection = _MySqlConnection;
				if (connection != null)
				{
					try
					{
						connection.Close();
					}
					catch (Exception e)
					{
						_Logger.Warn($"Unexpected exception while closing connection\n{e}");
					}

					try
					{
						connection.Dispose();
					}
					catch
					{
						// who cares?
					}
				}

				_MySqlConnection = null;
			}
			finally
			{
				_GetConnectionLock.Release();
			}
		}

		public async Task CloseAsync(CancellationToken cancellationToken)
		{
			await _GetConnectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				var connection = _MySqlConnection;
				if (connection != null)
				{
					try
					{
						await connection.CloseAsync(cancellationToken).ConfigureAwait(false);
					}
					catch (Exception e)
					{
						_Logger.Warn($"Unexpected exception while closing connection\n{e}");
					}

					try
					{
						await connection.DisposeAsync().ConfigureAwait(false);
					}
					catch
					{
						// who cares?
					}
				}

				_MySqlConnection = null;
			}
			finally
			{
				_GetConnectionLock.Release();
			}
		}

		private void ConnectionStateChange(object sender, StateChangeEventArgs e)
		{
			switch (e.CurrentState)
			{
				case ConnectionState.Broken:
				case ConnectionState.Closed:
					Close();
					return;
			}
		}
	}
}
