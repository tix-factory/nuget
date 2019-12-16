using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	public interface IDatabaseServerConnection
	{
		IReadOnlyCollection<T> ExecuteQuery<T>(string query, IDictionary<string, object> queryParameters)
			where T : class;
	}
}
