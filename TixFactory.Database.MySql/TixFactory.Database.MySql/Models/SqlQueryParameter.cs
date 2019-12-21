using System;
using System.ComponentModel;
using System.Data;

namespace TixFactory.Database.MySql
{
	public class SqlQueryParameter
	{
		public string Name { get; set; }

		public string DatabaseTypeName { get; set; }

		public long? Length { get; set; }

		public ParameterDirection ParameterDirection { get; set; }

		public SqlQueryParameter(string name, string databaseTypeName, long? length, ParameterDirection parameterDirection)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			}

			if (string.IsNullOrWhiteSpace(databaseTypeName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(databaseTypeName));
			}

			if (!Enum.IsDefined(typeof(ParameterDirection), parameterDirection))
			{
				throw new InvalidEnumArgumentException(nameof(parameterDirection), (int) parameterDirection, typeof(ParameterDirection));
			}

			Name = name;
			DatabaseTypeName = databaseTypeName;
			Length = length;
			ParameterDirection = parameterDirection;
		}
	}
}
