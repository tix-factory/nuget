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

    /// <summary>
    /// Whether or not to ack messages that could not be parsed.
    /// </summary>
    /// <remarks>
    /// If a message in the rabbit queue cannot be parsed as the expected JSON object
    /// and this is set to <c>true</c>, the message will be acked, removing it from the queue.
    ///
    /// When <c>false</c> the message will remain in the queue.
    /// </remarks>
    public bool DisposeBadMessages { get; set; } = false;

    /// <summary>
    /// Whether or not to log a warning whenever a bad message is detected in the queue.
    /// </summary>
    public bool LogBadMessages { get; set; } = false;
}
