using System;

namespace TixFactory.Queueing.Remote
{
    /// <summary>
    /// A factory for creating <see cref="RemoteItemQueueV3{TItem}"/>s.
    /// </summary>
    public interface IRemoteItemQueueFactory
    {
        /// <summary>
        /// Creates a <see cref="RemoteItemQueueV3{TItem}"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="countRefreshInterval"/> will round up to a minimum value of one second.
        /// </remarks>
        /// <typeparam name="TItem">The queue item type.</typeparam>
        /// <param name="queueName">The remote queue name.</param>
        /// <param name="countRefreshInterval">The <see cref="TimeSpan"/> for how often to refresh the queue size.</param>
        /// <returns>The <see cref="RemoteItemQueueV3{TItem}"/>.</returns>
        /// <exception cref="ArgumentException">
        /// - <paramref name="queueName"/> is <c>null</c> or whitespace.
        /// </exception>
        IRemoteItemQueue<TItem> CreateRemoteItemQueue<TItem>(string queueName, TimeSpan? countRefreshInterval);
    }
}
