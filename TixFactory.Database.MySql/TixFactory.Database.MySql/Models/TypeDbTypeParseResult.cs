using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Represents a database type and its mapping to a <see cref="Type"/>.
	/// </summary>
	public class TypeDbTypeParseResult
	{
		/// <summary>
		/// The database type name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The length.
		/// </summary>
		/// <example>
		/// BIGINT(20) -> 20
		/// </example>
		public int? Length { get; }

		/// <summary>
		/// The <see cref="Type"/>.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// The <see cref="MySqlDbType"/>.
		/// </summary>
		public MySqlDbType MySqlType { get; }

		/// <summary>
		/// Whether or not the DB type is nullable.
		/// </summary>
		public bool Nullable { get; }

		/// <summary>
		/// Whether or not the DB type is unsigned.
		/// </summary>
		public bool Unsigned { get; }

		/// <summary>
		/// Initializes a new <see cref="TypeDbTypeParseResult"/>.
		/// </summary>
		/// <param name="name">The database type name.</param>
		/// <param name="nullable">Whether or not the DB value is nullable.</param>
		/// <param name="length">The <see cref="Length"/>.</param>
		/// <param name="type">The <see cref="Type"/>.</param>
		/// <param name="mySqlType">The <see cref="MySqlDbType"/>.</param>
		/// <param name="unsigned">Whether or not the database type is unsigned.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="type"/>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// - <paramref name="name"/> is <c>null</c> or whitespace.
		/// </exception>
		/// <exception cref="InvalidEnumArgumentException">
		/// - <paramref name="mySqlType"/>
		/// </exception>
		public TypeDbTypeParseResult(string name, bool nullable, int? length, Type type, MySqlDbType mySqlType, bool unsigned)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			}

			if (!Enum.IsDefined(typeof(MySqlDbType), mySqlType))
			{
				throw new InvalidEnumArgumentException(nameof(mySqlType), (int)mySqlType, typeof(MySqlDbType));
			}

			Name = name;
			Nullable = nullable;
			Length = length;
			Type = type ?? throw new ArgumentNullException(nameof(type));
			MySqlType = mySqlType;
			Unsigned = unsigned;
		}
	}
}
