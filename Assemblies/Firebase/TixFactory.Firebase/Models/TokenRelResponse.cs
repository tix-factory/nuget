using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TixFactory.Firebase
{
    [DataContract]
    internal class TokenRelResponse
    {
        [DataMember(Name = "topics")]
        public IDictionary<string, TokenTopicResponse> Topics { get; set; }
    }
}
