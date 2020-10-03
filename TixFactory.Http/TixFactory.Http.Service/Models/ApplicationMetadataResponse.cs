using System.Runtime.Serialization;

namespace TixFactory.Http.Service
{
	/// <summary>
	/// Metadata about the running application.
	/// </summary>
	[DataContract]
	public class ApplicationMetadataResponse
	{
		/// <summary>
		/// The name of the running application.
		/// </summary>
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}
