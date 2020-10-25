using System.Runtime.Serialization;

namespace TixFactory.Data.MySql
{
	[DataContract]
	internal class CountResult
	{
		[DataMember(Name = "Count")]
		public long Count { get; set; }
	}
}
