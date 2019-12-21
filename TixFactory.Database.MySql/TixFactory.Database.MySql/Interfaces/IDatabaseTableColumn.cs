using MySql.Data.MySqlClient;
using System;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Represents a MySql database table column.
	/// </summary>
	public interface IDatabaseTableColumn
	{
		/// <summary>
		/// The column name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The data size.
		/// </summary>
		/// <example>
		/// varchar(60) -> 60
		/// </example>
		int Length { get; }

		/// <summary>
		/// The application <see cref="Type"/>.
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// The <see cref="MySqlDbType"/>.
		/// </summary>
		MySqlDbType MySqlType { get; }

		/// <summary>
		/// Whether or not the column is unique.
		/// </summary>
		bool Unique { get; }

		/// <summary>
		/// Whether or not this is the primary column.
		/// </summary>
		bool Primary { get; }

		/// <summary>
		/// Whether or not the column is auto-incrementing.
		/// </summary>
		bool AutoIncrementing { get; }
	}
}
