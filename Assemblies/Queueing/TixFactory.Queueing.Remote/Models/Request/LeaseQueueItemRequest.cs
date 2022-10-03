using System;
using System.Runtime.Serialization;

namespace TixFactory.Queueing.Remote
{
    [DataContract]
    internal class LeaseQueueItemRequest
    {
        [DataMember(Name = "queueName")]
        public string QueueName { get; set; }

        [DataMember(Name = "leaseExpiry")]
        public TimeSpan LeaseExpiry { get; set; }
    }
}
