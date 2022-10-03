using System;
using System.IO;
using System.Net;
using System.Text;

namespace TixFactory.Http
{
    /// <summary>
    /// An object representing HTTP response information.
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// The <see cref="HttpStatusCode"/>.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The status description.
        /// </summary>
        string StatusText { get; }

        /// <summary>
        /// Whether or not the response had a successful status code.
        /// </summary>
        /// <remarks>
        /// Successful status codes are considered 2xx level status codes.
        /// </remarks>
        bool IsSuccessful { get; }

        /// <summary>
        /// The final <see cref="Uri"/> from the request.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// The <see cref="IHttpResponseHeaders"/>.
        /// </summary>
        IHttpResponseHeaders Headers { get; }

        /// <summary>
        /// The response body.
        /// </summary>
        Stream Body { get; }

        /// <summary>
        /// Gets <see cref="Body"/> as a string.
        /// </summary>
        /// <param name="encoding">The encoding (defaults to <see cref="Encoding.UTF8"/>.)</param>
        /// <returns>The string response body.</returns>
        string GetStringBody(Encoding encoding = null);
    }
}
