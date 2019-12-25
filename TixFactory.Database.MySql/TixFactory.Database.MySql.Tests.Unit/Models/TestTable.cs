using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[DataContract(Name = "test_table")]
	public class TestTable
	{
		[DataMember(Name = "ID")]
		[AutoIncrementColumn]
		public long Id { get; set; }

		[DataMember(Name = "Name")]
		[MaxLength(50)]
		public string Name { get; set; }

		[DataMember(Name = "Created")]
		[CreatedColumn]
		public DateTime Created { get; set; }

		[DataMember(Name = "Updated")]
		[UpdatedColumn]
		public DateTime Updated { get; set; }
	}
}
