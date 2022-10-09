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
}
