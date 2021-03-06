﻿using System;

namespace TixFactory.Processors.Queueing
{
	/// <summary>
	/// Settings required for processing queue items.
	/// </summary>
	/// <seealso cref="IQueueProcessor"/>
	public interface IQueueProcessorSettings
	{
		/// <summary>
		/// The number of threads available to process queue items.
		/// </summary>
		int NumberOfThreads { get; }

		/// <summary>
		/// How long to hold an item from the queue while it is being processed.
		/// </summary>
		/// <remarks>
		/// If the lock times out the item will be given to the next available thread.
		/// The lock duration is also taken into effect for items that fail to process with an unhandled exception.
		/// </remarks>
		TimeSpan ItemLockDuration { get; }

		/// <summary>
		/// How long a thread should sleep if there is no item to give it.
		/// </summary>
		TimeSpan ThreadSleepTime { get; }

		/// <summary>
		/// How long to wait before releasing a message that gracefully failed to process.
		/// </summary>
		TimeSpan ItemRetryDelay { get; }
	}
}
