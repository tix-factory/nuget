using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.Configuration;
using TixFactory.Logging;

namespace TixFactory.Queueing
{
	/// <inheritdoc cref="IItemQueueProcessor{TItem}"/>
	public class ItemQueueProcessor<TItem> : IItemQueueProcessor<TItem>
	{
		private readonly ISetting<Task> _RunningTask;

		private readonly IItemQueue<TItem> _ItemQueue;
		private readonly IItemQueueProcessorSettings _ItemQueueProcessorSettings;
		private readonly ILogger _Logger;
		private readonly Func<TItem, CancellationToken, Task<bool>> _ProcessItemFunc;
		private readonly SemaphoreSlim _StartLock;
		private readonly SemaphoreSlim _ProcessQueueLock;
		private readonly IList<Task> _RunningThreads;

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
			ILogger logger, Func<TItem, CancellationToken, Task<bool>> processItemFunc)
		{
			var runningTaks = new TrackedList<Task>();

			_ItemQueue = itemQueue ?? throw new ArgumentNullException(nameof(itemQueue));
			_ItemQueueProcessorSettings = itemQueueProcessorSettings ?? throw new ArgumentNullException(nameof(itemQueueProcessorSettings));
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ProcessItemFunc = processItemFunc ?? throw new ArgumentNullException(nameof(processItemFunc));
			_StartLock = new SemaphoreSlim(0, 1);
			_ProcessQueueLock = new SemaphoreSlim(1, 1);
			_RunningThreads = runningTaks;

			_RunningTask = new Setting<Task>(defaultValue: null);

			runningTaks.CountSetting.Changed += async (newCount, oldCold) => await ProcessQueueAsync(CancellationToken.None).ConfigureAwait(false);
			itemQueue.QueueSize.Changed += async (newQueueSize, oldQueueSize) => await ProcessQueueAsync(CancellationToken.None).ConfigureAwait(false);
			itemQueue.HeldQueueSize.Changed += async (newQueueSize, oldQueueSize) => await ProcessQueueAsync(CancellationToken.None).ConfigureAwait(false);
		}

		/// <inheritdoc cref="IItemQueueProcessor{TItem}.Start"/>
		public Task Start(CancellationToken cancellationToken)
		{
			var task = _RunningTask.Value;
			if (task != null)
			{
				return task;
			}

			task = _StartLock.WaitAsync(cancellationToken);
			_RunningTask.Value = task;

			var processTask = ProcessQueueAsync(cancellationToken);
			
			return Task.WhenAll(task, processTask);
		}

		/// <inheritdoc cref="IItemQueueProcessor{TItem}.Stop"/>
		public void Stop()
		{
			var runningTask = _RunningTask.Value;
			if (runningTask != null)
			{
				_StartLock.Release();
				_RunningTask.Value = null;
			}
		}

		private async Task ProcessItemAsync(QueueItem<TItem> queueItem, CancellationToken cancellationToken)
		{
			var holderId = queueItem.HolderId;

			try
			{
				var processed = await _ProcessItemFunc(queueItem.Value, cancellationToken).ConfigureAwait(false);
				if (processed)
				{
					await _ItemQueue.RemoveQueueItemAsync(queueItem.Id, holderId, cancellationToken).ConfigureAwait(false);
				}

				await Task.Delay(_ItemQueueProcessorSettings.ItemRetryDelay, cancellationToken).ConfigureAwait(false);
				await _ItemQueue.ReleaseQueueItemAsync(queueItem.Id, holderId, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				OnUnhandledException?.Invoke(queueItem.Value, e);
				await Task.Delay(_ItemQueueProcessorSettings.ItemLockDuration, cancellationToken).ConfigureAwait(false);
			}
		}

		private async Task ProcessQueueAsync(CancellationToken cancellationToken)
		{
			var runningTask = _RunningTask.Value;
			if (runningTask == null)
			{
				return;
			}

			if (_ProcessQueueLock.CurrentCount == 0)
			{
				return;
			}
			
			await _ProcessQueueLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			var threadClosureTasks = new List<Task>();

			try
			{
				var removalTasks = new List<Task>();
				foreach (var runningThread in _RunningThreads)
				{
					if (runningThread.IsCompleted)
					{
						removalTasks.Add(runningThread);
					}
				}

				// TODO: Verify this is always empty. The ContinueWith should have handled this.
				foreach (var removalTask in removalTasks)
				{
					_RunningThreads.Remove(removalTask);
				}

				for (var threadNumber = _RunningThreads.Count; threadNumber < _ItemQueueProcessorSettings.NumberOfThreads; threadNumber++)
				{
					Task runTask;

					if (_ItemQueue.TryGetNextQueueItem(_ItemQueueProcessorSettings.ItemLockDuration, out var queueItem))
					{
						runTask = ProcessItemAsync(queueItem, cancellationToken);
					}
					else
					{
						runTask = Task.Delay(_ItemQueueProcessorSettings.ThreadSleepTime, cancellationToken);
					}

					_RunningThreads.Add(runTask);
					threadClosureTasks.Add(runTask.ContinueWith(_RunningThreads.Remove, cancellationToken));
				}
			}
			catch (Exception e)
			{
				_Logger.Error(e);
			}
			finally
			{
				_ProcessQueueLock.Release();
			}

			await Task.WhenAll(threadClosureTasks).ConfigureAwait(false);
		}
	}
}
