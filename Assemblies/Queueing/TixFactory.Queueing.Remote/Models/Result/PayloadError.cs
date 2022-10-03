using System.Runtime.Serialization;

namespace TixFactory.Queueing.Remote
{
	[DataContract]
	internal class PayloadError
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }
	}
}
