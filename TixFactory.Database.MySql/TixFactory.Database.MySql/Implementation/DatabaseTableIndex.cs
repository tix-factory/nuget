using System;
using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseTableIndex"/>
	internal class DatabaseTableIndex : IDatabaseTableIndex
	{
		/// <inheritdoc cref="IDatabaseTableIndex.Name"/>
		public string Name { get; }

		/// <inheritdoc cref="IDatabaseTableIndex.Unique"/>
		public bool Unique { get; }

		/// <inheritdoc cref="IDatabaseTableIndex.Columns"/>
		public IReadOnlyCollection<IDatabaseTableIndexColumn> Columns { get; }

		public DatabaseTableIndex(string name, bool unique, IReadOnlyCollection<IDatabaseTableIndexColumn> columns)
		{
			Name = name;
			Unique = unique;
			Columns = columns ?? throw new ArgumentNullException(nameof(columns));
		}
	}
}
