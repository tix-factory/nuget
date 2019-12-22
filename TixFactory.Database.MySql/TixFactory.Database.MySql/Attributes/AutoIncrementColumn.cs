using System;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// An attribute that signals this column is auto-incrementing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class AutoIncrementColumnAttribute : ImmutableColumnAttribute
	{
	}
}
