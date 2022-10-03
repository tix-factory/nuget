using System;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.Http;
using TixFactory.Http.Client;

namespace TixFactory.Firebase
{
    /// <summary>
    /// An <see cref="IHttpClientHandler"/> to add the Authorization header to fcm requests.
    /// </summary>
    public class FirebaseAuthorizationHandler : HttpClientHandlerBase
    {
        private readonly string _ServerKey;

        /// <summary>
        /// Initializes a new <see cref="FirebaseAuthorizationHandler"/>.
        /// </summary>
        /// <param name="serverKey">The firebase server key.</param>
        /// <exception cref="ArgumentException"><paramref name="serverKey"/> is null or whitespace.</exception>
        public FirebaseAuthorizationHandler(string serverKey)
        {
            if (string.IsNullOrWhiteSpace(serverKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serverKey));
            }

            _ServerKey = serverKey;
        }

        /// <inheritdoc cref="HttpClientHandlerBase.Invoke"/>
        public override IHttpResponse Invoke(IHttpRequest request)
        {
            AddAuthorization(request);
            return base.Invoke(request);
        }

        /// <inheritdoc cref="HttpClientHandlerBase.InvokeAsync"/>
        public override Task<IHttpResponse> InvokeAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            AddAuthorization(request);
            return base.InvokeAsync(request, cancellationToken);
        }

        private void AddAuthorization(IHttpRequest request)
        {
            if (request == null)
            {
                return;
            }

            if (request.Url.Host == FirebaseDomain.Fcm || request.Url.Host == FirebaseDomain.Iid)
            {
                request.Headers.AddOrUpdate("Authorization", $"key={_ServerKey}");
            }
        }
    }
}
