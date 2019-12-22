using System;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// An attribute that signals this table column is the created <see cref="DateTime"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class CreatedColumnAttribute : ImmutableColumnAttribute
	{
	}
}
