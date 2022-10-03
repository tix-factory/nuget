using System.Runtime.Serialization;

namespace TixFactory.Firebase
{
    [DataContract]
    internal class FirebaseMessage
    {
        /// <summary>
        /// Where to send the message.
        /// </summary>
        [DataMember(Name = "to", EmitDefaultValue = false)]
        public string To { get; set; }

        /// <summary>
        /// The message data.
        /// </summary>
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public object Data { get; set; }
    }
}
