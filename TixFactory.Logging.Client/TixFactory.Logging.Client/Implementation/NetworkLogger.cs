using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TixFactory.Http;
using TixFactory.Http.Client;
using HttpMethod = TixFactory.Http.HttpMethod;

namespace TixFactory.Logging.Client
{
	/// <summary>
	/// An <see cref="ILogger"/> designed to send log messages to tix-factory/monitoring/Services/TixFactory.Logging.Service
	/// </summary>
	public class NetworkLogger : ILogger
	{
		private const int _MaxMessageLength = 8192;
		private readonly IHttpClient _HttpClient;
		private readonly ILogger _FailureLogger;
		private readonly string _LogName;
		private readonly string _LogServiceHostName;
		
		/// <summary>
		/// Initializes a new <see cref="NetworkLogger"/>.
		/// </summary>
		/// <param name="httpClient">An <see cref="IHttpClient"/>.</param>
		/// <param name="failureLogger">An <see cref="ILogger"/> that logs failures that couldn't go to the network.</param>
		/// <param name="logName">The log name associated with the logs output from this logger.</param>
		/// <param name="logServiceHostName">The host name of the logging service.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="httpClient"/>
		/// - <paramref name="failureLogger"/>
		/// </exception>
		public NetworkLogger(IHttpClient httpClient, ILogger failureLogger, string logName, string logServiceHostName)
		{
			if (string.IsNullOrWhiteSpace(logName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(logName));
			}

			if (string.IsNullOrWhiteSpace(logServiceHostName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(logServiceHostName));
			}

			_HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_FailureLogger = failureLogger ?? throw new ArgumentNullException(nameof(failureLogger));
			_LogName = logName;
			_LogServiceHostName = logServiceHostName;
		}

		/// <inheritdoc cref="ILogger.Verbose"/>
		public void Verbose(string message)
		{
			Write(LogLevel.Verbose, message);
		}

		/// <inheritdoc cref="ILogger.Info"/>
		public void Info(string message)
		{
			Write(LogLevel.Information, message);
		}

		/// <inheritdoc cref="ILogger.Warn"/>
		public void Warn(string message)
		{
			Write(LogLevel.Warning, message);
		}

		/// <inheritdoc cref="ILogger.Error(Exception)"/>
		public void Error(Exception ex)
		{
			Write(LogLevel.Error, $"{ex}");
		}

		/// <inheritdoc cref="ILogger.Error(string)"/>
		public void Error(string message)
		{
			Write(LogLevel.Error, message);
		}

		/// <inheritdoc cref="ILogger.Write"/>
		public void Write(LogLevel logLevel, string message)
		{
			Task.Run(() => WriteInBackground(logLevel, message));
		}

		private void WriteInBackground(LogLevel logLevel, string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			if (message.Length > _MaxMessageLength)
			{
				message = message.Substring(0, _MaxMessageLength);
			}

			var requestBody = new LogRequest
			{
				Message = message,
				Host = new HostData
				{
					Name = Environment.MachineName
				},
				Log = new LogData
				{
					Level = logLevel,
					Name = _LogName
				}
			};

			var requestJson = JsonConvert.SerializeObject(requestBody);
			var httpRequest = new HttpRequest(HttpMethod.Post, new Uri($"http://{_LogServiceHostName}/v1/Log"));
			httpRequest.Body = new StringContent(requestJson);
			httpRequest.Headers.AddOrUpdate("Content-Type", "application/json");

			try
			{
				var response = _HttpClient.Send(httpRequest);
				if (!response.IsSuccessful)
				{
					_FailureLogger.Error($"{nameof(NetworkLogger)}.{nameof(Write)}({logLevel}) failed ({response.StatusCode}: {response.Url})\n{response.GetStringBody()}\n{message}");
				}
			}
			catch (Exception e)
			{
				_FailureLogger.Error($"{nameof(NetworkLogger)}.{nameof(Write)}({logLevel}) threw unhandled exception\n\nORIGINAL MESSAGE\n{message}\n\nUNHANDLED LOGGER EXCEPTION\n{e}");
			}
		}
	}
}
