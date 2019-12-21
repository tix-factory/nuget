using TixFactory.Configuration;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A column in an <see cref="IDatabaseTableIndex"/>.
	/// </summary>
	public interface IDatabaseTableIndexColumn
	{
		/// <summary>
		/// The <see cref="IDatabaseTableColumn"/>.
		/// </summary>
		IDatabaseTableColumn Column { get; }

		/// <summary>
		/// The <see cref="SortOrder"/> of the column.
		/// </summary>
		/// <seealso cref="ShowIndexResult.Collation"/>
		SortOrder? SortOrder { get; }
	}
}
