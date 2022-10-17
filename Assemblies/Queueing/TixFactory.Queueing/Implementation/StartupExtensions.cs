using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;

namespace TixFactory.Queueing;

/// <summary>
/// Helper extensions for initializing RabbitMQ at startup.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds RabbitMQ instances to an <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="serviceCollection"/>
    /// </exception>
    public static void AddRabbit(this IServiceCollection serviceCollection)
    {
        if (serviceCollection == null)
        {
            throw new ArgumentNullException(nameof(serviceCollection));
        }

        // Same connection factory for everything.
        serviceCollection.TryAddSingleton(CreateAsyncConnectionFactory);

        // Use transient for IModel so that we can configure the number of threads per-queue.
        serviceCollection.AddTransient(CreateRabbitConnection);
    }

    private static IModel CreateRabbitConnection(IServiceProvider serviceProvider)
    {
        var connectionFactory = serviceProvider.GetRequiredService<IAsyncConnectionFactory>();
        var connection = connectionFactory.CreateConnection();
        return connection.CreateModel();
    }

    private static IAsyncConnectionFactory CreateAsyncConnectionFactory(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var globalConfiguration = GetRabbitConfiguration(configuration);

        return new ConnectionFactory
        {
            HostName = globalConfiguration.HostName,
            DispatchConsumersAsync = globalConfiguration.AsyncEnabled
        };
    }

    private static GlobalConfiguration GetRabbitConfiguration(IConfiguration configuration)
    {
        var settings = new GlobalConfiguration();

        var rabbitConfiguration = configuration.GetSection("Rabbit");
        var globalConfiguration = rabbitConfiguration.GetSection("Settings");
        globalConfiguration.Bind(settings);

        return settings;
    }
}
