using RabbitMQ.Client;

namespace TixFactory.Queueing;

/// <summary>
/// Configuration that applies to all things.
/// </summary>
public class GlobalConfiguration
{
    /// <summary>
    /// The host name to find RabbitMQ under.
    /// </summary>
    /// <seealso cref="ConnectionFactory.HostName"/>
    public string HostName { get; set; } = "rabbitmq";

    /// <summary>
    /// Whether or not Rabbit should process received messages asynchronously.
    /// </summary>
    /// <seealso cref="ConnectionFactory.DispatchConsumersAsync"/>
    public bool AsyncEnabled { get; set; } = false;
}
