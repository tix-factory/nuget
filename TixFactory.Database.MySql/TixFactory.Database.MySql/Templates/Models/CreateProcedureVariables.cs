using System.Collections.Generic;

namespace TixFactory.Database.MySql.Templates
{
	internal class CreateProcedureVariables : QueryTemplateVariables
	{
		public string StoredProcedureName { get; set; }
		public string Delimiter { get; set; }
		public string Query { get; set; }
		public IReadOnlyCollection<SqlQueryParameter> Parameters { get; set; }
	}
}
