using System.Runtime.Serialization;

namespace TixFactory.Queueing.Remote
{
    /// <summary>
    /// A wrapper for a remote queue item.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    [DataContract]
    internal class RemoteQueueItem<TItem>
    {
        /// <summary>
        /// The item.
        /// </summary>
        [DataMember(Name = "data")]
        public TItem Data { get; set; }
    }
}
