using System;

namespace TixFactory.Http
{
	/// <summary>
	/// An exception thrown when there is an error with an HTTP request
	/// </summary>
	public class HttpException : Exception
	{
		/// <summary>
		/// The <see cref="IHttpRequest"/> associated with the exception.
		/// </summary>
		public IHttpRequest Request { get; set; }

		/// <summary>
		/// The <see cref="IHttpResponse"/> associated with the exception.
		/// </summary>
		public IHttpResponse Response { get; set; }

		/// <summary>
		/// Initializes a new <see cref="HttpException"/>.
		/// </summary>
		public HttpException()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="HttpException"/>.
		/// </summary>
		/// <param name="message">The <see cref="Exception.Message"/>.</param>
		public HttpException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="HttpException"/>.
		/// </summary>
		/// <param name="message">The <see cref="Exception.Message"/>.</param>
		/// <param name="innerException">The <see cref="Exception.InnerException"/>.</param>
		public HttpException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="HttpException"/>.
		/// </summary>
		/// <param name="request">The <see cref="Request"/>.</param>
		/// <param name="response">The <see cref="Response"/>.</param>
		public HttpException(IHttpRequest request, IHttpResponse response)
			: this(BuildExceptionMessage(response))
		{
			Request = request;
			Response = response;
		}

		private static string BuildExceptionMessage(IHttpResponse response)
		{
			return $"Url: {response.Url}"
			       + $"Status: {response.StatusCode} ({response.StatusText})"
			       + $"Body\n{response.GetStringBody()}";
		}
	}
}
