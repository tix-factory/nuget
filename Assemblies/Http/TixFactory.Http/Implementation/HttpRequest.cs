using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace TixFactory.Http
{
    /// <inheritdoc cref="IHttpRequest"/>
    public class HttpRequest : IHttpRequest
    {
        /// <inheritdoc cref="IHttpRequest.Method"/>
        public HttpMethod Method { get; set; }

        /// <inheritdoc cref="IHttpRequest.Url"/>
        public Uri Url { get; set; }

        /// <inheritdoc cref="IHttpRequest.Headers"/>
        public IHttpRequestHeaders Headers { get; set; }

        /// <inheritdoc cref="IHttpRequest.Body"/>
        /// <remarks>
        /// Excluded from code coverage because it doesn't make sense to test getter, and setter.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        public HttpContent Body { get; set; }

        /// <summary>
        /// Initializes a new <see cref="HttpRequest"/>
        /// </summary>
        /// <param name="method">The <see cref="Method"/></param>
        /// <param name="url">The <see cref="Url"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
        public HttpRequest(HttpMethod method, Uri url)
        {
            Method = method;
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Headers = new HttpRequestHeaders();
        }
    }
}
