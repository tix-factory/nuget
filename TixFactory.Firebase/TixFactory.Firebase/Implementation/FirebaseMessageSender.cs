using System;
using System.Net.Http;
using Newtonsoft.Json;
using TixFactory.Http;
using TixFactory.Http.Client;
using HttpMethod = TixFactory.Http.HttpMethod;

namespace TixFactory.Firebase
{
	/// <inheritdoc cref="IFirebaseMessageSender"/>
	public class FirebaseMessageSender : IFirebaseMessageSender
	{
		private readonly IHttpClient _HttpClient;
		private readonly Uri _SendUri = new Uri($"https://{FirebaseDomain.Fcm}/fcm/send");

		/// <summary>
		/// Initializes a new <see cref="FirebaseMessageSender"/>.
		/// </summary>
		/// <param name="httpClient">An <see cref="IHttpClient"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="httpClient"/></exception>
		public FirebaseMessageSender(IHttpClient httpClient)
		{
			_HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		/// <inheritdoc cref="IFirebaseMessageSender.Send"/>
		public void Send(string topic, object data)
		{
			if (string.IsNullOrWhiteSpace(topic))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
			}

			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var requestBody = new FirebaseMessage
			{
				To = $"/topics/{topic}",
				Data = data
			};

			var request = new HttpRequest(HttpMethod.Post, _SendUri);
			request.Body = new StringContent(JsonConvert.SerializeObject(requestBody));
			request.Headers.AddOrUpdate("Content-Type", "application/json");

			var response = _HttpClient.Send(request);
			if (!response.IsSuccessful)
			{
				throw new FirebaseException(request, response);
			}
		}
	}
}
