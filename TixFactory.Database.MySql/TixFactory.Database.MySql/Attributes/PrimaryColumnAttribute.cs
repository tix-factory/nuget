using System;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// An attribute that signals this column is primary column for the table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PrimaryColumnAttribute : ImmutableColumnAttribute
	{
	}
}
