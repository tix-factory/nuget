using System.Net;
using System.Net.Http.Headers;

namespace TixFactory.Http
{
    /// <inheritdoc cref="IHttpRequestHeaders"/>
    public class HttpRequestHeaders : HttpHeaders, IHttpRequestHeaders
    {
        /// <inheritdoc cref="HttpHeaders()"/>
        public HttpRequestHeaders()
        {

        }

        /// <inheritdoc cref="HttpHeaders(WebHeaderCollection)"/>
        public HttpRequestHeaders(WebHeaderCollection webHeaders)
            : base(webHeaders)
        {
        }

        /// <inheritdoc cref="HttpHeaders(System.Net.Http.Headers.HttpHeaders, HttpContentHeaders)"/>
        public HttpRequestHeaders(System.Net.Http.Headers.HttpHeaders httpHeaders, HttpContentHeaders httpContentHeaders)
            : base(httpHeaders, httpContentHeaders)
        {
        }
    }
}
