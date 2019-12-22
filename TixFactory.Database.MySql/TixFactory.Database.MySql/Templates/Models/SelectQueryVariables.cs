namespace TixFactory.Database.MySql.Templates
{
	internal class SelectQueryVariables : QueryTemplateVariables
	{
		public string TableName { get; set; }

		public string WhereClause { get; set; }

		public string OrderBy { get; set; }
	}
}
