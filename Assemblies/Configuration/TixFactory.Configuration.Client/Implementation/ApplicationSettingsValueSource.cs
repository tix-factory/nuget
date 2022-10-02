using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using TixFactory.ApplicationContext;
using TixFactory.Http;
using TixFactory.Http.Client;
using TixFactory.Logging;

namespace TixFactory.Configuration.Client
{
	/// <summary>
	/// An <see cref="ISettingValueSource"/> that pulls configuration from https://github.com/tix-factory/application-configuration-service
	/// </summary>
	public class ApplicationSettingsValueSource : ISettingValueSource
	{
		private const string _ApiKeyHeaderName = "Tix-Factory-Api-Key";
		private readonly ILogger _Logger;
		private readonly ISetting<Guid> _ApplicationApiKey;
		private readonly IApplicationContext _ApplicationContext;
		private readonly Uri _GetApplicationSettingsUrl;
		private readonly IHttpClient _HttpClient;
		private readonly JsonSerializerOptions _JsonSerializerOptions;
		private readonly IRefreshAheadSetting<IReadOnlyDictionary<string, string>> _SettingsValues;

		/// <inheritdoc cref="ISettingValueSource.SettingValueChanged"/>
		public event Action<string, string> SettingValueChanged;

		/// <summary>
		/// Initializes a new <see cref="ApplicationSettingsValueSource"/>.
		/// </summary>
		/// <param name="configurationServiceUrl">The configuration service base <see cref="Uri"/>.</param>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <param name="applicationApiKey">An <see cref="ISetting{T}"/> with the application ApiKey.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="configurationServiceUrl"/>
		/// - <paramref name="logger"/>
		/// - <paramref name="applicationApiKey"/>
		/// </exception>
		public ApplicationSettingsValueSource(Uri configurationServiceUrl, ILogger logger, ISetting<Guid> applicationApiKey)
			: this(configurationServiceUrl, logger, applicationApiKey, ApplicationContext.ApplicationContext.Singleton, new Setting<TimeSpan>(TimeSpan.FromSeconds(30)))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="ApplicationSettingsValueSource"/>.
		/// </summary>
		/// <param name="configurationServiceUrl">The configuration service base <see cref="Uri"/>.</param>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <param name="applicationApiKey">An <see cref="ISetting{T}"/> with the application ApiKey.</param>
		/// <param name="applicationContext">An <see cref="IApplicationContext"/>.</param>
		/// <param name="refreshInterval">An <see cref="ISetting{T}"/> with the refresh interval <see cref="TimeSpan"/>, for how long to wait between checking for changed settings.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="configurationServiceUrl"/>
		/// - <paramref name="logger"/>
		/// - <paramref name="applicationApiKey"/>
		/// - <paramref name="applicationContext"/>
		/// - <paramref name="refreshInterval"/>
		/// </exception>
		public ApplicationSettingsValueSource(Uri configurationServiceUrl, ILogger logger, ISetting<Guid> applicationApiKey, IApplicationContext applicationContext, ISetting<TimeSpan> refreshInterval)
		{
			if (configurationServiceUrl == null)
			{
				throw new ArgumentNullException(nameof(configurationServiceUrl));
			}

			if (refreshInterval == null)
			{
				throw new ArgumentNullException(nameof(refreshInterval));
			}

			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_ApplicationApiKey = applicationApiKey ?? throw new ArgumentNullException(nameof(applicationApiKey));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));

