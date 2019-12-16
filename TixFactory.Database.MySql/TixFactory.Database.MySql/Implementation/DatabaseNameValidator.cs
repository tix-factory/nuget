using System.Text.RegularExpressions;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseNameValidator"/>
	public class DatabaseNameValidator : IDatabaseNameValidator
	{
		/// <summary>
		/// The max length for a database name.
		/// </summary>
		/// <remarks>
		/// https://dev.mysql.com/doc/refman/8.0/en/identifier-length.html
		/// </remarks>
		private const int _MaxDatabaseNameLength = 64;

		/// <summary>
		/// The max length for a MySQL variable name.
		/// </summary>
		/// <remarks>
		/// https://dev.mysql.com/doc/refman/8.0/en/user-variables.html
		/// </remarks>
		private const int _MaxVariableNameLength = 64;

		/// <summary>
		/// A regex for validating all the characters in the database name are valid.
		/// </summary>
		/// <remarks>
		/// https://dev.mysql.com/doc/refman/8.0/en/identifiers.html
		/// </remarks>
		private readonly Regex _DatabaseNameValidationRegex = new Regex(@"^[0-9,a-z,A-Z$_]+$");

		/// <summary>
		/// A regex for validating all characters in the variable name are valid.
		/// </summary>
		/// <remarks>
		/// https://dev.mysql.com/doc/refman/8.0/en/user-variables.html
		/// </remarks>
		private readonly Regex _VariableNameValidationRegex = new Regex(@"^[0-9,a-z,A-Z$_\.]+$");

		/// <inheritdoc cref="IDatabaseNameValidator.IsDatabaseNameValid"/>
		public bool IsDatabaseNameValid(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName) || databaseName.Length > _MaxDatabaseNameLength)
			{
				return false;
			}

			return _DatabaseNameValidationRegex.IsMatch(databaseName);
		}

		/// <inheritdoc cref="IDatabaseNameValidator.IsVariableNameValid"/>
		public bool IsVariableNameValid(string variableName)
		{
			if (string.IsNullOrWhiteSpace(variableName) || variableName.Length > _MaxVariableNameLength)
			{
				return false;
			}

			return _VariableNameValidationRegex.IsMatch(variableName);
		}
	}
}
