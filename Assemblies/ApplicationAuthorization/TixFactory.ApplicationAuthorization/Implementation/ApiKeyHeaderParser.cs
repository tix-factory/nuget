using System;
using Microsoft.AspNetCore.Http;

namespace TixFactory.ApplicationAuthorization
{
	/// <summary>
	/// An <see cref="IApiKeyParser"/> that parses the ApiKey from a request header.
	/// </summary>
	/// <seealso cref="IApiKeyParser"/>
	public class ApiKeyHeaderParser : IApiKeyParser
	{
		private readonly string _ApiKeyHeaderName;

		/// <summary>
		/// Initializes a new <see cref="ApiKeyHeaderParser"/>.
		/// </summary>
		/// <param name="apiKeyHeaderName">The name of the ApiKey header.</param>
		/// <exception cref="ArgumentException">
		/// - <paramref name="apiKeyHeaderName"/> is <c>null</c> or whitespace.
		/// </exception>
		public ApiKeyHeaderParser(string apiKeyHeaderName)
		{
			if (string.IsNullOrWhiteSpace(apiKeyHeaderName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(apiKeyHeaderName));
			}

			_ApiKeyHeaderName = apiKeyHeaderName;
		}

		/// <inheritdoc cref="IApiKeyParser.TryParseApiKey"/>
		public bool TryParseApiKey(HttpRequest request, out Guid apiKey)
		{
			apiKey = default;
			return request.Headers.TryGetValue(_ApiKeyHeaderName, out var rawApiKey)
				   && Guid.TryParse(rawApiKey, out apiKey);
		}
	}
}
