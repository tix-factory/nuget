using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	[DataContract]
	internal class ShowDatabaseResult
	{
		[DataMember(Name = "Database")]
		public string DatabaseName { get; set; }
	}
}
