using MySql.Data.MySqlClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Represents table column information.
	/// </summary>
	public class TableColumn
	{
		/// <summary>
		/// The column name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The column <see cref="MySqlDbType"/>.
		/// </summary>
		public MySqlDbType MySqlDatabaseType { get; }

		/// <summary>
		/// The database type with extended detail (length, "is primary key", "is auto incrementing", unsigned).
		/// </summary>
		public string DatabaseType { get; }

		/// <summary>
		/// Initializes a new <see cref="TableColumn"/>.
		/// </summary>
		/// <param name="property">The <see cref="PropertyInfo"/> to parse the column from.</param>
		/// <param name="databaseTypeParser">An <see cref="IDatabaseTypeParser"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="property"/>
		/// - <paramref name="databaseTypeParser"/>
		/// </exception>
		public TableColumn(PropertyInfo property, IDatabaseTypeParser databaseTypeParser)
		{
			if (property == null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			if (databaseTypeParser == null)
			{
				throw new ArgumentNullException(nameof(databaseTypeParser));
			}

			var (databaseType, mySqlDatabaseType) = GetDatabaseType(property, databaseTypeParser);
			Name = GetName(property);
			DatabaseType = databaseType;
			MySqlDatabaseType = mySqlDatabaseType;
		}

		private string GetName(PropertyInfo property)
		{
			var dataMemberAttribute = property.GetCustomAttribute<DataMemberAttribute>();
			if (string.IsNullOrWhiteSpace(dataMemberAttribute?.Name))
			{
				return property.Name;
			}

			return dataMemberAttribute.Name;
		}

		private (string, MySqlDbType) GetDatabaseType(PropertyInfo property, IDatabaseTypeParser databaseTypeParser)
		{
			var maxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();
			var dataTypeAttribute = property.GetCustomAttribute<DataTypeAttribute>();
			var length = maxLengthAttribute?.Length;

			string databaseType;
			MySqlDbType mySqlDatabaseType;

			if (string.IsNullOrWhiteSpace(dataTypeAttribute?.CustomDataType))
			{
				databaseType = databaseTypeParser.GetDatabaseTypeName(property.PropertyType, length);
				mySqlDatabaseType = databaseTypeParser.GetMySqlType(property.PropertyType);
			}
			else
			{
				databaseType = dataTypeAttribute.CustomDataType;

				var parsedDatabaseType = databaseTypeParser.ParseDatabaseType(databaseType, nullable: false);
				mySqlDatabaseType = parsedDatabaseType.MySqlType;

				if (length.HasValue)
				{
					databaseType += $"({length})";
				}
			}

			if (HasAttribute<AutoIncrementColumnAttribute>(property))
			{
				databaseType += " AUTO_INCREMENT";
			}

			if (HasAttribute<PrimaryColumnAttribute>(property))
			{
				databaseType += " PRIMARY KEY";
			}

			return (databaseType, mySqlDatabaseType);
		}

		private bool HasAttribute<T>(PropertyInfo property)
			where T : Attribute
		{
			return property.GetCustomAttributes<T>(inherit: true).Any();
		}
	}
}
