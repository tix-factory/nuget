using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prometheus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TixFactory.ApplicationContext;

namespace TixFactory.Queueing;

/// <summary>
/// A base class for a RabbitMQ consumer <see cref="IHostedService"/>.
/// </summary>
/// <typeparam name="TMessage">The type of message expected in the Rabbit queue.</typeparam>
public abstract class RabbitConsumer<TMessage> : IHostedService
{
    private readonly IModel _RabbitConnection;
    private readonly IApplicationContext _ApplicationContext;
    private readonly EventingBasicConsumer _QueueConsumer;
    private readonly IndividualQueueConfiguration _Configuration;
    private readonly Gauge.Child _InProgressGauge;
    private readonly Histogram.Child _ProcessingHistogram;
    private readonly Counter _ResultCounter;

    /// <summary>
    /// The name of the queue this consumer is for.
    /// </summary>
    public string QueueName { get; }

    /// <summary>
    /// The <see cref="ILogger"/>.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new <see cref="RabbitConsumer{TMessage}"/>.
    /// </summary>
    /// <param name="rabbitConnection">The Rabbit connection.</param>
    /// <param name="logger">The <see cref="Logger"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="rabbitConnection"/>
    /// - <paramref name="logger"/>
    /// - <paramref name="configuration"/>
    /// </exception>
    protected RabbitConsumer(IModel rabbitConnection, ILogger logger, IConfiguration configuration)
        : this(rabbitConnection, ApplicationContext.ApplicationContext.Singleton, logger, configuration)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="RabbitConsumer{TMessage}"/>.
    /// </summary>
    /// <param name="rabbitConnection">The Rabbit connection.</param>
    /// <param name="applicationContext">The <see cref="IApplicationContext"/>.</param>
    /// <param name="logger">The <see cref="Logger"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="rabbitConnection"/>
    /// - <paramref name="applicationContext"/>
    /// - <paramref name="logger"/>
    /// - <paramref name="configuration"/>
    /// </exception>
    protected RabbitConsumer(IModel rabbitConnection, IApplicationContext applicationContext, ILogger logger, IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var queueName = QueueName = RabbitExtensions.GetQueueName<TMessage>();

        _RabbitConnection = rabbitConnection ?? throw new ArgumentNullException(nameof(rabbitConnection));
        _ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _QueueConsumer = new EventingBasicConsumer(rabbitConnection);
        _Configuration = LoadConfiguration(configuration, queueName);

        _InProgressGauge = Metrics.CreateGauge(
            name: "rabbit_consumer_in_progress",
            help: "How many messages are actively being processed.",
            labelNames: new[] { "queue_name" })
            .WithLabels(QueueName);

        _ResultCounter = Metrics.CreateCounter(
            name: "rabbit_consumer_messages_processed",
            help: "Number of messages processed, and their result.",
            labelNames: new[] { "queue_name", "result" });

        _ProcessingHistogram = Metrics.CreateHistogram(
            name: "rabbit_consumer_messages_processing",
            help: "Messages being processed, and their processing times.",
            labelNames: new[] { "queue_name" })
            .WithLabels(QueueName);
    }

    /// <inheritdoc cref="IHostedService.StartAsync"/>
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        _QueueConsumer.Received += HandleQueueItem;

        _RabbitConnection.BasicQos(prefetchSize: 0, prefetchCount: _Configuration.NumberOfThreads, global: false);

