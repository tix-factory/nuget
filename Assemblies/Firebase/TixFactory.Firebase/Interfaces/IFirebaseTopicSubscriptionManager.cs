using System;
using System.Collections.Generic;

namespace TixFactory.Firebase
{
    /// <summary>
    /// Manages subscriptions to firebase topics.
    /// </summary>
    public interface IFirebaseTopicSubscriptionManager
    {
        /// <summary>
        /// Subscribes a token to a topic.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="topic">The topic.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="token"/> or <paramref name="topic"/> is null or whitespace.
        /// </exception>
        /// <exception cref="FirebaseException">The firebase request fails.</exception>
        void Subscribe(string token, string topic);

        /// <summary>
        /// Unsubscribes a token to a topic.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="topic">The topic.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="token"/> or <paramref name="topic"/> is null or whitespace.
        /// </exception>
        /// <exception cref="FirebaseException">The firebase request fails.</exception>
        void Unsubscribe(string token, string topic);

        /// <summary>
        /// Gets topics a token is subscribed to.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <exception cref="ArgumentException"><paramref name="token"/> is null or whitespace.</exception>
        /// <exception cref="FirebaseException">The firebase request fails.</exception>
        /// <returns>The topics.</returns>
        ICollection<string> GetSubscribedTopics(string token);
    }
}
