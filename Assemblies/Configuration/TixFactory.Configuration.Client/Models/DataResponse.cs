using System.Runtime.Serialization;

namespace TixFactory.Configuration.Client
{
	[DataContract]
	internal class DataResponse<T>
	{
		[DataMember(Name = "data")]
		public T Data { get; set; }
	}
}
