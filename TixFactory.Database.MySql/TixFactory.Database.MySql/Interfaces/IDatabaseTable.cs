using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A table in a MySQL database.
	/// </summary>
	public interface IDatabaseTable
	{
		/// <summary>
		/// The table name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets all the <see cref="IDatabaseTableColumn"/>s for the table.
		/// </summary>
		/// <returns>The collection of <see cref="IDatabaseTableColumn"/>.</returns>
		IReadOnlyCollection<IDatabaseTableColumn> GetAllColumns();
	}
}
