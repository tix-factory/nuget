using System;

namespace TixFactory.Queueing
{
    /// <summary>
    /// Default implementation of <see cref="IItemQueueProcessorSettings"/>.
    /// </summary>
    public class ItemQueueProcessorSettings : IItemQueueProcessorSettings
    {
        /// <inheritdoc cref="IItemQueueProcessorSettings.NumberOfThreads"/>
        public int NumberOfThreads { get; set; } = 1;

        /// <inheritdoc cref="IItemQueueProcessorSettings.ItemLockDuration"/>
        public TimeSpan ItemLockDuration { get; set; } = TimeSpan.FromMinutes(5);

        /// <inheritdoc cref="IItemQueueProcessorSettings.ThreadSleepTime"/>
        public TimeSpan ThreadSleepTime { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <inheritdoc cref="IItemQueueProcessorSettings.ItemRetryDelay"/>
        public TimeSpan ItemRetryDelay { get; set; } = TimeSpan.FromMinutes(1);
    }
}
