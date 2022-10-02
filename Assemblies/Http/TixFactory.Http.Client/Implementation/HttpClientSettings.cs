using System;

namespace TixFactory.Http.Client
{
	/// <inheritdoc cref="IHttpClientSettings"/>
	public class HttpClientSettings : IHttpClientSettings
	{
		private string _UserAgent = "TixFactory.Http.Client";
		private TimeSpan _RequestTimeout = TimeSpan.FromMinutes(2);
		private int _MaxRedirects = 20;
		private bool _SslCertificateValidationEnabled = true;

#pragma warning disable CS0067
		/// <inheritdoc cref="IHttpClientSettings.SettingChanged"/>
		public event Action<string> SettingChanged;
#pragma warning restore CS0067

		/// <inheritdoc cref="IHttpClientSettings.UserAgent"/>
		public string UserAgent
		{
			get => _UserAgent;
			set
			{
				if (_UserAgent == value)
				{
					return;
				}

				_UserAgent = value;
				SettingChanged?.Invoke(nameof(UserAgent));
			}
		}

		/// <inheritdoc cref="IHttpClientSettings.RequestTimeout"/>
		public TimeSpan RequestTimeout
		{
			get => _RequestTimeout;
			set
			{
				if (_RequestTimeout == value)
				{
					return;
				}

				_RequestTimeout = value;
				SettingChanged?.Invoke(nameof(RequestTimeout));
			}
		}

		/// <inheritdoc cref="IHttpClientSettings.MaxRedirects"/>
		public int MaxRedirects
		{
			get => _MaxRedirects;
			set
			{
				if (_MaxRedirects == value)
				{
					return;
				}

				_MaxRedirects = value;
				SettingChanged?.Invoke(nameof(MaxRedirects));
			}
		}

		/// <inheritdoc cref="IHttpClientSettings.SslCertificateValidationEnabled"/>
		public bool SslCertificateValidationEnabled
		{
			get => _SslCertificateValidationEnabled;
			set
			{
				if (_SslCertificateValidationEnabled == value)
				{
					return;
				}

				_SslCertificateValidationEnabled = value;
				SettingChanged?.Invoke(nameof(SslCertificateValidationEnabled));
			}
		}
	}
}
