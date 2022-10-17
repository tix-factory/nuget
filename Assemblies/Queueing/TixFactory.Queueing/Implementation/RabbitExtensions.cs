using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace TixFactory.Queueing;

/// <summary>
/// Extension methods for RabbitMQ.
/// </summary>
public static class RabbitExtensions
{
    /// <summary>
    /// Writes a message into a queue.
    /// </summary>
    /// <typeparam name="TMessage">The queue message.</typeparam>
    /// <param name="rabbitConnection">A RabbitMQ connection.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="rabbitConnection"/>
    /// - <paramref name="message"/>
    /// </exception>
    public static void WriteQueueMessage<TMessage>(this IModel rabbitConnection, TMessage message)
    {
        if (rabbitConnection == null)
        {
            throw new ArgumentNullException(nameof(rabbitConnection));
        }

        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var queueName = GetQueueName<TMessage>();
        var messageProperties = rabbitConnection.CreateBasicProperties();
        messageProperties.Persistent = true;

        var body = JsonConvert.SerializeObject(message);

        rabbitConnection.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            body: Encoding.UTF8.GetBytes(body),
            basicProperties: messageProperties);
    }

    internal static string GetQueueName<TMessage>()
    {
        var dataContract = typeof(TMessage).GetCustomAttribute<DataContractAttribute>();
        if (string.IsNullOrWhiteSpace(dataContract?.Name))
        {
            throw new ArgumentException($"{nameof(DataContractAttribute)} with {nameof(DataContractAttribute.Name)} set, to identify queue the message is for.", nameof(TMessage));
        }

        return dataContract.Name;
    }
}
