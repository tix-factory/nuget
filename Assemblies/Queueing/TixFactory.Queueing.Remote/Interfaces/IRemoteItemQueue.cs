using System;

namespace TixFactory.Queueing.Remote
{
    /// <summary>
    /// An <see cref="IItemQueue{TItem}"/> designed for tix-factory/queue-service.
    /// </summary>
    /// <remarks>
    /// https://github.com/tix-factory/queue-service
    /// </remarks>
    /// <typeparam name="TItem">The remote queue item.</typeparam>
    public interface IRemoteItemQueue<TItem> : IItemQueue<TItem>
    {
        /// <summary>
        /// An event fired when a item can't be parsed into <typeparamref name="TItem"/>.
        /// </summary>
        event Action<QueueItemResult> OnInvalidItem;

        /// <summary>
        /// The name of the remote queue.
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Refreshes the values for <see cref="IItemQueue{TItem}.QueueSize"/> and <see cref="IItemQueue{TItem}.HeldQueueSize"/>.
        /// </summary>
        void RefreshQueueSize();
    }
}
