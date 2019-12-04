using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.Configuration;

namespace TixFactory.Queueing
{
	/// <summary>
	/// An in-memory <seealso cref="IItemQueue{TItem}"/>.
	/// </summary>
	/// <typeparam name="TItem">The item type.</typeparam>
	public class VirtualItemQueue<TItem> : IItemQueue<TItem>
	{
		private readonly IList<QueueItem<TItem>> _Queue;
		private readonly IManufacturedSetting<long> _QueueSize;
		private readonly IManufacturedSetting<long> _HeldQueueSize;

		/// <inheritdoc cref="IItemQueue{TItem}.QueueSize"/>
		public IReadOnlySetting<long> QueueSize => _QueueSize;

		/// <inheritdoc cref="IItemQueue{TItem}.HeldQueueSize"/>
		public IReadOnlySetting<long> HeldQueueSize => _HeldQueueSize;

		/// <summary>
		/// Initializes a new <seealso cref="VirtualItemQueue{TItem}"/>.
		/// </summary>
		public VirtualItemQueue()
		{
			_Queue = new List<QueueItem<TItem>>();
			_QueueSize = new ManufacturedSetting<long>(() => _Queue.Count, refreshOnRead: true);
			_HeldQueueSize = new ManufacturedSetting<long>(() => _Queue.Count(IsHeld), refreshOnRead: true);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.AppendItemToQueue"/>
		public void AppendItemToQueue(TItem item)
		{
			var queueItem = new QueueItem<TItem>(Guid.NewGuid().ToString(), item);

			_Queue.Add(queueItem);
			CheckSizes();
		}

		/// <inheritdoc cref="IItemQueue{TItem}.AppendItemToQueueAsync"/>
		public Task AppendItemToQueueAsync(TItem item, CancellationToken cancellationToken)
		{
			AppendItemToQueue(item);
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="IItemQueue{TItem}.TryGetNextQueueItem"/>
		public bool TryGetNextQueueItem(TimeSpan lockExpiration, out QueueItem<TItem> queueItem)
		{
			queueItem = _Queue.FirstOrDefault(i => !IsHeld(i));
			if (queueItem == null)
			{
				return false;
			}

			queueItem.HolderId = Guid.NewGuid().ToString();
			queueItem.LockExpiration = DateTime.UtcNow + lockExpiration;
			CheckSizes();

			ThreadPool.QueueUserWorkItem(state =>
			{
				Thread.Sleep(lockExpiration);
				CheckSizes();
			});

			return true;
		}

		/// <inheritdoc cref="IItemQueue{TItem}.TryGetNextQueueItem"/>
		public Task<(bool, QueueItem<TItem>)> TryGetNextQueueItemAsync(TimeSpan lockExpiration, CancellationToken cancellationToken)
		{
			var hasNextQueueItem = TryGetNextQueueItem(lockExpiration, out var queueItem);
			return Task.FromResult((hasNextQueueItem, queueItem));
		}

		/// <inheritdoc cref="IItemQueue{TItem}.RemoveQueueItem"/>
		public void RemoveQueueItem(string id, string holderId)
		{
			var item = _Queue.FirstOrDefault(i => i.Id == id && i.HolderId == holderId);
			if (item == null)
			{
				return;
			}

			_Queue.Remove(item);
			CheckSizes();
		}

		/// <inheritdoc cref="IItemQueue{TItem}.RemoveQueueItemAsync"/>
		public Task RemoveQueueItemAsync(string id, string holderId, CancellationToken cancellationToken)
		{
			RemoveQueueItem(id, holderId);
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ReleaseQueueItem"/>
		public void ReleaseQueueItem(string id, string holderId)
		{
			var item = _Queue.FirstOrDefault(i => i.Id == id && i.HolderId == holderId);
			if (item == null)
			{
				return;
			}

			item.LockExpiration = DateTime.MinValue;
			CheckSizes();
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ReleaseQueueItemAsync"/>
		public Task ReleaseQueueItemAsync(string id, string holderId, CancellationToken cancellationToken)
		{
			ReleaseQueueItem(id, holderId);
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="IItemQueue{TItem}.Clear"/>
		public void Clear()
		{
			_Queue.Clear();
			CheckSizes();
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ClearAsync"/>
		public Task ClearAsync(CancellationToken cancellationToken)
		{
			Clear();
			return Task.CompletedTask;
		}

		private bool IsHeld(QueueItem<TItem> queueItem)
		{
			return queueItem.LockExpiration > DateTime.UtcNow;
		}

		private void CheckSizes()
		{
			_QueueSize.Refresh();
			_HeldQueueSize.Refresh();
		}
	}
}
