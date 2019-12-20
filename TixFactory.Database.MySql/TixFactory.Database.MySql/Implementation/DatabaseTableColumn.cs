using System;
using MySql.Data.MySqlClient;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseTableColumn"/>
	internal class DatabaseTableColumn : IDatabaseTableColumn
	{
		/// <inheritdoc cref="IDatabaseTableColumn.Name"/>
		public string Name { get; }

		/// <inheritdoc cref="IDatabaseTableColumn.Length"/>
		public int Length { get; }

		/// <inheritdoc cref="IDatabaseTableColumn.Type"/>
		public Type Type { get; }

		/// <inheritdoc cref="IDatabaseTableColumn.MySqlType"/>
		public MySqlDbType MySqlType { get; }

		/// <inheritdoc cref="IDatabaseTableColumn.Unique"/>
		public bool Unique { get; }

		/// <inheritdoc cref="IDatabaseTableColumn.Primary"/>
		public bool Primary { get; }

		/// <summary>
		/// Initializes a new <see cref="DatabaseTableColumn"/>.
		/// </summary>
		/// <param name="showColumnsResult">A <see cref="ShowColumnsResult"/>.</param>
		/// <exception cref="ShowColumnsResult">
		/// - <paramref name="showColumnsResult"/>
		/// </exception>
		public DatabaseTableColumn(ShowColumnsResult showColumnsResult)
		{
			if (showColumnsResult == null)
			{
				throw new ArgumentNullException(nameof(showColumnsResult));
			}

			Name = showColumnsResult.Name;
			Primary = showColumnsResult.Key == "PRI";
			Unique = Primary || showColumnsResult.Key == "UNI"; // The primary key has to be unique.

			var parseResult = new TypeDbTypeParseResult(showColumnsResult.RawDataType, showColumnsResult.IsNullable == "YES");
			Type = parseResult.Type;
			MySqlType = parseResult.MySqlType;

			if (parseResult.Length.HasValue)
			{
				Length = parseResult.Length.Value;
			}
		}
	}
}
