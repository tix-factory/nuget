using System;
using TixFactory.Http;

namespace TixFactory.Firebase
{
	/// <summary>
	/// An exception thrown when there is an issue processing a request made to firebase.
	/// </summary>
	public class FirebaseException : Exception
	{
		/// <summary>
		/// The <see cref="IHttpRequest"/> that was sent.
		/// </summary>
		public IHttpRequest HttpRequest { get; }

		/// <summary>
		/// The <see cref="IHttpResponse"/> that was returned from Firebase.
		/// </summary>
		public IHttpResponse HttpResponse { get; }

		/// <summary>
		/// Initializes a new <see cref="FirebaseException"/>.
		/// </summary>
		/// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
		/// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="httpRequest"/>
		/// - <paramref name="httpResponse"/>
		/// </exception>
		public FirebaseException(IHttpRequest httpRequest, IHttpResponse httpResponse)
			: base(GetMessage(httpRequest, httpResponse))
		{
			HttpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
			HttpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
		}

		private static string GetMessage(IHttpRequest httpRequest, IHttpResponse httpResponse)
		{
			return $"Firebase request failed.\n\tRequest Url: {httpRequest?.Url} ({httpRequest?.Method})\n\tResponse status code: {httpResponse?.StatusCode} ({httpResponse?.StatusText})";
		}
	}
}
