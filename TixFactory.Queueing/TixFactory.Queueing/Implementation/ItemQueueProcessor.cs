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
		private readonly Func<TItem, bool> _ProcessItemFunc;
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
			ILogger logger, Func<TItem, bool> processItemFunc)
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

			runningTaks.CountSetting.Changed += (newCount, oldCold) => ProcessQueueAsync(CancellationToken.None);
			itemQueue.QueueSize.Changed += (newQueueSize, oldQueueSize) => ProcessQueueAsync(CancellationToken.None);
			itemQueue.HeldQueueSize.Changed += (newQueueSize, oldQueueSize) => ProcessQueueAsync(CancellationToken.None);
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

			ProcessQueueAsync(cancellationToken);
			
			return task;
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

		private void ProcessItem(QueueItem<TItem> queueItem)
		{
			var holderId = queueItem.HolderId;

			try
			{
				var processed = _ProcessItemFunc(queueItem.Value);
				if (processed)
				{
					_ItemQueue.RemoveQueueItem(queueItem.Id, holderId);
					return;
				}

				Thread.Sleep(_ItemQueueProcessorSettings.ItemRetryDelay);
				_ItemQueue.ReleaseQueueItem(queueItem.Id, holderId);
			}
			catch (Exception e)
			{
				OnUnhandledException?.Invoke(queueItem.Value, e);
				Thread.Sleep(_ItemQueueProcessorSettings.ItemLockDuration);
			}
		}

		private Task ProcessQueueAsync(CancellationToken cancellationToken)
		{
			var runningTask = _RunningTask.Value;
			if (runningTask == null)
			{
				return Task.CompletedTask;
			}

			if (_ProcessQueueLock.CurrentCount == 0)
			{
				return Task.CompletedTask;
			}

			return Task.Run(async () =>
			{
				await _ProcessQueueLock.WaitAsync(cancellationToken).ConfigureAwait(false);

				try
				{
					for (var threadNumber = _RunningThreads.Count; threadNumber < _ItemQueueProcessorSettings.NumberOfThreads; threadNumber++)
					{
						Task runTask;

						if (_ItemQueue.TryGetNextQueueItem(_ItemQueueProcessorSettings.ItemLockDuration, out var queueItem))
						{
							runTask = Task.Run(() =>
							{
								ProcessItem(queueItem);
							}, cancellationToken);
						}
						else
						{
							runTask = Task.Delay(_ItemQueueProcessorSettings.ThreadSleepTime, cancellationToken);
						}

						_RunningThreads.Add(runTask);
						RemoveOnCompletion(runTask, cancellationToken);
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
			}, cancellationToken);
		}

		private void RemoveOnCompletion(Task runTask, CancellationToken cancellationToken)
		{
			Task.Run(async () =>
			{
				try
				{
					await runTask.ConfigureAwait(false);
				}
				catch (Exception e)
				{
					_Logger.Error(e);
				}
				finally
				{
					_RunningThreads.Remove(runTask);
				}
			}, cancellationToken);
		}
	}
}
