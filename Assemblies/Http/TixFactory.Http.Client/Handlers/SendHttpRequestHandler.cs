using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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

        private readonly CookieContainer _CookieContainer;
        private readonly IHttpClientSettings _HttpClientSettings;
        private System.Net.Http.HttpClient _HttpClient;

        /// <summary>
        /// Initializes a new <see cref="SendHttpRequestHandler"/>.
        /// </summary>
        /// <param name="cookieContainer">The <see cref="CookieContainer"/> to use in the requests (initializes a new one when <c>null</c>.)</param>
        /// <param name="httpClientSettings">An <see cref="IHttpClientSettings"/>.</param>
        public SendHttpRequestHandler(CookieContainer cookieContainer, IHttpClientSettings httpClientSettings)
        {
            _CookieContainer = cookieContainer ?? new CookieContainer();
            _HttpClientSettings = httpClientSettings ?? throw new ArgumentNullException(nameof(httpClientSettings));

            httpClientSettings.SettingChanged += RefreshHttpClient;
            RefreshHttpClient(null);
        }

        /// <inheritdoc cref="IHttpClientHandler.Invoke"/>
        public IHttpResponse Invoke(IHttpRequest request)
        {
            IHttpResponse response;

            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(request.Url);
                webRequest.Method = request.Method.ToString();
                webRequest.CookieContainer = _CookieContainer;
                webRequest.UserAgent = _HttpClientSettings.UserAgent;
                webRequest.AllowAutoRedirect = _HttpClientSettings.MaxRedirects > 0;
                webRequest.KeepAlive = false;

                if (_HttpClientSettings.RequestTimeout > TimeSpan.Zero)
                {
                    webRequest.Timeout = (int)Math.Ceiling(_HttpClientSettings.RequestTimeout.TotalMilliseconds);
                }

                if (webRequest.AllowAutoRedirect)
                {
                    webRequest.MaximumAutomaticRedirections = _HttpClientSettings.MaxRedirects;
                }

                if (!_HttpClientSettings.SslCertificateValidationEnabled)
                {
                    webRequest.ServerCertificateValidationCallback = BypassSslCertificateValidation;
                }

                foreach (var headerName in request.Headers.Keys)
                {
                    foreach (var headerValue in request.Headers.Get(headerName))
                    {
                        switch (headerName)
                        {
                            case HttpRequestHeaderName.Accept:
                                webRequest.Accept = headerValue;
                                break;
                            case HttpRequestHeaderName.ContentType:
                                webRequest.ContentType = headerValue;
                                break;
                            case HttpRequestHeaderName.UserAgent:
                                webRequest.UserAgent = headerValue;
                                break;
                            case HttpRequestHeaderName.Connection:
                                webRequest.KeepAlive = !headerValue.Equals("close", StringComparison.OrdinalIgnoreCase);
                                break;
                            default:
                                webRequest.Headers.Add(headerName, headerValue);
                                break;
                        }
                    }
                }

                if (request.Body != null)
                {
                    using (var requestStream = webRequest.GetRequestStream())
                    {
                        var requestBodyCopyTask = request.Body.CopyToAsync(requestStream);
                        requestBodyCopyTask.Wait();
                    }
                }

                using (var httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    response = BuildHttpResponse(httpWebResponse);
                }
            }
            catch (WebException e)
            {
                if (e.Response is HttpWebResponse httpWebResponse)
                {
                    response = BuildHttpResponse(httpWebResponse);
                }
                else
                {
                    throw new HttpException(_UnexpectedErrorMessage, e);
                }
            }
            catch (SocketException e)
            {
                throw new HttpException(_UnexpectedErrorMessage, e);
            }

            return response;
        }

        /// <inheritdoc cref="IHttpClientHandler.InvokeAsync"/>
        public async Task<IHttpResponse> InvokeAsync(IHttpRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var requestMessage = BuildHttpRequestMessage(request);
                var responseMessage = await SendAsync(_HttpClient, requestMessage, cancellationToken).ConfigureAwait(false);
                var response = await BuildHttpResponseAsync(responseMessage).ConfigureAwait(false);

                return response;
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
        }

        internal HttpRequestMessage BuildHttpRequestMessage(IHttpRequest request)
        {
            var httpMethod = new System.Net.Http.HttpMethod(request.Method.ToString().ToUpper());
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

        internal async Task<IHttpResponse> BuildHttpResponseAsync(HttpResponseMessage responseMessage)
        {
            return new HttpResponse
            {
                StatusCode = responseMessage.StatusCode,
                StatusText = responseMessage.ReasonPhrase,
                Headers = new HttpResponseHeaders(responseMessage.Headers, responseMessage.Content.Headers),
                Url = responseMessage.RequestMessage.RequestUri,
                Body = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)
            };
        }

        internal IHttpResponse BuildHttpResponse(HttpWebResponse httpWebResponse)
        {
            var responseHeaders = new HttpResponseHeaders();
            foreach (var headerName in httpWebResponse.Headers.AllKeys)
            {
                var headerValues = httpWebResponse.Headers.GetValues(headerName);
                if (headerValues != null)
                {
                    foreach (var headerValue in headerValues)
                    {
                        responseHeaders.Add(headerName, headerValue);
                    }
                }
            }

            var responseBodyStream = new MemoryStream();
            using (var responseStream = httpWebResponse.GetResponseStream())
            {
                responseStream?.CopyTo(responseBodyStream);
            }

            return new HttpResponse
            {
                StatusCode = httpWebResponse.StatusCode,
                StatusText = httpWebResponse.StatusDescription,
                Headers = responseHeaders,
                Url = httpWebResponse.ResponseUri,
                Body = responseBodyStream
            };
        }

        [ExcludeFromCodeCoverage]
        internal virtual async Task<HttpResponseMessage> SendAsync(System.Net.Http.HttpClient httpClient, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            return await httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        }

        internal void RefreshHttpClient(string changedPropertyName)
        {
            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.CookieContainer = _CookieContainer;
            httpClientHandler.AllowAutoRedirect = _HttpClientSettings.MaxRedirects > 0;

            if (httpClientHandler.AllowAutoRedirect)
            {
                httpClientHandler.MaxAutomaticRedirections = _HttpClientSettings.MaxRedirects;
            }

            if (!_HttpClientSettings.SslCertificateValidationEnabled)
            {
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ServerCertificateCustomValidationCallback = BypassSslCertificateValidation;
            }

            var httpClient = new System.Net.Http.HttpClient(httpClientHandler);

            if (_HttpClientSettings.RequestTimeout > TimeSpan.Zero)
            {
                httpClient.Timeout = _HttpClientSettings.RequestTimeout;
            }

            _HttpClient = httpClient;
        }

        private bool BypassSslCertificateValidation(HttpRequestMessage httpRequestMessage, X509Certificate2 certificate, X509Chain certificateChain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private bool BypassSslCertificateValidation(object sender, X509Certificate certificate, X509Chain certificateChain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
