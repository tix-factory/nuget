using System.Runtime.Serialization;

namespace TixFactory.ApplicationAuthorization
{
    [DataContract]
    internal class AuthorizedOperationsResult
    {
        [DataMember(Name = "data")]
        public string[] Data { get; set; }
    }
}
