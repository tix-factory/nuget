using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TixFactory.Configuration;
using TixFactory.Http;
using TixFactory.Http.Client;
using HttpMethod = TixFactory.Http.HttpMethod;

namespace TixFactory.Queueing.Remote
{
	/// <inheritdoc cref="IRemoteItemQueue{TItem}"/>
	public class RemoteItemQueueV3<TItem> : IRemoteItemQueue<TItem>
	{
		private const int _MaxLeaseAttempts = 20;
		private const string _ApiKeyHeaderName = "Tix-Factory-Api-Key";
		private readonly string _BaseUrl;
		private readonly IHttpClient _HttpClient;
		private readonly SemaphoreSlim _SizeCheckLock;
		private readonly IManufacturedSetting<long> _QueueSize;
		private readonly IManufacturedSetting<long> _HeldQueueSize;
		private readonly IReadOnlySetting<Guid> _ApplicationApiKey;

		/// <inheritdoc cref="IRemoteItemQueue{TItem}.OnInvalidItem"/>
		public event Action<QueueItemResult> OnInvalidItem;

		/// <inheritdoc cref="IItemQueue{TItem}.QueueSize"/>
		public IReadOnlySetting<long> QueueSize { get; }

		/// <inheritdoc cref="IItemQueue{TItem}.HeldQueueSize"/>
		public IReadOnlySetting<long> HeldQueueSize { get; }

		/// <inheritdoc cref="IRemoteItemQueue{TItem}.QueueName"/>
		public string QueueName { get; }

