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
		private readonly SemaphoreSlim _QueueLock;
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
			_QueueLock = new SemaphoreSlim(1, 1);
			_Queue = new List<QueueItem<TItem>>();
			_QueueSize = new ManufacturedSetting<long>(() => _Queue.Count, refreshOnRead: true);
			_HeldQueueSize = new ManufacturedSetting<long>(() => _Queue.Count(IsHeld), refreshOnRead: true);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.AppendItemToQueue"/>
		public void AppendItemToQueue(TItem item)
		{
			_QueueLock.Wait();

			try
			{
				var queueItem = new QueueItem<TItem>(Guid.NewGuid().ToString(), item);

				_Queue.Add(queueItem);
				CheckSizes();
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.AppendItemToQueueAsync"/>
		public async Task AppendItemToQueueAsync(TItem item, CancellationToken cancellationToken)
		{
			await _QueueLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				var queueItem = new QueueItem<TItem>(Guid.NewGuid().ToString(), item);

				_Queue.Add(queueItem);
				await CheckSizesAsync(cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.TryGetNextQueueItem"/>
		public bool TryGetNextQueueItem(TimeSpan lockExpiration, out QueueItem<TItem> queueItem)
		{
			_QueueLock.Wait();

			try
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
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.TryGetNextQueueItem"/>
		public async Task<(bool, QueueItem<TItem>)> TryGetNextQueueItemAsync(TimeSpan lockExpiration, CancellationToken cancellationToken)
		{
			await _QueueLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				var queueItem = _Queue.FirstOrDefault(i => !IsHeld(i));
				if (queueItem == null)
				{
					return (false, null);
				}

				queueItem.HolderId = Guid.NewGuid().ToString();
				queueItem.LockExpiration = DateTime.UtcNow + lockExpiration;

				ThreadPool.QueueUserWorkItem(async state =>
				{
					await CheckSizesAsync(cancellationToken).ConfigureAwait(false);
					await Task.Delay(lockExpiration, cancellationToken).ConfigureAwait(false);
					await CheckSizesAsync(cancellationToken).ConfigureAwait(false);
				});

				return (true, queueItem);
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.RemoveQueueItem"/>
		public void RemoveQueueItem(string id, string holderId)
		{
			_QueueLock.Wait();

			try
			{
				var item = _Queue.FirstOrDefault(i => i.Id == id && i.HolderId == holderId);
				if (item == null)
				{
					return;
				}

				_Queue.Remove(item);
				CheckSizes();
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.RemoveQueueItemAsync"/>
		public async Task RemoveQueueItemAsync(string id, string holderId, CancellationToken cancellationToken)
		{
			await _QueueLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				var item = _Queue.FirstOrDefault(i => i.Id == id && i.HolderId == holderId);
				if (item == null)
				{
					return;
				}

				_Queue.Remove(item);
				await CheckSizesAsync(cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ReleaseQueueItem"/>
		public void ReleaseQueueItem(string id, string holderId)
		{
			_QueueLock.Wait();

			try
			{
				var item = _Queue.FirstOrDefault(i => i.Id == id && i.HolderId == holderId);
				if (item == null)
				{
					return;
				}

				item.LockExpiration = DateTime.MinValue;
				CheckSizes();
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ReleaseQueueItemAsync"/>
		public async Task ReleaseQueueItemAsync(string id, string holderId, CancellationToken cancellationToken)
		{
			await _QueueLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				var item = _Queue.FirstOrDefault(i => i.Id == id && i.HolderId == holderId);
				if (item == null)
				{
					return;
				}

				item.LockExpiration = DateTime.MinValue;
				await CheckSizesAsync(cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.Clear"/>
		public void Clear()
		{
			_QueueLock.Wait();

			try
			{
				_Queue.Clear();
				CheckSizes();
			}
			finally
			{
				_QueueLock.Release();
			}
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ClearAsync"/>
		public async Task ClearAsync(CancellationToken cancellationToken)
		{
			await _QueueLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				_Queue.Clear();
				await CheckSizesAsync(cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				_QueueLock.Release();
			}
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

		private Task CheckSizesAsync(CancellationToken cancellationToken)
		{
			return Task.Run(() => CheckSizes(), cancellationToken);
		}
	}
}