        _RabbitConnection.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        _RabbitConnection.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: _QueueConsumer,
            consumerTag: _ApplicationContext.Name);

        Logger.LogInformation($"Listening to queue... {QueueName}\n\tNumber of Threads: {_Configuration.NumberOfThreads} (process thread count: {ThreadPool.ThreadCount})");

        return Task.CompletedTask;
    }

    /// <inheritdoc cref="IHostedService.StopAsync"/>
    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        _QueueConsumer.Received -= HandleQueueItem;

        // TODO: Is there some way to undo BasicConsume?

        return Task.CompletedTask;
    }

    /// <summary>
    /// Processes a message from the Rabbit queue.
    /// </summary>
    /// <param name="message">The message itself.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The result of what happened while processing the message.</returns>
    public abstract Task<MessageProcessingResult> ProcessMessageAsync(TMessage message, CancellationToken cancellationToken);

    private void HandleQueueItem(object sender, BasicDeliverEventArgs message)
    {
        ThreadPool.QueueUserWorkItem(async (_) =>
        {
            // Need to process the message in a background thread, to ensure the 
            await HandleQueueItemAsync(sender, message);
        });
    }

    private async Task HandleQueueItemAsync(object sender, BasicDeliverEventArgs message)
    {
        using var timer = _ProcessingHistogram.NewTimer();
        _InProgressGauge.Inc();

        try
        {
            var messageBody = Encoding.UTF8.GetString(message.Body.ToArray());
            var (processingResult, exception) = await HandleQueueItemAsync(sender, messageBody, CancellationToken.None);
            _ResultCounter.WithLabels(QueueName, processingResult.ToString()).Inc();

            switch (processingResult)
            {
                case MessageProcessingResult.Retry:
                case MessageProcessingResult.UnhandledException:
                    if (exception != null)
                    {
                        Logger.LogError(exception, $"An unhandled exception happened while processing message. The message will be retried.\n\tMessage: {messageBody}");
                    }

                    _RabbitConnection.BasicNack(message.DeliveryTag, multiple: false, requeue: true);

                    return;
                case MessageProcessingResult.Success:
                    _RabbitConnection.BasicAck(message.DeliveryTag, multiple: false);
                    return;
                case MessageProcessingResult.BadMessage:
                    if (_Configuration.DisposeBadMessages)
                    {
                        if (_Configuration.LogBadMessages)
                        {
                            Logger.LogWarning(exception, $"Failed to parse message from queue: {QueueName} (the message will be removed)\n\tMessage: {messageBody}");
                        }

                        _RabbitConnection.BasicAck(message.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        if (_Configuration.LogBadMessages)
                        {
                            Logger.LogWarning(exception, $"Failed to parse message from queue: {QueueName} (message will remain in queue)\n\tMessage: {messageBody}");
                        }

                        _RabbitConnection.BasicNack(message.DeliveryTag, multiple: false, requeue: true);
                    }

                    return;
                default:
                    Logger.LogError($"The processing result is invalid ({processingResult}). This message will be back after the timeout.");
                    return;
            }
        }
        catch (Exception e)
        {
            // Uh oh... the message _may_ come back after it times out.
            Logger.LogError(e, $"Unhandled exception processing queue: {QueueName})");
            _ResultCounter.WithLabels(QueueName, nameof(MessageProcessingResult.UnhandledException)).Inc();
        }
        finally
        {
            _InProgressGauge.Dec();
        }
    }

    private async Task<(MessageProcessingResult, Exception)> HandleQueueItemAsync(object _, string message, CancellationToken cancellationToken)
    {
        TMessage parsedMessage;

        try
        {
            parsedMessage = JsonConvert.DeserializeObject<TMessage>(message);
        }
        catch (Exception e)
        {
            return (MessageProcessingResult.BadMessage, e);
        }

        try
        {
            var result = await ProcessMessageAsync(parsedMessage, cancellationToken);
            return (result, null);
        }
        catch (Exception e)
        {
            return (MessageProcessingResult.UnhandledException, e);
        }
    }

    private static IndividualQueueConfiguration LoadConfiguration(IConfiguration configuration, string queueName)
    {
        var settings = new IndividualQueueConfiguration();
        var rabbitSection = configuration.GetSection("Rabbit");
        var queuesSection = rabbitSection.GetSection("Queues");
        var queueSection = queuesSection.GetSection(queueName);
        queueSection.Bind(settings);

        return settings;
    }
}
