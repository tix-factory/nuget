using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TixFactory.Data.MySql
{
	/// <summary>
	/// Represents a connection to a MySql database.
	/// </summary>
	public interface IDatabaseConnection
	{
		T ExecuteInsertStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters);

		long ExecuteCountStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters);

		IReadOnlyCollection<T> ExecuteReadStoredProcedure<T>(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters)
			where T : class;

		int ExecuteWriteStoredProcedure(string storedProcedureName, IReadOnlyCollection<MySqlParameter> mySqlParameters);
	}
}
