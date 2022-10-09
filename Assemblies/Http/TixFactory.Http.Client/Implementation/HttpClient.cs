using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.CookieJar;

namespace TixFactory.Http.Client;

/// <inheritdoc cref="IHttpClient"/>
public class HttpClient : IHttpClient
{
    private bool _HandlersInitialized;

    /// <inheritdoc cref="IHttpClient.Handlers"/>
    public IList<IHttpClientHandler> Handlers { get; }

    /// <summary>
    /// Initializes a new <see cref="HttpClient"/>.
    /// </summary>
    /// <remarks>
    /// Adds the <see cref="CookieSaveHandler"/> to <see cref="Handlers"/> if <paramref name="cookieJar"/> is not <c>null</c>.
    /// </remarks>
    /// <param name="cookieJar">The <see cref="ICookieJar"/> to use for the requests.</param>
    /// <param name="httpClientSettings">The <see cref="IHttpClientSettings"/> (uses <see cref="HttpClientSettings"/> if <c>null</c>.)</param>
    public HttpClient(ICookieJar cookieJar = null, IHttpClientSettings httpClientSettings = null)
        : this(cookieJar?.CookieContainer, httpClientSettings ?? new HttpClientSettings())
    {
        if (cookieJar != null)
        {
            Handlers.Insert(0, new CookieSaveHandler(cookieJar));
        }
    }

    /// <summary>
    /// Initializes a new <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="cookieContainer">The <see cref="CookieContainer"/> to use for the requests.</param>
    /// <param name="httpClientSettings">The <see cref="IHttpClientSettings"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClientSettings"/></exception>
    public HttpClient(CookieContainer cookieContainer, IHttpClientSettings httpClientSettings)
    {
        if (httpClientSettings == null)
        {
            throw new ArgumentNullException(nameof(httpClientSettings));
        }

        Handlers = new List<IHttpClientHandler>();
        Handlers.Add(new SendHttpRequestHandler(cookieContainer, httpClientSettings));
    }

    /// <inheritdoc cref="IHttpClient.Send"/>
    public IHttpResponse Send(IHttpRequest request)
    {
        InitializeHandlers();

        var firstHandler = Handlers.First();
        return firstHandler.Invoke(request);
    }

    /// <inheritdoc cref="IHttpClient.SendAsync"/>
    public Task<IHttpResponse> SendAsync(IHttpRequest request, CancellationToken cancellationToken)
    {
        InitializeHandlers();

        var firstHandler = Handlers.First();
        return firstHandler.InvokeAsync(request, cancellationToken);
    }

    private void InitializeHandlers()
    {
        if (_HandlersInitialized)
        {
            return;
        }

        for (var n = 1; n < Handlers.Count; n++)
        {
            var handler = Handlers.ElementAt(n - 1);
            if (handler is HttpClientHandlerBase handlerBase)
            {
                handlerBase.NextHandler = Handlers.ElementAt(n);
            }
        }

        _HandlersInitialized = true;
    }
}