			_GetApplicationSettingsUrl = new Uri(configurationServiceUrl.GetLeftPart(UriPartial.Authority) + "/v1/GetApplicationSettings");
			_HttpClient = new HttpClient();
			_JsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var settingsValues = _SettingsValues = new RefreshAheadSetting<IReadOnlyDictionary<string, string>>(LoadApplicationSettings, refreshInterval);
			settingsValues.RefreshException += HandleException;
			settingsValues.Changed += SettingsChanged;
		}

		/// <inheritdoc cref="ISettingValueSource.TryGetSettingValue{T}"/>
		public bool TryGetSettingValue<T>(string groupName, string settingName, out T value)
		{
			if (groupName != _ApplicationContext.Name)
			{
				throw new NotSupportedException($"{nameof(ApplicationSettingsValueSource)} only supports loading the application settings group ({groupName})");
			}

			value = default;

			if (!_SettingsValues.Value.TryGetValue(settingName, out var rawValue))
			{
				return false;
			}

			var converter = TypeDescriptor.GetConverter(typeof(T));
			if (!converter.CanConvertFrom(typeof(string)))
			{
				return false;
			}

			try
			{
				value = (T)converter.ConvertFromString(rawValue);
				return true;
			}
			catch (Exception e)
			{
				_Logger.Warn($"{nameof(ApplicationSettingsValueSource)} could not convert setting to requested type.\n\tSetting: {settingName} (in group: {groupName})\n\tValue: {rawValue}\n\tExpected type: {typeof(T).FullName}\n{e}");
				return false;
			}
		}

		/// <inheritdoc cref="ISettingValueSource.WriteSettingValue{T}"/>
		public void WriteSettingValue<T>(string groupName, string settingName, T value)
		{
			if (groupName != _ApplicationContext.Name)
			{
				throw new NotSupportedException($"{nameof(ApplicationSettingsValueSource)} only supports loading the application settings group ({groupName})");
			}

			throw new NotImplementedException($"{nameof(ApplicationSettingsValueSource)} does not support writing settings at this time.");
		}

		private IReadOnlyDictionary<string, string> LoadApplicationSettings()
		{
			var httpRequest = new HttpRequest(HttpMethod.Post, _GetApplicationSettingsUrl);
			httpRequest.Headers.AddOrUpdate(_ApiKeyHeaderName, _ApplicationApiKey.Value.ToString());

			var httpResponse = _HttpClient.Send(httpRequest);
			if (!httpResponse.IsSuccessful)
			{
				throw new HttpException(httpRequest, httpResponse);
			}

			var responseBody = httpResponse.GetStringBody();
			IReadOnlyDictionary<string, string> settings;

			try
			{
				var responseModel = JsonSerializer.Deserialize<DataResponse<Dictionary<string, string>>>(responseBody, _JsonSerializerOptions);
				settings = new Dictionary<string, string>(responseModel.Data, StringComparer.OrdinalIgnoreCase);
			}
			catch (Exception e)
			{
				throw new HttpException($"Failed to parse settings response\n{responseBody}", e)
				{
					Request = httpRequest,
					Response = httpResponse
				};
			}

			return settings;
		}

		private void SettingsChanged(IReadOnlyDictionary<string, string> settings, IReadOnlyDictionary<string, string> previousSettings)
		{
			if (previousSettings == null)
			{
				return;
			}

			var changedSettingNames = new HashSet<string>();

			foreach (var setting in settings)
			{
				if (previousSettings.TryGetValue(setting.Key, out var previousValue))
				{
					if (previousValue != setting.Value)
					{
						// Had the value before, and it changed.
						changedSettingNames.Add(setting.Key);
					}
				}
				else
				{
					// Didn't have setting before, but we do now.
					changedSettingNames.Add(setting.Key);
				}
			}

			foreach (var setting in previousSettings)
			{
				if (settings.TryGetValue(setting.Key, out var value))
				{
					if (value != setting.Value)
					{
						// Had the value before, and it changed. Should be redundant with the above loop.
						changedSettingNames.Add(setting.Key);
					}
				}
				else
				{
					// Had the value before, but don't anymore.
					changedSettingNames.Add(setting.Key);
				}
			}

			foreach (var settingName in changedSettingNames)
			{
				SettingValueChanged?.Invoke(_ApplicationContext.Name, settingName);
			}
		}

		private void HandleException(Exception e)
		{
			_Logger.Warn($"Unhandled exception reading configuration\n{e}");
		}
	}
}
