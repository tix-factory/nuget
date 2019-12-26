using System;
using System.Linq;
using System.Reflection;

namespace TixFactory.Database.MySql
{
	internal class InsertColumn : TableColumn
	{
		public PropertyInfo Property { get; }

		public string ParameterName { get; }

		public string InsertValue { get; }

		public InsertColumn(PropertyInfo property, bool isUpdate, IDatabaseTypeParser databaseTypeParser)
			: base(property, databaseTypeParser)
		{
			Property = property ?? throw new ArgumentNullException(nameof(property));

			if (isUpdate)
			{
				if (HasAttribute<ImmutableColumnAttribute>(property))
				{
					return;
				}
			}
			else
			{
				if (HasAttribute<AutoIncrementColumnAttribute>(property))
				{
					return;
				}
			}

			if (HasAttribute<CreatedColumnAttribute>(property))
			{
				InsertValue = "UTC_Timestamp()";
			}
			else if (HasAttribute<UpdatedColumnAttribute>(property))
			{
				InsertValue = "UTC_Timestamp()";
			}
			else
			{
				ParameterName = property.Name;
				InsertValue = $"@{property.Name}";
			}
		}

		private bool HasAttribute<T>(PropertyInfo property)
			where T : Attribute
		{
			return property.GetCustomAttributes<T>(inherit: true).Any();
		}
	}
}
