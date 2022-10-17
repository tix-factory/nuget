namespace TixFactory.Queueing;

/// <summary>
/// Message processing results.
/// </summary>
public enum MessageProcessingResult
{
    /// <summary>
    /// Invalid.
    /// </summary>
    /// <remarks>
    /// Exists to catch the <c>default</c>.
    /// </remarks>
    Invalid = 0,

    /// <summary>
    /// The message was processed successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The message should be retried.
    /// </summary>
    Retry = 2,

    /// <summary>
    /// The message data is invalid, and cannot be processed.
    /// </summary>
    BadMessage = 3,

    /// <summary>
    /// An unhandled exception occurred while processing the message.
    /// </summary>
    /// <remarks>
    /// Acts as a retry, but should not be returned directly.
    /// </remarks>
    UnhandledException = 4,
}
