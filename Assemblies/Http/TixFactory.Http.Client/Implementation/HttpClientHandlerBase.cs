using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Http.Client
{
    /// <summary>
    /// A base class that can be used for <see cref="IHttpClientHandler"/>s that keeps track of the next handler to invoke.
    /// </summary>
    public abstract class HttpClientHandlerBase : IHttpClientHandler
    {
        /// <summary>
        /// The next handler to invoke.
        /// </summary>
        public IHttpClientHandler NextHandler { get; set; }

        /// <summary>
        /// Invokes the next handler.
        /// </summary>
        /// <param name="request">The <see cref="IHttpRequest"/>.</param>
        /// <returns>The <see cref="IHttpResponse"/>.</returns>
        public virtual IHttpResponse Invoke(IHttpRequest request)
        {
            return NextHandler.Invoke(request);
        }

        /// <summary>
        /// Invokes the next handler asynchronously.
        /// </summary>
        /// <param name="request">The <see cref="IHttpRequest"/>.</param>
        /// <param name="cancellationToken">The request <see cref="CancellationToken"/>.</param>
        /// <returns>The task which will respond with the <see cref="IHttpResponse"/>.</returns>
        public virtual Task<IHttpResponse> InvokeAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            return NextHandler.InvokeAsync(request, cancellationToken);
        }
    }
}
