using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TixFactory.Configuration;
using TixFactory.Logging;
using TixFactory.Queueing;

namespace TixFactory.Processors.Queueing
{
	/// <inheritdoc cref="IQueueProcessor"/>
	public class QueueProcessor<TItem> : IQueueProcessor
	{
		private readonly IItemQueue<TItem> _ItemQueue;
		private readonly IQueueItemHandler<TItem> _QueueItemHandler;
		private readonly IQueueProcessorSettings _QueueProcessorSettings;
		private readonly ILogger _Logger;
		private readonly SemaphoreSlim _ProcessQueueLock;
		private readonly TrackedList<Task> _RunningTasks;
		private bool _Started;

		/// <summary>
		/// Initializes a new <see cref="IItemQueue{TItem}"/>.
		/// </summary>
		/// <param name="itemQueue">An <see cref="IItemQueue{TItem}"/>.</param>
		/// <param name="queueItemHandler">An <see cref="IQueueItemHandler{TItem}"/>.</param>
		/// <param name="queueProcessorSettings">An <see cref="ItemQueueProcessorSettings"/>.</param>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="itemQueue"/>
		/// - <paramref name="queueItemHandler"/>
		/// - <paramref name="queueProcessorSettings"/>
		/// - <paramref name="logger"/>
		/// </exception>
		public QueueProcessor(IItemQueue<TItem> itemQueue, IQueueItemHandler<TItem> queueItemHandler, IQueueProcessorSettings queueProcessorSettings, ILogger logger)
		{
			_ItemQueue = itemQueue ?? throw new ArgumentNullException(nameof(itemQueue));
			_QueueItemHandler = queueItemHandler ?? throw new ArgumentNullException(nameof(queueItemHandler));
			_QueueProcessorSettings = queueProcessorSettings ?? throw new ArgumentNullException(nameof(queueProcessorSettings));
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));

			var runningTaks = _RunningTasks = new TrackedList<Task>();
			_ProcessQueueLock = new SemaphoreSlim(1, 1);

			runningTaks.CountSetting.Changed += (newCount, oldCold) => ProcessQueue();
			itemQueue.QueueSize.Changed += (newQueueSize, oldQueueSize) => ProcessQueue();
			itemQueue.HeldQueueSize.Changed += (newQueueSize, oldQueueSize) => ProcessQueue();
		}

		/// <inheritdoc cref="IQueueProcessor.Start"/>
		public void Start()
		{
			_Started = true;
		}

		/// <inheritdoc cref="IQueueProcessor.Stop"/>
		public void Stop()
		{
			_Started = false;

			while (_RunningTasks.Any())
			{
				ClearFinishedTasks();
				Thread.Sleep(TimeSpan.FromMilliseconds(10));
			}
		}

		private void ProcessItem(QueueItem<TItem> queueItem)
		{
			var id = queueItem.Id;
			var holderId = queueItem.HolderId;

			try
			{
				var processed = _QueueItemHandler.ProcessItem(queueItem.Value);
				if (processed)
				{
					_ItemQueue.RemoveQueueItem(id, holderId);
					return;
				}

				Thread.Sleep(_QueueProcessorSettings.ItemRetryDelay);
				_ItemQueue.ReleaseQueueItem(id, holderId);
			}
			catch (Exception e)
			{
				var serializedQueueItem = JsonConvert.SerializeObject(queueItem);
				_Logger.Error($"Error processing queue item.\n\tQueue Item ID: {queueItem.Id}\n\tItem type: {typeof(TItem).FullName}\n\tItem: {serializedQueueItem}\n{e}");
			}
		}

		private Task ProcessItemAsync(QueueItem<TItem> queueItem)
		{
			return Task.Run(() => ProcessItem(queueItem));
		}

		private void ProcessQueue()
		{
			if (!_Started)
			{
				return;
			}

			if (_ProcessQueueLock.CurrentCount == 0)
			{
				return;
			}

			_ProcessQueueLock.Wait();

			try
			{
				ClearFinishedTasks();

				for (var threadNumber = _RunningTasks.Count; threadNumber < _QueueProcessorSettings.NumberOfThreads; threadNumber++)
				{
					Task runTask;

					if (_ItemQueue.TryGetNextQueueItem(_QueueProcessorSettings.ItemLockDuration, out var queueItem))
					{
						runTask = ProcessItemAsync(queueItem);
					}
					else
					{
						runTask = Task.Delay(_QueueProcessorSettings.ThreadSleepTime);
					}

					_RunningTasks.Add(runTask.ContinueWith(t =>
					{
						_RunningTasks.Remove(runTask);
					}));
				}
			}
			finally
			{
				_ProcessQueueLock.Release();
			}
		}

		private void ClearFinishedTasks()
		{
			try
			{
				var removalTasks = new List<Task>();
				foreach (var runningThread in _RunningTasks)
				{
					if (runningThread.IsCompleted)
					{
						removalTasks.Add(runningThread);
					}
				}
				foreach (var removalTask in removalTasks)
				{
					_RunningTasks.Remove(removalTask);
				}
			}
			catch (Exception e)
			{
				_Logger.Error($"Error clearing out running theads.\n{e}");
			}
		}
	}
}
