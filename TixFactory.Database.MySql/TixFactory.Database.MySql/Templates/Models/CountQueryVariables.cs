namespace TixFactory.Database.MySql.Templates
{
	internal class CountQueryVariables : QueryTemplateVariables
	{
		public string TableName { get; set; }

		public string WhereClause { get; set; }
	}
}
