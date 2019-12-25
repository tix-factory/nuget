using System.Collections.Generic;

namespace TixFactory.Database.MySql.Templates
{
	internal class CreateTableQueryVariables : QueryTemplateVariables
	{
		public string TableName { get; set; }

		public IReadOnlyCollection<CreateTableColumn> Columns { get; set; }
	}
}
