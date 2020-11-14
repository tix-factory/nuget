using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.Configuration;
using TixFactory.Http;
using TixFactory.Http.Client;
using TixFactory.Logging;
using HttpClient = TixFactory.Http.Client.HttpClient;
using HttpMethod = TixFactory.Http.HttpMethod;

namespace TixFactory.ApplicationAuthorization
{
	/// <inheritdoc cref="IApplicationAuthorizationsAccessor"/>
	public class ApplicationAuthorizationsAccessor : IApplicationAuthorizationsAccessor
	{
		private const int _MaxParallelRequests = 3;
		private const string _ApiKeyHeaderName = "Tix-Factory-Api-Key";

		private readonly ILogger _Logger;
		private readonly ISetting<Guid> _ApplicationApiKey;
		private readonly ISetting<TimeSpan> _CacheExpiry;
		private readonly ConcurrentDictionary<Guid, ApplicationAuthorization> _ApplicationAuthorizationsByApiKey;
		private readonly SemaphoreSlim _LoadApplicationAuthorizationsLock;
		private readonly IHttpClient _HttpClient;
		private readonly Uri _LoadAuthorizationsUrl;
		private readonly JsonSerializerOptions _JsonSerializerOptions;
		private bool _RefreshDebounce = false;

		/// <summary>
		/// Initializes a new <see cref="ApplicationAuthorizationsAccessor"/>.
		/// </summary>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <param name="serviceUrl">The base <see cref="Uri"/> for the application authorization service.</param>
		/// <param name="applicationApiKey">The running application ApiKey.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="logger"/>
		/// - <paramref name="serviceUrl"/>
		/// - <paramref name="applicationApiKey"/>
		/// </exception>
		public ApplicationAuthorizationsAccessor(ILogger logger, Uri serviceUrl, ISetting<Guid> applicationApiKey)
			: this(logger, serviceUrl, applicationApiKey, new Setting<TimeSpan>(TimeSpan.FromMinutes(1)))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ApplicationAuthorizationsAccessor"/>.
		/// </summary>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <param name="serviceUrl">The base <see cref="Uri"/> for the application authorization service.</param>
		/// <param name="applicationApiKey">The running application ApiKey.</param>
		/// <param name="cacheExpiry">The max time to cache the authorized operations before forgetting the checked ApiKey.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="logger"/>
		/// - <paramref name="serviceUrl"/>
		/// - <paramref name="applicationApiKey"/>
		/// - <paramref name="cacheExpiry"/>
		/// </exception>
		public ApplicationAuthorizationsAccessor(ILogger logger, Uri serviceUrl, ISetting<Guid> applicationApiKey, ISetting<TimeSpan> cacheExpiry)
		{
			if (serviceUrl == null)
			{
				throw new ArgumentNullException(nameof(serviceUrl));
			}

			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_LoadAuthorizationsUrl = new Uri($"{serviceUrl.GetLeftPart(UriPartial.Authority)}/v1/GetAuthorizedOperations");
			_ApplicationApiKey = applicationApiKey ?? throw new ArgumentNullException(nameof(applicationApiKey));
			_CacheExpiry = cacheExpiry ?? throw new ArgumentNullException(nameof(cacheExpiry));
			_ApplicationAuthorizationsByApiKey = new ConcurrentDictionary<Guid, ApplicationAuthorization>();
			_HttpClient = new HttpClient();
			_LoadApplicationAuthorizationsLock = new SemaphoreSlim(_MaxParallelRequests, _MaxParallelRequests);

			var jsonSerializerOptions = _JsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var refreshTimer = new Timer(async state => await RefreshAuthorizations(state, CancellationToken.None).ConfigureAwait(false),
				state: null,
				dueTime: TimeSpan.Zero,
				period: TimeSpan.FromSeconds(15));
		}

