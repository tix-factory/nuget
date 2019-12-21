using System;
using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQuery"/>
	internal class SqlQuery : ISqlQuery
	{
		/// <inheritdoc cref="ISqlQuery.Query"/>
		public string Query { get; }

		/// <inheritdoc cref="ISqlQuery.Parameters"/>
		public IReadOnlyCollection<SqlQueryParameter> Parameters { get; }

		public SqlQuery(string query, IReadOnlyCollection<SqlQueryParameter> parameters)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));
			}

			Query = query;
			Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
		}
	}
}
