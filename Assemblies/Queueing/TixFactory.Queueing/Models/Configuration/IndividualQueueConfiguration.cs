namespace TixFactory.Queueing;

/// <summary>
/// Configuration for the <see cref="RabbitConsumer{TMessage}"/>.
/// </summary>
internal class IndividualQueueConfiguration
{
    /// <summary>
    /// The number of messages that may be processed in parallel.
    /// </summary>
    public ushort NumberOfThreads { get; set; } = 10;
}
