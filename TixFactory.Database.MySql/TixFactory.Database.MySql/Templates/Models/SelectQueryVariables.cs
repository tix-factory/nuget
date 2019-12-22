namespace TixFactory.Database.MySql
{
	internal class SelectQueryVariables : QueryTemplateVariables
	{
		public string TableName { get; set; }

		public string WhereClause { get; set; }

		public string OrderBy { get; set; }
	}
}
