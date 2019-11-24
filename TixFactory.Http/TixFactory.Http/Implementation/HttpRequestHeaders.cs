using System.Net;
using System.Net.Http.Headers;

namespace TixFactory.Http
{
	/// <inheritdoc cref="IHttpRequestHeaders"/>
	public class HttpRequestHeaders : HttpHeaders, IHttpRequestHeaders
	{
		/// <inheritdoc cref="TixFactory.Http.HttpHeaders()"/>
		public HttpRequestHeaders()
		{

		}

		/// <inheritdoc cref="TixFactory.Http.HttpHeaders(WebHeaderCollection)"/>
		public HttpRequestHeaders(WebHeaderCollection webHeaders)
			: base(webHeaders)
		{
		}

		/// <inheritdoc cref="TixFactory.Http.HttpHeaders(System.Net.Http.Headers.HttpHeaders, HttpContentHeaders)"/>
		public HttpRequestHeaders(System.Net.Http.Headers.HttpHeaders httpHeaders, HttpContentHeaders httpContentHeaders)
			: base(httpHeaders, httpContentHeaders)
		{
		}
	}
}
