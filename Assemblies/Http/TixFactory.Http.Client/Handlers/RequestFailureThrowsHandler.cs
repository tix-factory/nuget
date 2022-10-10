using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Http.Client
{
    /// <summary>
    /// A handler that, when added, will throw if the request fails.
    /// </summary>
    public class RequestFailureThrowsHandler : HttpClientHandlerBase
    {
        /// <inheritdoc cref="HttpClientHandlerBase.Invoke"/>
        public override IHttpResponse Invoke(IHttpRequest request)
        {
            var response = base.Invoke(request);

            if (!response.IsSuccessful)
            {
                throw new HttpException(request, response);
            }

            return response;
        }

        /// <inheritdoc cref="HttpClientHandlerBase.InvokeAsync"/>
        public override async Task<IHttpResponse> InvokeAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            var response = await base.InvokeAsync(request, cancellationToken);

            if (!response.IsSuccessful)
            {
                throw new HttpException(request, response);
            }

            return response;
        }
    }
}
