using System.Runtime.Serialization;

namespace TixFactory.Logging.Client
{
    [DataContract]
    internal class HostData
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
