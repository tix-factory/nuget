using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TixFactory.MongoDB;

/// <summary>
/// Extension methods for <see cref="IMongoCollection{TDocument}"/>
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// A generic comment to use for write operations.
    /// </summary>
    public static readonly string WriteComment = $"Written by {ApplicationContext.ApplicationContext.Singleton.Name}";

    /// <summary>
    /// An extension method to find the first one document matching the expression.
    /// </summary>
    /// <typeparam name="TDocument">The document type.</typeparam>
    /// <param name="collection">The <see cref="IMongoCollection{TDocument}"/>.</param>
    /// <param name="filter">The filter for finding the document.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The document, or <c>null</c> if the document does not exist.</returns>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="collection"/>
    /// - <paramref name="filter"/>
    /// </exception>
    public static async Task<TDocument> FindOneAsync<TDocument>(this IMongoCollection<TDocument> collection, Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        var records = await collection.FindAsync(filter, new FindOptions<TDocument>
        {
            Limit = 1
        }, cancellationToken);

        return records.FirstOrDefault(cancellationToken);
    }

    /// <summary>
    /// Updates an individual document in an <see cref="IMongoCollection{TDocument}"/>.
    /// </summary>
    /// <typeparam name="TDocument">The document type.</typeparam>
    /// <param name="collection">The <see cref="IMongoCollection{TDocument}"/>.</param>
    /// <param name="filter">The filter for finding the document to update.</param>
    /// <param name="updateDocument">A method used to update the entity. Return <c>true</c> to push the update, otherwise <c>false</c> to skip the update.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns><c>true</c> if a document was found to be updated, otherwise <c>false</c></returns>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="collection"/>
    /// - <paramref name="filter"/>
    /// - <paramref name="updateDocument"/>
    /// </exception>
    public static async Task<bool> UpdateOneAsync<TDocument>(this IMongoCollection<TDocument> collection, Expression<Func<TDocument, bool>> filter, Func<TDocument, bool> updateDocument, CancellationToken cancellationToken)
    {
        if (updateDocument == null)
        {
            throw new ArgumentNullException(nameof(updateDocument));
        }

        var document = await collection.FindOneAsync(filter, cancellationToken);
        if (document == null)
        {
            return false;
        }

        if (updateDocument(document))
        {
            var result = await collection.UpdateOneAsync(filter, new ObjectUpdateDefinition<TDocument>(document), new UpdateOptions
            {
                Comment = WriteComment
            }, cancellationToken);

            if (result != null)
            {
                throw new MongoException($"Expected exactly one document to be updated, but got {result.ModifiedCount}");
            }
        }

        return true;
    }
}
