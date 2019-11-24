using System;

namespace TixFactory.Http
{
	/// <summary>
	/// An exception thrown when an HTTP request times out.
	/// </summary>
	public class HttpTimeoutException : HttpException
	{
		/// <summary>
		/// Initializes a new <see cref="HttpTimeoutException"/>.
		/// </summary>
		public HttpTimeoutException()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="HttpTimeoutException"/>.
		/// </summary>
		/// <param name="message">The <see cref="Exception.Message"/>.</param>
		/// <param name="innerException">The <see cref="Exception.InnerException"/>.</param>
		public HttpTimeoutException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
