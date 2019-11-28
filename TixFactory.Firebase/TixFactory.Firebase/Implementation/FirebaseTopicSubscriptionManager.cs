using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TixFactory.Http;
using TixFactory.Http.Client;

namespace TixFactory.Firebase
{
	/// <inheritdoc cref="IFirebaseTopicSubscriptionManager"/>
	public class FirebaseTopicSubscriptionManager : IFirebaseTopicSubscriptionManager
	{
		private const string _IidDomain = "https://" + FirebaseDomain.Iid;

		private readonly IHttpClient _HttpClient;

		/// <summary>
		/// Initializes a new <see cref="FirebaseTopicSubscriptionManager"/>.
		/// </summary>
		/// <param name="httpClient">An <see cref="IHttpClient"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="httpClient"/></exception>
		public FirebaseTopicSubscriptionManager(IHttpClient httpClient)
		{
			_HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		/// <inheritdoc cref="IFirebaseTopicSubscriptionManager.Subscribe"/>
		public void Subscribe(string token, string topic)
		{
			var request = new HttpRequest(HttpMethod.Post, new Uri($"{_IidDomain}/iid/v1/{token}/rel/topics/{topic}"));
			var response = _HttpClient.Send(request);

			if (!response.IsSuccessful)
			{
				throw new FirebaseException(request, response);
			}
		}

		/// <inheritdoc cref="IFirebaseTopicSubscriptionManager.Unsubscribe"/>
		public void Unsubscribe(string token, string topic)
		{
			var request = new HttpRequest(HttpMethod.Delete, new Uri($"{_IidDomain}/iid/v1/{token}/rel/topics/{topic}"));
			var response = _HttpClient.Send(request);

			if (!response.IsSuccessful)
			{
				throw new FirebaseException(request, response);
			}
		}

		/// <inheritdoc cref="IFirebaseTopicSubscriptionManager.GetSubscribedTopics"/>
		public ICollection<string> GetSubscribedTopics(string token)
		{
			var request = new HttpRequest(HttpMethod.Get, new Uri($"{_IidDomain}/iid/info/{token}?details=true"));
			request.Headers.AddOrUpdate("Accept", "application/json");

			var response = _HttpClient.Send(request);

			if (!response.IsSuccessful)
			{
				throw new FirebaseException(request, response);
			}

			var responseModel = JsonConvert.DeserializeObject<TokenDetailsResponse>(response.GetStringBody());
			return responseModel.Rel.Topics.Keys;
		}
	}
}
