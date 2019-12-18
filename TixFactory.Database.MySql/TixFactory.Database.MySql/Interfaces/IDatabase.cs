﻿namespace TixFactory.Database.MySql
{
	/// <summary>
	/// A MySQL database.
	/// </summary>
	public interface IDatabase
	{
		/// <summary>
		/// The database name.
		/// </summary>
		string Name { get; }
	}
}
