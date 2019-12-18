namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Validates names of database things.
	/// </summary>
	/// <remarks>
	/// Name validations that belong here:
	/// - Database names
	/// - Table names
	/// - Stored procedure names
	/// - etc
	/// </remarks>
	public interface IDatabaseNameValidator
	{
		/// <summary>
		/// Checks if a database name matches MySQL database naming rules.
		/// </summary>
		/// <param name="databaseName">The database name to check.</param>
		/// <returns><c>true</c> if the name is valid for a MySQL database (otherwise <c>false</c>).</returns>
		bool IsDatabaseNameValid(string databaseName);

		/// <summary>
		/// Checks if a table name matches MySQL table naming rules.
		/// </summary>
		/// <param name="tableName">The table name to check.</param>
		/// <returns><c>true</c> if the name is valid for a MySQL database (otherwise <c>false</c>).</returns>
		bool IsTableNameValid(string tableName);

		/// <summary>
		/// Checks if a string is valid to be a variable name.
		/// </summary>
		/// <param name="variableName">The string to check.</param>
		/// <returns><c>true</c> if the name is valid for a MySQL variable (otherwise <c>false</c>).</returns>
		bool IsVariableNameValid(string variableName);
	}
}
