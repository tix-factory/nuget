using System.Runtime.Serialization;

namespace TixFactory.Logging.Client
{
    [DataContract]
    internal class LogRequest
    {
        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "log")]
        public LogData Log { get; set; }

        [DataMember(Name = "host")]
        public HostData Host { get; set; }
    }
}
