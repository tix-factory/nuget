using System;
using System.Threading;
using TixFactory.Configuration;
using TixFactory.Http.Client;
using TixFactory.Logging;

namespace TixFactory.Queueing.Remote
{
	/// <inheritdoc cref="IRemoteItemQueueFactory"/>
	public class RemoteItemQueueFactoryV3 : IRemoteItemQueueFactory
	{
		private readonly Uri _BaseUrl;
		private readonly IHttpClient _HttpClient;
		private readonly IReadOnlySetting<Guid> _ApplicationApiKey;
		private readonly ILogger _Logger;

		/// <summary>
		/// The minimum interval to refresh created <see cref="IRemoteItemQueue{IItem}"/> queue sizes.
		/// </summary>
		public static readonly TimeSpan MinimumCountRefreshInterval = TimeSpan.FromSeconds(1);

		/// <summary>
		/// Initailizes a new <see cref="RemoteItemQueueFactoryV3"/>.
		/// </summary>
		/// <param name="baseUrl">The <see cref="Uri"/>, pointing at the root URL for the remote queue service.</param>
		/// <param name="httpClient">An <see cref="IHttpClient"/>.</param>
		/// <param name="applicationApiKey">An <see cref="IReadOnlySetting{T}"/> containing the application ApiKey.</param>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="baseUrl"/>
		/// - <paramref name="httpClient"/>
		/// - <paramref name="applicationApiKey"/>
		/// - <paramref name="logger"/>
		/// </exception>
		public RemoteItemQueueFactoryV3(Uri baseUrl, IHttpClient httpClient, IReadOnlySetting<Guid> applicationApiKey, ILogger logger)
		{
			_BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
			_HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_ApplicationApiKey = applicationApiKey ?? throw new ArgumentNullException(nameof(applicationApiKey));
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc cref="IRemoteItemQueueFactory.CreateRemoteItemQueue{TItem}"/>
		public IRemoteItemQueue<TItem> CreateRemoteItemQueue<TItem>(string queueName, TimeSpan? countRefreshInterval)
		{
			var remoteItemQueue = new RemoteItemQueueV3<TItem>(queueName, _BaseUrl, _HttpClient, _ApplicationApiKey);
			remoteItemQueue.OnInvalidItem += (queueItem) => RemoveInvalidItem(remoteItemQueue, queueItem);

			if (countRefreshInterval.HasValue)
			{
				if (countRefreshInterval < MinimumCountRefreshInterval)
				{
					countRefreshInterval = MinimumCountRefreshInterval;
				}

				var countRefreshTimer = new Timer(
					callback: s => RefreshQueueSize(remoteItemQueue),
					state: null,
					dueTime: countRefreshInterval.Value,
					period: countRefreshInterval.Value);
			}

			return remoteItemQueue;
		}

		private void RemoveInvalidItem<TItem>(IRemoteItemQueue<TItem> remoteItemQueue, QueueItemResult queueItem)
		{
			var itemType = typeof(TItem);
			_Logger.Warn($"Removing item from queue ({remoteItemQueue.QueueName}) because it could not be parsed to {itemType.FullName}\n\tID: {queueItem.Id}\n\tData: {queueItem.Data}");

			try
			{
				remoteItemQueue.RemoveQueueItem(queueItem.Id.ToString(), queueItem.LeaseId.ToString());
			}
			catch (Exception e)
			{
				_Logger.Warn($"Failed to remove invalid queue item ({queueItem.Id})\n{e}");
			}
		}

		private void RefreshQueueSize<TItem>(IRemoteItemQueue<TItem> remoteItemQueue)
		{
			try
			{
				remoteItemQueue.RefreshQueueSize();
			}
			catch (Exception e)
			{
				_Logger.Warn($"Failed to refresh item queue size.\n\tQueue: {remoteItemQueue.QueueName}\n{e}");
			}
		}
	}
}
