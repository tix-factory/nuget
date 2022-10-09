using System;

namespace TixFactory.Http.Client;

/// <inheritdoc cref="IHttpClientSettings"/>
public class HttpClientSettings : IHttpClientSettings
{
    /// <inheritdoc cref="IHttpClientSettings.UserAgent"/>
    public string UserAgent { get; set; } = "TixFactory.Http.Client";

    /// <inheritdoc cref="IHttpClientSettings.RequestTimeout"/>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(2);

    /// <inheritdoc cref="IHttpClientSettings.MaxRedirects"/>
    public int MaxRedirects { get; set; } = 20;
}
