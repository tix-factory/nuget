using System;

namespace TixFactory.Http
{
	/// <summary>
	/// An exception thrown when there is an error with an HTTP request
	/// </summary>
	public class HttpException : Exception
	{
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
		/// <param name="innerException">The <see cref="Exception.InnerException"/>.</param>
		public HttpException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
