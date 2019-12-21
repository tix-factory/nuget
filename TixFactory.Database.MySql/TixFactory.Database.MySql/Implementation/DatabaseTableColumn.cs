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

		/// <inheritdoc cref="IDatabaseTableColumn.AutoIncrementing"/>
		public bool AutoIncrementing { get; }

		/// <summary>
		/// Initializes a new <see cref="DatabaseTableColumn"/>.
		/// </summary>
		/// <param name="databaseTypeParser">An <see cref="IDatabaseTypeParser"/>.</param>
		/// <param name="showColumnsResult">A <see cref="ShowColumnsResult"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseTypeParser"/>
		/// - <paramref name="showColumnsResult"/>
		/// </exception>
		public DatabaseTableColumn(IDatabaseTypeParser databaseTypeParser, ShowColumnsResult showColumnsResult)
		{
			if (databaseTypeParser == null)
			{
				throw new ArgumentNullException(nameof(databaseTypeParser));
			}

			if (showColumnsResult == null)
			{
				throw new ArgumentNullException(nameof(showColumnsResult));
			}

			Name = showColumnsResult.Name;
			Primary = showColumnsResult.Key == "PRI";
			Unique = Primary || showColumnsResult.Key == "UNI"; // The primary key has to be unique.

			var parseResult = databaseTypeParser.ParseDatabaseType(showColumnsResult.RawDataType, showColumnsResult.IsNullable == "YES");
			Type = parseResult.Type;
			MySqlType = parseResult.MySqlType;

			if (parseResult.Length.HasValue)
			{
				Length = parseResult.Length.Value;
			}

			AutoIncrementing = showColumnsResult.Extra == "auto_increment";
		}
	}
}