		/// <inheritdoc cref="IApplicationAuthorizationsAccessor.GetAuthorizedOperationNames"/>
		public async Task<ISet<string>> GetAuthorizedOperationNames(Guid apiKey, CancellationToken cancellationToken)
		{
			if (apiKey == default)
			{
				return new HashSet<string>();
			}

			var currentTime = DateTime.UtcNow;
			if (_ApplicationAuthorizationsByApiKey.TryGetValue(apiKey, out var applicationAuthorization))
			{
				applicationAuthorization.LastAccess = currentTime;
				return applicationAuthorization.OperationNames;
			}

			await _LoadApplicationAuthorizationsLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			ISet<string> operationNames;

			try
			{
				if (_ApplicationAuthorizationsByApiKey.TryGetValue(apiKey, out applicationAuthorization))
				{
					applicationAuthorization.LastAccess = currentTime;
					operationNames = applicationAuthorization.OperationNames;
				}
				else
				{
					operationNames = await LoadAuthorizations(apiKey, cancellationToken).ConfigureAwait(false);
				}
			}
			finally
			{
				_LoadApplicationAuthorizationsLock.Release();
			}

			_ApplicationAuthorizationsByApiKey[apiKey] = new ApplicationAuthorization
			{
				OperationNames = operationNames,
				LastAccess = currentTime,
				LastRefresh = currentTime
			};

			return operationNames;
		}

		private async Task<ISet<string>> LoadAuthorizations(Guid apiKey, CancellationToken cancellationToken)
		{
			var httpRequest = new HttpRequest(HttpMethod.Post, _LoadAuthorizationsUrl);
			httpRequest.Body = new StringContent($"{{\"apiKey\":\"{apiKey}\"}}");
			httpRequest.Headers.Add(_ApiKeyHeaderName, _ApplicationApiKey.Value.ToString());
			httpRequest.Headers.AddOrUpdate(HttpRequestHeaderName.ContentType, MediaTypeNames.Application.Json);

			var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
			var responseBody = httpResponse.GetStringBody();
			if (!httpResponse.IsSuccessful)
			{
				throw new HttpException($"Failed to load ApiKey authorizations"
										+ $"\n\tUrl: {httpResponse.Url}"
										+ $"\n\tStatus Code: {httpResponse.StatusCode} ({httpResponse.StatusText})"
										+ $"\n\tResponse Body: {responseBody}",
					innerException: new Exception("fake exception to satisfy compiler"));
			}

			var responseModel = JsonSerializer.Deserialize<AuthorizedOperationsResult>(responseBody, _JsonSerializerOptions);
			return new HashSet<string>(responseModel.Data);
		}

		private async Task RefreshAuthorizations(object state, CancellationToken cancellationToken)
		{
			if (_RefreshDebounce)
			{
				return;
			}

			_RefreshDebounce = true;

			try
			{
				var currentTime = DateTime.UtcNow;
				var refreshTasks = new Dictionary<ApplicationAuthorization, Task<ISet<string>>>();

				foreach (var apiKey in _ApplicationAuthorizationsByApiKey.Keys)
				{
					if (_ApplicationAuthorizationsByApiKey.TryGetValue(apiKey, out var applicationAuthorization))
					{
						if (currentTime - applicationAuthorization.LastRefresh < _CacheExpiry.Value)
						{
							continue;
						}

						if (currentTime - applicationAuthorization.LastAccess < _CacheExpiry.Value)
						{
							// Refresh, we've seen them recently.
							applicationAuthorization.LastRefresh = currentTime;
							refreshTasks.Add(applicationAuthorization, LoadAuthorizations(apiKey, cancellationToken));
						}
						else
						{
							_ApplicationAuthorizationsByApiKey.TryRemove(apiKey, out _);
						}
					}
				}

				await Task.WhenAll(refreshTasks.Values).ConfigureAwait(false);

				currentTime = DateTime.UtcNow;
				foreach (var refreshTask in refreshTasks)
				{
					refreshTask.Key.LastRefresh = currentTime;

					if (refreshTask.Value.IsCompletedSuccessfully)
					{
						refreshTask.Key.OperationNames = refreshTask.Value.Result;
					}
				}
			}
			catch (Exception e)
			{
				_Logger.Warn($"{nameof(ApplicationAuthorizationsAccessor)}.{nameof(RefreshAuthorizations)}\n{e}");
			}
			finally
			{
				_RefreshDebounce = false;
			}
		}
	}
}
