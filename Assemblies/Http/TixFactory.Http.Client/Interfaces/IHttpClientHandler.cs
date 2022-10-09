using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Http.Client
{
    /// <summary>
    /// Handlers used in the execution of an outbound HTTP request.
    /// </summary>
    public interface IHttpClientHandler
    {
        /// <summary>
        /// Synchronous invocation of the handler.
        /// </summary>
        /// <param name="request">The <see cref="IHttpRequest"/>.</param>
        /// <returns>The <see cref="IHttpResponse"/>.</returns>
        IHttpResponse Invoke(IHttpRequest request);

        /// <summary>
        /// Asynchronous invocation of the handler.
        /// </summary>
        /// <param name="request">The <see cref="IHttpRequest"/>.</param>
        /// <param name="cancellationToken">The request <see cref="CancellationToken"/>.</param>
        /// <returns>The task returning the <see cref="IHttpResponse"/>.</returns>
        Task<IHttpResponse> InvokeAsync(IHttpRequest request, CancellationToken cancellationToken);
    }
}
