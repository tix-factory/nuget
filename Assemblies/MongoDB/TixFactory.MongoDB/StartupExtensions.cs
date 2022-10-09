using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TixFactory.MongoDB;

/// <summary>
/// Extension methods for initializing MongoDB entities at service startup.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds an <see cref="IMongoCollection{TDocument}"/> to an <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This extension method will create the collection if it does not already exist.
    /// </remarks>
    /// <typeparam name="TDocument">The type of the entity model for the collection.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="services"/>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// - <typeparamref name="TDocument"/> is missing <see cref="DataContractAttribute"/>, or <see cref="DataContractAttribute"/> does not have required properties set.
    /// </exception>
    public static void AddMongoCollection<TDocument>(this IServiceCollection services)
        where TDocument : class
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var dataContract = typeof(TDocument).GetCustomAttribute(typeof(DataContractAttribute)) as DataContractAttribute;
        if (string.IsNullOrWhiteSpace(dataContract?.Name) || string.IsNullOrWhiteSpace(dataContract.Namespace))
        {
            throw new ArgumentException($"[DataContract] attribute expected with {nameof(DataContractAttribute.Name)} set to the collection name, and {nameof(DataContractAttribute.Namespace)} set to the database name.", nameof(TDocument));
        }

        services.TryAddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration.GetValue<string>("MONGODB_CONNECTION_STRING"));
        });

        services.TryAddSingleton(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            var database = mongoClient.GetDatabase(dataContract.Namespace);
            return GetOrCreateCollection<TDocument>(database, dataContract.Name);
        });
    }

    private static IMongoCollection<TDocument> GetOrCreateCollection<TDocument>(IMongoDatabase database, string collectionName)
    {
        var collectionExists = CollectionExists(database, collectionName);
        if (!collectionExists)
        {
            database.CreateCollection(collectionName);
        }

        return database.GetCollection<TDocument>(collectionName);
    }

    private static bool CollectionExists(IMongoDatabase database, string collectionName)
    {
        // https://stackoverflow.com/a/51305539/1663648
        var filter = new BsonDocument("name", collectionName);
        var options = new ListCollectionNamesOptions { Filter = filter };
        return database.ListCollectionNames(options).Any();
    }
}
