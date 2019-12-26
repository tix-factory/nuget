using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using TixFactory.Serialization.Json;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[DataContract(Name = "test_table")]
	[ExcludeFromCodeCoverage]
	public class TestTable
	{
		[DataMember(Name = "ID")]
		[AutoIncrementColumn]
		public long Id { get; set; }

		[DataMember(Name = "Name")]
		[MaxLength(50)]
		[JsonConverter(typeof(Base64Converter))]
		public string Name { get; set; }

		[DataMember(Name = "Description")]
		[DataType("TEXT")]
		public string Description { get; set; }

		[DataMember(Name = "Value")]
		public int? Value { get; set; }

		[DataMember(Name = "Created")]
		[CreatedColumn]
		public DateTime Created { get; set; }

		[DataMember(Name = "Updated")]
		[UpdatedColumn]
		public DateTime Updated { get; set; }
	}
}
