using System;
using System.ComponentModel;
using MySql.Data.MySqlClient;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Parses database types.
	/// </summary>
	/// <remarks>
	/// - https://dev.mysql.com/doc/refman/8.0/en/numeric-type-overview.html
	/// - https://dev.mysql.com/doc/refman/8.0/en/date-and-time-types.html
	/// - https://dev.mysql.com/doc/refman/8.0/en/string-types.html
	/// - https://dev.mysql.com/doc/refman/8.0/en/json.html
	/// </remarks>
	public interface IDatabaseTypeParser
	{
		/// <summary>
		/// Parses a database type into a <see cref="TypeDbTypeParseResult"/>.
		/// </summary>
		/// <example>
		/// - <paramref name="databaseType"/> = BIGINT(20)
		/// </example>
		/// <param name="databaseType">The database type.</param>
		/// <param name="nullable">Whether the parsed type is nullable.</param>
		/// <returns>The parsed database type.</returns>
		/// <exception cref="ArgumentException">
		/// - <paramref name="databaseType"/> is <c>null</c> or whitespace.
		/// - <paramref name="databaseType"/> could not be parsed.
		/// </exception>
		TypeDbTypeParseResult ParseDatabaseType(string databaseType, bool nullable);

		/// <summary>
		/// Gets a database type name from a <see cref="MySqlDbType"/>.
		/// </summary>
		/// <example>
		/// BIGINT -> <see cref="MySqlDbType.Int64"/>
		/// </example>
		/// <param name="mySqlType">The <see cref="MySqlDbType"/>.</param>
		/// <returns>The database type name.</returns>
		/// <exception cref="InvalidEnumArgumentException">
		/// - <paramref name="mySqlType"/>
		/// </exception>
		string GetDatabaseTypeName(MySqlDbType mySqlType);

		/// <summary>
		/// Gets a <see cref="MySqlDbType"/> from a <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="MySqlDbType"/>.</param>
		/// <returns>The <see cref="MySqlDbType"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="type"/>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// - <paramref name="type"/> does not map to <see cref="MySqlDbType"/>.
		/// </exception>
		MySqlDbType GetMySqlType(Type type);
	}
}
