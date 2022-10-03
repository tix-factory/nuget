using System;
using Microsoft.AspNetCore.Http;

namespace TixFactory.ApplicationAuthorization
{
    public interface IApiKeyParser
    {
        bool TryParseApiKey(HttpRequest request, out Guid apiKey);
    }
}
