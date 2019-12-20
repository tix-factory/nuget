using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	[DataContract]
	internal class ShowColumnsResult
	{
		[DataMember(Name = "Field")]
		public string Name { get; set; }

		[DataMember(Name = "Type")]
		public string RawDataType { get; set; }

		[DataMember(Name = "Null")]
		public string IsNullable { get; set; }

		[DataMember(Name = "Key")]
		public string Key { get; set; }

		[DataMember(Name = "Default")]
		public object Default { get; set; }
		
		[DataMember(Name = "Extra")]
		public object Extra { get; set; }
	}
}
