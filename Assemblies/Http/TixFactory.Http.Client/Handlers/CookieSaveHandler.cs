using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.CookieJar;

namespace TixFactory.Http.Client;

/// <summary>
/// An <see cref="IHttpClientHandler"/> to save cookies after the request.
/// </summary>
/// <seealso cref="IHttpClientHandler"/>
public class CookieSaveHandler : HttpClientHandlerBase
{
    private readonly ICookieJar _CookieJar;

    /// <summary>
    /// Initializes a new <see cref="CookieSaveHandler"/>.
    /// </summary>
    /// <param name="cookieJar">The <see cref="ICookieJar"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="cookieJar"/></exception>
    public CookieSaveHandler(ICookieJar cookieJar)
    {
        _CookieJar = cookieJar ?? throw new ArgumentNullException(nameof(cookieJar));
    }

    /// <inheritdoc cref="HttpClientHandlerBase.Invoke"/>
    public override IHttpResponse Invoke(IHttpRequest request)
    {
        var response = base.Invoke(request);

        if (response.Headers.Keys.Contains(HttpResponseHeaderName.SetCookie))
        {
            _CookieJar.Save();
        }

        return response;
    }

    /// <inheritdoc cref="HttpClientHandlerBase.InvokeAsync"/>
    public override async Task<IHttpResponse> InvokeAsync(IHttpRequest request, CancellationToken cancellationToken)
    {
        var response = await base.InvokeAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.Headers.Keys.Contains(HttpResponseHeaderName.SetCookie))
        {
            _CookieJar.Save();
        }

        return response;
    }
}
