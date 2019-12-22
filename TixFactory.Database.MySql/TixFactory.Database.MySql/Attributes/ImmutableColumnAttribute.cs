using System;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// An attribute that signals this column is immutable (may not be updated).
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ImmutableColumnAttribute : Attribute
	{
	}
}
