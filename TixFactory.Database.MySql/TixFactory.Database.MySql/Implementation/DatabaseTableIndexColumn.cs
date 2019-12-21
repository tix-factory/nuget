using System;
using TixFactory.Configuration;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseTableIndexColumn"/>
	internal class DatabaseTableIndexColumn : IDatabaseTableIndexColumn
	{
		/// <inheritdoc cref="IDatabaseTableIndexColumn.Column"/>
		public IDatabaseTableColumn Column { get; }

		/// <inheritdoc cref="IDatabaseTableIndexColumn.SortOrder"/>
		public SortOrder? SortOrder { get; }

		/// <summary>
		/// Initializes a new <see cref="DatabaseTableIndexColumn"/>.
		/// </summary>
		/// <param name="column">The associated <see cref="IDatabaseTableColumn"/>.</param>
		/// <param name="showIndexResult">The <see cref="ShowIndexResult"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="column"/>
		/// - <paramref name="showIndexResult"/>
		/// </exception>
		public DatabaseTableIndexColumn(IDatabaseTableColumn column, ShowIndexResult showIndexResult)
		{
			if (showIndexResult == null)
			{
				throw new ArgumentNullException(nameof(showIndexResult));
			}

			Column = column ?? throw new ArgumentNullException(nameof(column));

			if (showIndexResult.Collation == "A")
			{
				SortOrder = Configuration.SortOrder.Ascending;
			}
			else if (showIndexResult.Collation == "D")
			{
				SortOrder = Configuration.SortOrder.Descending;
			}
		}
	}
}
