using System.Collections.Generic;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Represents a MySql database table index.
	/// </summary>
	public interface IDatabaseTableIndex
	{
		/// <summary>
		/// The name of the index.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Whether or not the index is unique.
		/// </summary>
		bool Unique { get; }

		/// <summary>
		/// The columns in order involved with the index.
		/// </summary>
		IReadOnlyCollection<IDatabaseTableIndexColumn> Columns { get; }
	}
}
