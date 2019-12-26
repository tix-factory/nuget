using System;
using System.ComponentModel;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// An attribute that signals this table column is the created <see cref="DateTime"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class CreatedColumnAttribute : ImmutableColumnAttribute
	{
		/// <summary>
		/// The <see cref="DateTimeKind"/> for the created date.
		/// </summary>
		public DateTimeKind DateTimeKind { get; }

		/// <summary>
		/// Initializes a new <see cref="CreatedColumnAttribute"/>.
		/// </summary>
		/// <param name="dateTimeKind">The <see cref="DateTimeKind"/>.</param>
		/// <exception cref="InvalidEnumArgumentException">
		/// - <paramref name="dateTimeKind"/>
		/// </exception>
		public CreatedColumnAttribute(DateTimeKind dateTimeKind = DateTimeKind.Utc)
		{
			if (!Enum.IsDefined(typeof(DateTimeKind), dateTimeKind))
			{
				throw new InvalidEnumArgumentException(nameof(dateTimeKind), (int) dateTimeKind, typeof(DateTimeKind));
			}

			DateTimeKind = dateTimeKind;
		}
	}
}
