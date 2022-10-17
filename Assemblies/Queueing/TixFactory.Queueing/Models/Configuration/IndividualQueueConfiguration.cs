using System;

namespace TixFactory.Queueing;

/// <summary>
/// Configuration for the <see cref="RabbitConsumer{TMessage}"/>.
/// </summary>
internal class IndividualQueueConfiguration
{
    /// <summary>
    /// How long to wait before retrying queued messages when they fail explicitly to be retried.
    /// </summary>
    public TimeSpan RetryDealy { get; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// The number of messages that may be processed in parallel.
    /// </summary>
    public ushort NumberOfThreads { get; set; } = 100;
}
