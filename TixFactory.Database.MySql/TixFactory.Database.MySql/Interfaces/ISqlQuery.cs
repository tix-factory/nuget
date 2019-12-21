using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	public interface ISqlQuery
	{
		string Query { get; }

		IReadOnlyCollection<SqlQueryParameter> Parameters { get; }
	}
}
