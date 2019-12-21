using System;
using System.Runtime.Serialization;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[DataContract]
	public class TestTable
	{
		[DataMember(Name = "ID")]
		public long Id { get; set; }

		[DataMember(Name = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "IsSelectable")]
		public bool Selectable { get; set; }

		[DataMember(Name = "Created")]
		public DateTime Created { get; set; }
	}
}
