using System.Runtime.Serialization;

namespace TixFactory.Queueing.Remote
{
    [DataContract]
    internal class Payload<T>
    {
        [DataMember(Name = "data")]
        public T Data { get; set; }

        [DataMember(Name = "error")]
        public PayloadError Error { get; set; }
    }
}
