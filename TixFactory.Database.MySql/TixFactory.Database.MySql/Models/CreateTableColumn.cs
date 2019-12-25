using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	internal class CreateTableColumn
	{
		public string ColumnName { get; }
		public string DatabaseType { get; }

		public CreateTableColumn(PropertyInfo columnProperty, IDatabaseTypeParser databaseTypeParser)
		{
			if (columnProperty == null)
			{
				throw new ArgumentNullException(nameof(columnProperty));
			}

			if (databaseTypeParser == null)
			{
				throw new ArgumentNullException(nameof(databaseTypeParser));
			}

			ColumnName = GetName(columnProperty);
			DatabaseType = GetDatabaseType(columnProperty, databaseTypeParser);
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

		private string GetDatabaseType(PropertyInfo property, IDatabaseTypeParser databaseTypeParser)
		{
			var maxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();
			var databaseType = databaseTypeParser.GetDatabaseTypeName(property.PropertyType, maxLengthAttribute?.Length);
			
			if (HasAttribute<AutoIncrementColumnAttribute>(property))
			{
				databaseType += " AUTO_INCREMENT";
			}

			if (HasAttribute<PrimaryColumnAttribute>(property))
			{
				databaseType += " PRIMARY KEY";
			}

			return databaseType;
		}

		private bool HasAttribute<T>(PropertyInfo property)
			where T : Attribute
		{
			return property.GetCustomAttributes<T>(inherit: true).Any();
		}
	}
}
