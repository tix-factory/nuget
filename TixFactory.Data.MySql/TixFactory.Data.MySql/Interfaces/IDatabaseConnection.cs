using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TixFactory.Data.MySql
{
	/// <summary>
	/// Represents a connection to a MySql database.
	/// </summary>
	public interface IDatabaseConnection
	{
		T ExecuteInsertStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters);

		Task<T> ExecuteInsertStoredProcedureAsync<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken);

		long ExecuteCountStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters);

		Task<long> ExecuteCountStoredProcedureAsync(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken);

		IReadOnlyCollection<T> ExecuteReadStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class;

		Task<IReadOnlyCollection<T>> ExecuteReadStoredProcedureAsync<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken)
			where T : class;

		int ExecuteWriteStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters);

		Task<int> ExecuteWriteStoredProcedureAsync(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters, CancellationToken cancellationToken);
	}
}
