using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.Configuration;
using TixFactory.Logging;

namespace TixFactory.Queueing
{
	/// <inheritdoc cref="IItemQueueProcessor{TItem}"/>
	public class ItemQueueProcessor<TItem> : IItemQueueProcessor<TItem>
	{
		private int _RunId;

		private readonly IItemQueue<TItem> _ItemQueue;
		private readonly IItemQueueProcessorSettings _ItemQueueProcessorSettings;
		private readonly ILogger _Logger;
		private readonly Func<TItem, bool> _ProcessItemFunc;
		private readonly ISetting<Task> _RunningTask;

		/// <inheritdoc cref="IItemQueueProcessor{TItem}.OnUnhandledException"/>
		public event Action<TItem, Exception> OnUnhandledException;

		/// <inheritdoc cref="IItemQueueProcessor{TItem}.RunningTask"/>
		public IReadOnlySetting<Task> RunningTask => _RunningTask;

		/// <summary>
		/// Initializes a new <see cref="ItemQueueProcessor{TItem}"/>.
		/// </summary>
		/// <param name="itemQueue">The <see cref="IItemQueue{TItem}"/> to listen for.</param>
		/// <param name="itemQueueProcessorSettings">The <see cref="IItemQueueProcessorSettings"/>.</param>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <param name="processItemFunc">An action to process the messages.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="itemQueue"/>
		/// - <paramref name="itemQueueProcessorSettings"/>
		/// - <paramref name="logger"/>
		/// - <paramref name="processItemFunc"/>
		/// </exception>
		public ItemQueueProcessor(IItemQueue<TItem> itemQueue, IItemQueueProcessorSettings itemQueueProcessorSettings,
			ILogger logger, Func<TItem, bool> processItemFunc)
		{
			_ItemQueue = itemQueue ?? throw new ArgumentNullException(nameof(itemQueue));
			_ItemQueueProcessorSettings = itemQueueProcessorSettings ?? throw new ArgumentNullException(nameof(itemQueueProcessorSettings));
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ProcessItemFunc = processItemFunc ?? throw new ArgumentNullException(nameof(processItemFunc));
			_RunningTask = new Setting<Task>();
		}

		/// <inheritdoc cref="IItemQueueProcessor{TItem}.Start"/>
		public Task Start(CancellationToken cancellationToken)
		{
			var id = ++_RunId;

			var task = Task.Run(async () =>
			{
				var runningThreads = new List<RunningItem<TItem>>();

				while (id == _RunId && !cancellationToken.IsCancellationRequested)
				{
					Process(runningThreads, cancellationToken);
					await Task.Delay(_ItemQueueProcessorSettings.ThreadCheckInterval, cancellationToken).ConfigureAwait(false);
				}
			}, cancellationToken);

			_RunningTask.Value = task;
			return task;
		}

		/// <inheritdoc cref="IItemQueueProcessor{TItem}.Stop"/>
		public void Stop()
		{
			++_RunId;
		}

		private void Process(IList<RunningItem<TItem>> runningItems, CancellationToken cancellationToken)
		{
			foreach (var runningItem in runningItems.ToArray())
			{
				if (runningItem.Task.IsCompleted || runningItem.Expiration < DateTime.UtcNow)
				{
					runningItems.Remove(runningItem);
				}
			}

			try
			{
				for (var n = runningItems.Count; n < _ItemQueueProcessorSettings.NumberOfThreads; n++)
				{
					var lockDuration = _ItemQueueProcessorSettings.ItemLockDuration;
					var hasNextItem = _ItemQueue.TryGetNextQueueItem(lockDuration, out var queueItem);

					var runningItem = new RunningItem<TItem>
					{
						Item = queueItem.Value,
						Expiration = DateTime.UtcNow + lockDuration
					};

					if (hasNextItem)
					{
						runningItem.Task = Task.Run(() => ProcessItem(queueItem.Id, queueItem.Value, queueItem.HolderId), cancellationToken);
						runningItems.Add(runningItem);
					}
					else
					{
						// If there's no item to process, put the thread to sleep for now so we don't try to get an item
						// for it in a few milliseconds.
						runningItem.Task = Task.Delay(_ItemQueueProcessorSettings.ThreadSleepTime, cancellationToken);
						runningItems.Add(runningItem);

						break;
					}
				}

				for (var n = runningItems.Count; n < _ItemQueueProcessorSettings.NumberOfThreads; n++)
				{
					var runningItem = new RunningItem<TItem>
					{
						Item = default(TItem),
						Expiration = DateTime.UtcNow + _ItemQueueProcessorSettings.ItemLockDuration,
						Task = Task.Delay(_ItemQueueProcessorSettings.ThreadSleepTime, cancellationToken)
					};

					runningItems.Add(runningItem);
				}
			}
			catch (Exception e)
			{
				_Logger.Error(e);
			}
		}

		private void ProcessItem(string id, TItem item, string holderId)
		{
			try
			{
				var processed = _ProcessItemFunc(item);

				if (processed)
				{
					_ItemQueue.RemoveQueueItem(id, holderId);
					return;
				}

				Thread.Sleep(_ItemQueueProcessorSettings.ItemRetryDelay);
				_ItemQueue.ReleaseQueueItem(id, holderId);
			}
			catch (Exception e)
			{
				OnUnhandledException?.Invoke(item, e);
				// If the item errors don't release it, let the lock expire before it gets retried.
			}
		}
	}
}