		/// <summary>
		/// Initializes a new <see cref="RemoteItemQueueV3{TItem}"/>.
		/// </summary>
		/// <param name="queueName">The remote queue name.</param>
		/// <param name="baseUrl">The <see cref="Uri"/>, pointing at the root URL for the remote queue service.</param>
		/// <param name="httpClient">An <see cref="IHttpClient"/>.</param>
		/// <param name="applicationApiKey">An <see cref="IReadOnlySetting{T}"/> containing the application ApiKey.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="baseUrl"/>
		/// - <paramref name="httpClient"/>
		/// - <paramref name="applicationApiKey"/>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// - <paramref name="queueName"/> is <c>null</c> or white space.
		/// </exception>
		public RemoteItemQueueV3(string queueName, Uri baseUrl, IHttpClient httpClient, IReadOnlySetting<Guid> applicationApiKey)
		{
			if (baseUrl == null)
			{
				throw new ArgumentNullException(nameof(baseUrl));
			}

			if (string.IsNullOrWhiteSpace(queueName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(queueName));
			}

			QueueName = queueName;
			_BaseUrl = baseUrl.GetLeftPart(UriPartial.Authority);
			_HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_ApplicationApiKey = applicationApiKey ?? throw new ArgumentNullException(nameof(applicationApiKey));

			_SizeCheckLock = new SemaphoreSlim(1, 1);
			QueueSize = _QueueSize = new ManufacturedSetting<long>(() => GetQueueSize(), refreshOnRead: true);
			HeldQueueSize = _HeldQueueSize = new ManufacturedSetting<long>(() => GetHeldQueueSize(), refreshOnRead: true);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.AppendItemToQueue"/>
		public void AppendItemToQueue(TItem item)
		{
			var httpRequest = BuildPostRequest("v1/AddQueueItem", new AddQueueItemRequest
			{
				QueueName = QueueName,
				Data = JsonConvert.SerializeObject(new RemoteQueueItem<TItem>
				{
					Data = item
				})
			});

			var httpResponse = _HttpClient.Send(httpRequest);

			ParseResponse<Payload<AddQueueItemResult>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.AppendItemToQueueAsync"/>
		public async Task AppendItemToQueueAsync(TItem item, CancellationToken cancellationToken)
		{
			var httpRequest = BuildPostRequest("v1/AddQueueItem", new AddQueueItemRequest
			{
				QueueName = QueueName,
				Data = JsonConvert.SerializeObject(new RemoteQueueItem<TItem>
				{
					Data = item
				})
			});

			var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

			ParseResponse<Payload<AddQueueItemResult>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.TryGetNextQueueItem"/>
		public bool TryGetNextQueueItem(TimeSpan lockExpiration, out QueueItem<TItem> queueItem)
		{
			for (var n = 0; n < _MaxLeaseAttempts; n++)
			{
				var httpRequest = BuildPostRequest("v1/LeaseQueueItem", new LeaseQueueItemRequest
				{
					QueueName = QueueName,
					LeaseExpiry = lockExpiration
				});

				var httpResponse = _HttpClient.Send(httpRequest);

				var serializedQueueItem = ParseResponse<Payload<QueueItemResult>>(httpRequest, httpResponse);
				if (serializedQueueItem == null)
				{
					break;
				}

				try
				{
					var queueItemData = JsonConvert.DeserializeObject<RemoteQueueItem<TItem>>(serializedQueueItem.Data.Data);
					if (queueItemData.Data != null)
					{
						queueItem = new QueueItem<TItem>(serializedQueueItem.Data.Id.ToString(), queueItemData.Data)
						{
							HolderId = serializedQueueItem.Data.LeaseId.ToString(),
							LockExpiration = DateTime.UtcNow + lockExpiration
						};

						return true;
					}
				}
				catch
				{
					// Deserialization error, trigger invalid item event.
				}

				var invalidItemListener = OnInvalidItem;
				if (invalidItemListener != null)
				{
					ThreadPool.QueueUserWorkItem(state => invalidItemListener.Invoke(serializedQueueItem.Data));
				}
			}

			queueItem = null;
			return false;
		}

		/// <inheritdoc cref="IItemQueue{TItem}.TryGetNextQueueItemAsync"/>
		public async Task<(bool, QueueItem<TItem>)> TryGetNextQueueItemAsync(TimeSpan lockExpiration, CancellationToken cancellationToken)
		{
			var success = false;
			QueueItem<TItem> queueItem = null;

			for (var n = 0; n < _MaxLeaseAttempts; n++)
			{
				var httpRequest = BuildPostRequest("v1/LeaseQueueItem", new LeaseQueueItemRequest
				{
					QueueName = QueueName,
					LeaseExpiry = lockExpiration
				});

				var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

				var serializedQueueItem = ParseResponse<Payload<QueueItemResult>>(httpRequest, httpResponse);
				if (serializedQueueItem == null)
				{
					break;
				}

				try
				{
					var queueItemData = JsonConvert.DeserializeObject<RemoteQueueItem<TItem>>(serializedQueueItem.Data.Data);
					if (queueItemData.Data != null)
					{
						success = true;
						queueItem = new QueueItem<TItem>(serializedQueueItem.Data.Id.ToString(), queueItemData.Data)
						{
							HolderId = serializedQueueItem.Data.LeaseId.ToString(),
							LockExpiration = DateTime.UtcNow + lockExpiration
						};

						break;
					}
				}
				catch
				{
					// Deserialization error, trigger invalid item event.
				}

				var invalidItemListener = OnInvalidItem;
				if (invalidItemListener != null)
				{
					ThreadPool.QueueUserWorkItem(state => invalidItemListener.Invoke(serializedQueueItem.Data));
				}
			}

			return (success, queueItem);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.RemoveQueueItem"/>
		public void RemoveQueueItem(string id, string holderId)
		{
			var httpRequest = BuildPostRequest("v1/RemoveQueueItem", new ReleaseQueueItemRequest
			{
				Id = Convert.ToInt64(id),
				LeaseId = Guid.Parse(holderId)
			});

			var httpResponse = _HttpClient.Send(httpRequest);

			ParseResponse<Payload<ReleaseQueueItemResult>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.RemoveQueueItemAsync"/>
		public async Task RemoveQueueItemAsync(string id, string holderId, CancellationToken cancellationToken)
		{
			var httpRequest = BuildPostRequest("v1/RemoveQueueItem", new ReleaseQueueItemRequest
			{
				Id = Convert.ToInt64(id),
				LeaseId = Guid.Parse(holderId)
			});

			var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

			ParseResponse<Payload<ReleaseQueueItemResult>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ReleaseQueueItem"/>
		public void ReleaseQueueItem(string id, string holderId)
		{
			var httpRequest = BuildPostRequest("v1/ReleaseQueueItem", new ReleaseQueueItemRequest
			{
				Id = Convert.ToInt64(id),
				LeaseId = Guid.Parse(holderId)
			});

			var httpResponse = _HttpClient.Send(httpRequest);

			ParseResponse<Payload<ReleaseQueueItemResult>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ReleaseQueueItemAsync"/>
		public async Task ReleaseQueueItemAsync(string id, string holderId, CancellationToken cancellationToken)
		{
			var httpRequest = BuildPostRequest("v1/ReleaseQueueItem", new ReleaseQueueItemRequest
			{
				Id = Convert.ToInt64(id),
				LeaseId = Guid.Parse(holderId)
			});

			var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

			ParseResponse<Payload<ReleaseQueueItemResult>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.Clear"/>
		public void Clear()
		{
			var httpRequest = BuildPostRequest("v1/ClearQueue", new RequestPayload<string>
			{
				Data = QueueName
			});

			var httpResponse = _HttpClient.Send(httpRequest);

			ParseResponse<Payload<long>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IItemQueue{TItem}.ClearAsync"/>
		public async Task ClearAsync(CancellationToken cancellationToken)
		{
			var httpRequest = BuildPostRequest("v1/ClearQueue", new RequestPayload<string>
			{
				Data = QueueName
			});

			var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

			ParseResponse<Payload<long>>(httpRequest, httpResponse);
		}

		/// <inheritdoc cref="IRemoteItemQueue{TItem}.RefreshQueueSize"/>
		public void RefreshQueueSize()
		{
			if (_SizeCheckLock.CurrentCount == 0)
			{
				return;
			}

			_SizeCheckLock.Wait();

			try
			{
				_QueueSize.Refresh();
				_HeldQueueSize.Refresh();
			}
			finally
			{
				_SizeCheckLock.Release();
			}
		}
		private int GetQueueSize()
		{
			var httpRequest = BuildPostRequest("v1/GetQueueSize", new RequestPayload<string>
			{
				Data = QueueName
			});

			var httpResponse = _HttpClient.Send(httpRequest);
			var responseModel = ParseResponse<Payload<int>>(httpRequest, httpResponse);

			return responseModel.Data;
		}

		private int GetHeldQueueSize()
		{
			var httpRequest = BuildPostRequest("v1/GetHeldQueueSize", new RequestPayload<string>
			{
				Data = QueueName
			});

			var httpResponse = _HttpClient.Send(httpRequest);
			var responseModel = ParseResponse<Payload<int>>(httpRequest, httpResponse);

			return responseModel.Data;
		}

		private IHttpRequest BuildPostRequest(string path, object requestBody)
		{
			var serializedRequestBody = JsonConvert.SerializeObject(requestBody);
			var httpRequest = new HttpRequest(HttpMethod.Post, new Uri($"{_BaseUrl}/{path}"));
			httpRequest.Body = new StringContent(serializedRequestBody);
			httpRequest.Headers.AddOrUpdate(HttpRequestHeaderName.ContentType, "application/json");
			httpRequest.Headers.AddOrUpdate(_ApiKeyHeaderName, _ApplicationApiKey.Value.ToString());

			return httpRequest;
		}

		private T ParseResponse<T>(IHttpRequest httpRequest, IHttpResponse httpResponse)
			where T : class
		{
			if (!httpResponse.IsSuccessful)
			{
				throw new HttpException(httpRequest, httpResponse);
			}

			var responseBody = httpResponse.GetStringBody();
			var responseModel = JsonConvert.DeserializeObject<T>(responseBody);

			return responseModel;
		}
	}
}
