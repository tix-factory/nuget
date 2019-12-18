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
	}
}
