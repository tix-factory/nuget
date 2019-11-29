using System;

namespace TixFactory.Queueing
{
	/// <summary>
	/// A queue item.
	/// </summary>
	/// <typeparam name="TItem">The item type.</typeparam>
	public struct QueueItem<TItem>
	{
		/// <summary>
		/// The queued item id.
		/// </summary>
		public string Id { get; internal set; }

		/// <summary>
		/// The id of the item holder.
		/// </summary>
		public string HolderId { get; internal set; }

		/// <summary>
		/// The item.
		/// </summary>
		public TItem Value { get; internal set; }

		/// <summary>
		/// The lock expiration for the hold of the item.
		/// </summary>
		public DateTime LockExpiration { get; internal set; }
	}
}
