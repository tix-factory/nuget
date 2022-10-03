using System;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.Configuration;

namespace TixFactory.Queueing
{
    /// <summary>
    /// A monitor for an <see cref="IItemQueue{TItem}"/> that processes queue items when available.
    /// </summary>
    /// <typeparam name="TItem">The type of items the queue accepts.</typeparam>
    public interface IItemQueueProcessor<TItem>
    {
        /// <summary>
        /// Fired when an exception was thrown processing a queue item.
        /// </summary>
        event Action<TItem, Exception> OnUnhandledException;

        /// <summary>
        /// Gets the currently running processing <see cref="Task"/>.
        /// </summary>
        IReadOnlySetting<Task> RunningTask { get; }

        /// <summary>
        /// Starts listening for queue messages.
        /// </summary>
        /// <remarks>
        /// Calling after the processor has started will return the <see cref="RunningTask"/>.
        /// </remarks>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>The task that will run until the <see cref="CancellationToken"/> is cancelled.</returns>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the task started by <see cref="Start"/>.
        /// </summary>
        /// <remarks>
        /// Will not cancel the <see cref="CancellationToken"/> used in <see cref="Start"/>.
        /// Will not cancel running tasks triggered by start, but will stop more from starting.
        /// </remarks>
        void Stop();
    }
}
