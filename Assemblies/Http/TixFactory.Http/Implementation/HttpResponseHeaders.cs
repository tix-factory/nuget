using System.Net;
using System.Net.Http.Headers;

namespace TixFactory.Http
{
    /// <inheritdoc cref="IHttpResponseHeaders"/>
    public class HttpResponseHeaders : HttpHeaders, IHttpResponseHeaders
    {
        /// <inheritdoc cref="HttpHeaders()"/>
        public HttpResponseHeaders()
        {

        }

        /// <inheritdoc cref="HttpHeaders(WebHeaderCollection)"/>
        public HttpResponseHeaders(WebHeaderCollection webHeaders)
            : base(webHeaders)
        {
        }

        /// <inheritdoc cref="HttpHeaders(System.Net.Http.Headers.HttpHeaders, HttpContentHeaders)"/>
        public HttpResponseHeaders(System.Net.Http.Headers.HttpHeaders httpHeaders, HttpContentHeaders httpContentHeaders)
            : base(httpHeaders, httpContentHeaders)
        {
        }
    }
}
