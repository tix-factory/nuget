using System.Net;
using System.Net.Http.Headers;

namespace TixFactory.Http
{
	/// <inheritdoc cref="IHttpResponseHeaders"/>
	public class HttpResponseHeaders : HttpHeaders, IHttpResponseHeaders
	{
		/// <inheritdoc cref="TixFactory.Http.HttpHeaders()"/>
		public HttpResponseHeaders()
		{
			
		}

		/// <inheritdoc cref="TixFactory.Http.HttpHeaders(WebHeaderCollection)"/>
		public HttpResponseHeaders(WebHeaderCollection webHeaders)
			: base(webHeaders)
		{
		}

		/// <inheritdoc cref="TixFactory.Http.HttpHeaders(System.Net.Http.Headers.HttpHeaders, HttpContentHeaders)"/>
		public HttpResponseHeaders(System.Net.Http.Headers.HttpHeaders httpHeaders, HttpContentHeaders httpContentHeaders)
			: base(httpHeaders, httpContentHeaders)
		{
		}
	}
}
