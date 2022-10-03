using System;

namespace TixFactory.Processors.Queueing
{
    /// <summary>
    /// Default implementation of <see cref="IQueueProcessorSettings"/>.
    /// </summary>
    public class QueueProcessorSettings : IQueueProcessorSettings
    {
        /// <inheritdoc cref="IQueueProcessorSettings.NumberOfThreads"/>
        public int NumberOfThreads { get; set; } = 1;

        /// <inheritdoc cref="IQueueProcessorSettings.ItemLockDuration"/>
        public TimeSpan ItemLockDuration { get; set; } = TimeSpan.FromMinutes(5);

        /// <inheritdoc cref="IQueueProcessorSettings.ThreadSleepTime"/>
        public TimeSpan ThreadSleepTime { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <inheritdoc cref="IQueueProcessorSettings.ItemRetryDelay"/>
        public TimeSpan ItemRetryDelay { get; set; } = TimeSpan.FromMinutes(1);
    }
}
