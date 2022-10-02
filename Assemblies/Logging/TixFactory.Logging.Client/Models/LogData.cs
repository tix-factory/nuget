using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TixFactory.Logging.Client
{
	[DataContract]
	internal class LogData
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "level")]
		[JsonConverter(typeof(StringEnumConverter))]
		public LogLevel Level { get; set; }
	}
}
