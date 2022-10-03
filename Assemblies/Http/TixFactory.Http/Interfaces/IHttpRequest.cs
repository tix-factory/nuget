using System;
using System.Net.Http;

namespace TixFactory.Http
{
	/// <summary>
	/// An object representing HTTP request information
	/// </summary>
	public interface IHttpRequest
	{
		/// <summary>
		/// The <see cref="HttpMethod"/>.
		/// </summary>
		HttpMethod Method { get; set; }

		/// <summary>
		/// The <see cref="Uri"/>.
		/// </summary>
		Uri Url { get; set; }

		/// <summary>
		/// The <see cref="IHttpRequestHeaders"/>.
		/// </summary>
		IHttpRequestHeaders Headers { get; }

		/// <summary>
		/// The request body.
		/// </summary>
		HttpContent Body { get; set; }
	}
}
