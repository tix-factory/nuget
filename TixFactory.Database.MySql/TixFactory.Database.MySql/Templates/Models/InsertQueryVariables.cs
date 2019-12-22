using System.Collections.Generic;

namespace TixFactory.Database.MySql.Templates
{
	internal class InsertQueryVariables : QueryTemplateVariables
	{
		public string TableName { get; set; }

		public IReadOnlyCollection<InsertColumn> Columns { get; set; }
	}
}
