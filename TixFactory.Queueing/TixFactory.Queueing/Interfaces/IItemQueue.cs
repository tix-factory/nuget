using System;
using TixFactory.Configuration;

namespace TixFactory.Queueing
{
	/// <summary>
	/// An item queue.
	/// </summary>
	/// <typeparam name="TItem">The item type.</typeparam>
	public interface IItemQueue<TItem>
	{
		/// <summary>
		/// The number of items in the queue.
		/// </summary>
		IReadOnlySetting<long> QueueSize { get; }

		/// <summary>
		/// The number of items currently being held.
		/// </summary>
		IReadOnlySetting<long> HeldQueueSize { get; }

		/// <summary>
		/// Appends an item to the end of the queue.
		/// </summary>
		/// <param name="item">The <typeparamref name="TItem"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="item"/>
		/// </exception>
		void AppendItemToQueue(TItem item);

		/// <summary>
		/// Tries to get the next item from the queue.
		/// </summary>
		/// <param name="lockExpiration">How long the item should be held for.</param>
		/// <param name="queueItem">The <see cref="QueueItem{TItem}"/>.</param>
		/// <returns><c>true</c> if there was an item in the queue to get (otherwise <c>false</c>).</returns>
		bool TryGetNextQueueItem(TimeSpan lockExpiration, out QueueItem<TItem> queueItem);

		/// <summary>
		/// Removes an item from the queue by id + holder id.
		/// </summary>
		/// <param name="id">The <see cref="QueueItem{TItem}.Id"/>.</param>
		/// <param name="holderId">The <see cref="QueueItem{TItem}.HolderId"/>.</param>
		void RemoveQueueItem(string id, string holderId);

		/// <summary>
		/// Releases the hold on a queue item by id + holder id.
		/// </summary>
		/// <param name="id">The <see cref="QueueItem{TItem}.Id"/>.</param>
		/// <param name="holderId">The <see cref="QueueItem{TItem}.HolderId"/>.</param>
		void ReleaseQueueItem(string id, string holderId);

		/// <summary>
		/// Clears the queue.
		/// </summary>
		void Clear();
	}
}
