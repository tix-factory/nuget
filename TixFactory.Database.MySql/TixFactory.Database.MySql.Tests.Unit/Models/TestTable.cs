using System;
using System.Runtime.Serialization;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[DataContract]
	public class TestTable
	{
		[DataMember(Name = "ID")]
		[AutoIncrementColumn]
		public long Id { get; set; }

		[DataMember(Name = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "Created")]
		[CreatedColumn]
		public DateTime Created { get; set; }

		[DataMember(Name = "Updated")]
		[UpdatedColumn]
		public DateTime Updated { get; set; }
	}
}
