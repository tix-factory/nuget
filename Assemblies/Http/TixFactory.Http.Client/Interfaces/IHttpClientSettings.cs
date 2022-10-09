using System;

namespace TixFactory.Http.Client
{
    /// <summary>
    /// Configuration properties used for sending requests.
    /// </summary>
    public interface IHttpClientSettings
    {
        /// <summary>
        /// The default User-Agent request header for all requests.
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        /// The maximum time before the request is terminated.
        /// </summary>
        TimeSpan RequestTimeout { get; }

        /// <summary>
        /// The max amount of times the request can be redirected before it is terminated.
        /// </summary>
        int MaxRedirects { get; }
    }
}
