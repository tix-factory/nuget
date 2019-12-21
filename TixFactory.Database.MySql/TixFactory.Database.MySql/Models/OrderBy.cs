using System;
using System.ComponentModel;
using TixFactory.Configuration;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// ORDER BY parameters for <see cref="ISqlQueryBuilder"/>.
	/// </summary>
	/// <typeparam name="TRow">The model representing a table row of the table being sorted.</typeparam>
	public class OrderBy<TRow>
		where TRow : class
	{
		/// <summary>
		/// The property of the row model to order by.
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// The <see cref="SortOrder"/>.
		/// </summary>
		public SortOrder SortOrder { get; }

		/// <summary>
		/// Initializes a new <see cref="OrderBy{TRow}"/>.
		/// </summary>
		/// <param name="propertyName">The <see cref="PropertyName"/>.</param>
		/// <param name="sortOrder">The <see cref="SortOrder"/>.</param>
		/// <exception cref="ArgumentException">
		/// - <paramref name="propertyName"/> is <c>null</c> or whitespace.
		/// - <paramref name="propertyName"/> is not a property of <typeparamref name="TRow"/>.
		/// - <paramref name="sortOrder"/> not valid.
		/// </exception>
		public OrderBy(string propertyName, SortOrder sortOrder)
		{
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));
			}

			if (!Enum.IsDefined(typeof(SortOrder), sortOrder))
			{
				throw new InvalidEnumArgumentException(nameof(sortOrder), (int)sortOrder, typeof(SortOrder));
			}

			var rowType = typeof(TRow);
			var orderByProperty = rowType.GetProperty(propertyName);
			if (orderByProperty == null)
			{
				throw new ArgumentException($"'{propertyName}' is not a valid property on '{rowType.Name}'", nameof(propertyName));
			}

			PropertyName = propertyName;
			SortOrder = sortOrder;
		}
	}
}
