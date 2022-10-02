using System;

namespace TixFactory.Queueing
{
	/// <summary>
	/// A queue item.
	/// </summary>
	/// <typeparam name="TItem">The item type.</typeparam>
	public class QueueItem<TItem>
	{
		/// <summary>
		/// The queued item id.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// The id of the item holder.
		/// </summary>
		public string HolderId { get; set; }

		/// <summary>
		/// The item.
		/// </summary>
		public TItem Value { get; }

		/// <summary>
		/// The lock expiration for the hold of the item.
		/// </summary>
		public DateTime LockExpiration { get; set; }

		/// <summary>
		/// Initializes a new <see cref="QueueItem{TItem}"/>.
		/// </summary>
		/// <param name="id">The <see cref="Id"/>.</param>
		/// <param name="value">The <see cref="Value"/>.</param>
		public QueueItem(string id, TItem value)
		{
			Id = id;
			HolderId = null;
			Value = value;
			LockExpiration = DateTime.MinValue;
		}
	}
}
