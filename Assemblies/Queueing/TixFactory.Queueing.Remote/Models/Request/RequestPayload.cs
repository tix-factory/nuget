using System.Runtime.Serialization;

namespace TixFactory.Queueing.Remote
{
	[DataContract]
	internal class RequestPayload<T>
	{
		[DataMember(Name = "data")]
		public T Data { get; set; }
	}
}
