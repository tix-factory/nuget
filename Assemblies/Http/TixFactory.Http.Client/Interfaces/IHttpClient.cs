using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Http.Client
{
    /// <summary>
    /// A client for handling Http requests
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// The handlers to execute for the requests.
        /// </summary>
        /// <remarks>
        /// The first handler in the list is executed when <see cref="Send"/>
        /// and <see cref="SendAsync"/> are called.
        /// The first handler is expected to call the next handlers in this collection.
        /// 
        /// The handlers should not be tampered with after the first request is sent.
        /// All handlers that inherit from <see cref="HttpClientHandlerBase"/> will have their <see cref="HttpClientHandlerBase.NextHandler"/> set to the next handler in the list.
        /// </remarks>
        IList<IHttpClientHandler> Handlers { get; }

        /// <summary>
        /// Sends an <see cref="IHttpRequest"/> synchronously and returns the <see cref="IHttpResponse"/>.
        /// </summary>
        /// <param name="request">The <see cref="IHttpRequest"/>.</param>
        /// <returns>An <see cref="IHttpResponse"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/></exception>
        /// <exception cref="HttpException">Something went wrong with the request. See <see cref="Exception.InnerException"/>.</exception>
        IHttpResponse Send(IHttpRequest request);

        /// <summary>
        /// Sends an <see cref="IHttpRequest"/> asynchronously and returns the <see cref="IHttpResponse"/>.
        /// </summary>
        /// <param name="request">The <see cref="IHttpRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>An <see cref="IHttpResponse"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/></exception>
        /// <exception cref="HttpException">Something went wrong with the request. See <see cref="Exception.InnerException"/>.</exception>
        Task<IHttpResponse> SendAsync(IHttpRequest request, CancellationToken cancellationToken);
    }
}
