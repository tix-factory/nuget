using System.Runtime.Serialization;

namespace TixFactory.Firebase
{
	[DataContract]
	internal class TokenDetailsResponse
	{
		[DataMember(Name = "rel")]
		public TokenRelResponse Rel { get; set; }
	}
}
