namespace TixFactory.Database.MySql.Templates
{
	internal class DeleteQueryVariables : QueryTemplateVariables
	{
		public string TableName { get; set; }

		public string WhereClause { get; set; }
	}
}
