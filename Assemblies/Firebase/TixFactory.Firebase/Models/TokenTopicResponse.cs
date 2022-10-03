using System;
using System.Runtime.Serialization;

namespace TixFactory.Firebase
{
    [DataContract]
    internal class TokenTopicResponse
    {
        [DataMember(Name = "addDate")]
        public DateTime Created { get; set; }
    }
}
