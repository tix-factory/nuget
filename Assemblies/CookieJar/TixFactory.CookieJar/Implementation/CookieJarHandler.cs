using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.CookieJar
{
    /// <summary>
    /// An <see cref="HttpClientHandler"/> which saves cookies after a request completes.
    /// </summary>
    /// <remarks>
    /// This handler is intended to be used in place of the <see cref="HttpClientHandler"/>,
    /// passed to the constructor of the <see cref="HttpClient"/>.
    /// </remarks>
    public class CookieJarHandler : HttpClientHandler
    {
        private const string _SetCookieHeader = "Set-Cookie";
        private readonly ICookieJar _CookieJar;

        /// <summary>
        /// Initializes a new <see cref="CookieJarHandler"/>.
        /// </summary>
        /// <param name="cookieJar">The <see cref="ICookieJar"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="cookieJar"/>
        /// </exception>
        public CookieJarHandler(ICookieJar cookieJar)
        {
            _CookieJar = cookieJar ?? throw new ArgumentNullException(nameof(cookieJar));
            CookieContainer = cookieJar.CookieContainer;
        }

        /// <inheritdoc cref="HttpClientHandler.SendAsync"/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.Headers.Contains(_SetCookieHeader))
            {
                _CookieJar.Save();
            }

            return response;
        }
    }
}
