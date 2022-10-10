using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Http.Client
{
    /// <summary>
    /// The handler response for executing the HTTP request.
    /// </summary>
    /// <remarks>
    /// Will not fire any handler after this one.
    /// </remarks>
    public class SendHttpRequestHandler : IHttpClientHandler
    {
        private const string _UnexpectedErrorMessage = "An unexpected error occurred while processing the Http request. Check inner exception.";

        private readonly IHttpClientSettings _HttpClientSettings;
        private readonly CookieContainer _CookieContainer;
        private HttpClientHandler _HttpClientHandler;
        private System.Net.Http.HttpClient _HttpClient;

        /// <summary>
        /// Initializes a new <see cref="SendHttpRequestHandler"/>.
        /// </summary>
        /// <param name="cookieContainer">The <see cref="CookieContainer"/> to use in the requests (initializes a new one when <c>null</c>.)</param>
        /// <param name="httpClientSettings">An <see cref="IHttpClientSettings"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="httpClientSettings"/>
        /// </exception>
        public SendHttpRequestHandler(CookieContainer cookieContainer, IHttpClientSettings httpClientSettings)
        {
            if (cookieContainer == null)
            {
                cookieContainer = new CookieContainer();
            }

            _HttpClientSettings = httpClientSettings ?? throw new ArgumentNullException(nameof(httpClientSettings));
            _CookieContainer = cookieContainer;
        }

        /// <inheritdoc cref="IHttpClientHandler.Invoke"/>
        public IHttpResponse Invoke(IHttpRequest request)
        {
#if NET6_0_OR_GREATER
            var cancellationToken = GetRequestCancellationToken(CancellationToken.None);

            try
            {
                var requestMessage = BuildHttpRequestMessage(request);
                var responseMessage = Send(_HttpClient, requestMessage, cancellationToken);
                return BuildHttpResponse(request, responseMessage);
            }
            catch (TaskCanceledException ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    throw new HttpTimeoutException("The request timed out.", ex);
                }

                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(_UnexpectedErrorMessage, ex);
            }
#else
            return InvokeAsync(request, CancellationToken.None).GetAwaiter().GetResult();
#endif
        }

        /// <inheritdoc cref="IHttpClientHandler.InvokeAsync"/>
        public async Task<IHttpResponse> InvokeAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            var requestCancellationToken = GetRequestCancellationToken(cancellationToken);

            try
            {
                var requestMessage = BuildHttpRequestMessage(request);
                var responseMessage = await SendAsync(_HttpClient, requestMessage, requestCancellationToken);
                return await BuildHttpResponseAsync(request, responseMessage, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (!requestCancellationToken.IsCancellationRequested)
                {
                    throw new HttpTimeoutException("The request timed out.", ex);
                }

                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(_UnexpectedErrorMessage, ex);
            }
        }

        internal HttpRequestMessage BuildHttpRequestMessage(IHttpRequest request)
        {
            var httpMethod = new System.Net.Http.HttpMethod(request.Method.ToString().ToUpperInvariant());
            var requestMessage = new HttpRequestMessage(httpMethod, request.Url);

            if (request.Body != null)
            {
                requestMessage.Content = request.Body;

                var contentType = request.Headers.Get(HttpRequestHeaderName.ContentType).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
            }

            foreach (var name in request.Headers.Keys)
            {
                switch (name)
                {
                    case HttpRequestHeaderName.ContentType:
                        continue;
                    default:
                        requestMessage.Headers.Add(name, request.Headers.Get(name));
                        break;
                }
            }

            if (!request.Headers.Keys.Contains(HttpRequestHeaderName.UserAgent) && !string.IsNullOrWhiteSpace(_HttpClientSettings.UserAgent))
            {
                requestMessage.Headers.Add(HttpRequestHeaderName.UserAgent, _HttpClientSettings.UserAgent);
            }

            return requestMessage;
        }

#if NET6_0_OR_GREATER
        internal static IHttpResponse BuildHttpResponse(IHttpRequest httpRequest, HttpResponseMessage responseMessage)
        {
            var httpResponse = CreateHttpResponse(httpRequest, responseMessage);
            httpResponse.Body = responseMessage.Content.ReadAsStream();
            return httpResponse;
        }
#endif

        internal static async Task<IHttpResponse> BuildHttpResponseAsync(IHttpRequest httpRequest, HttpResponseMessage responseMessage, CancellationToken cancellationToken)
        {
            var httpResponse = CreateHttpResponse(httpRequest, responseMessage);
#if NET6_0_OR_GREATER
            httpResponse.Body = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
#else
            httpResponse.Body = await responseMessage.Content.ReadAsStreamAsync();
#endif
            return httpResponse;
        }

        private static HttpResponse CreateHttpResponse(IHttpRequest httpRequest, HttpResponseMessage responseMessage)
        {
            return new HttpResponse
            {
                StatusCode = responseMessage.StatusCode,
                StatusText = responseMessage.ReasonPhrase,
                Headers = new HttpResponseHeaders(responseMessage.Headers, responseMessage.Content.Headers),
                Url = responseMessage.RequestMessage?.RequestUri ?? httpRequest.Url
            };
        }

#if NET6_0_OR_GREATER
        [ExcludeFromCodeCoverage]
        internal virtual HttpResponseMessage Send(System.Net.Http.HttpClient httpClient, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            VerifyHttpClientSettings();
            return httpClient.Send(requestMessage, cancellationToken);
        }
#endif

        [ExcludeFromCodeCoverage]
        internal virtual async Task<HttpResponseMessage> SendAsync(System.Net.Http.HttpClient httpClient, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            VerifyHttpClientSettings();
            return await httpClient.SendAsync(requestMessage, cancellationToken);
        }

        private CancellationToken GetRequestCancellationToken(CancellationToken originalCancellationToken)
        {
            var cancellationTokenSource = new CancellationTokenSource(_HttpClientSettings.RequestTimeout);

            if (originalCancellationToken != CancellationToken.None)
            {
                var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, originalCancellationToken);
                return linkedCancellationToken.Token;
            }

            return cancellationTokenSource.Token;
        }

        private (System.Net.Http.HttpClient HttpClient, HttpClientHandler Handler) CreateHttpClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                CookieContainer = _CookieContainer,
                AllowAutoRedirect = _HttpClientSettings.MaxRedirects > 0
            };

            if (httpClientHandler.AllowAutoRedirect)
            {
                httpClientHandler.MaxAutomaticRedirections = _HttpClientSettings.MaxRedirects;
            }

            var httpClient = new System.Net.Http.HttpClient(httpClientHandler)
            {
                Timeout = GetHttpClientMaxTimeout()
            };

            return (httpClient, httpClientHandler);
        }

        private static TimeSpan GetHttpClientMaxTimeout()
        {
            if (TimeSpan.TryParse(Environment.GetEnvironmentVariable("TIXFACTORY_HTTP_CLIENT_MAX_TIMEOUT"), out var maxTimeout))
            {
                return maxTimeout;
            }

            return TimeSpan.FromMinutes(10);
        }

        private void VerifyHttpClientSettings()
        {
            if (_HttpClientHandler != null
                && _HttpClientHandler.MaxAutomaticRedirections == _HttpClientSettings.MaxRedirects)
            {
                return;
            }

            // Will this kill any requests currently in flight? Probably..
            _HttpClientHandler?.Dispose();
            _HttpClient?.Dispose();

            var (httpClient, httpClientHandler) = CreateHttpClient();
            _HttpClientHandler = httpClientHandler;
            _HttpClient = httpClient;
        }
    }
}
