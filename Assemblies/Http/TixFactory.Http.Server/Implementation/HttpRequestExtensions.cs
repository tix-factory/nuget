using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace TixFactory.Http.Server
{
    /// <summary>
    /// Extension methods for <see cref="HttpRequest"/>.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Whether or not an <see cref="HttpRequest"/> is coming in locally.
        /// </summary>
        /// <remarks>
        /// https://www.strathweb.com/2016/04/request-islocal-in-asp-net-core/
        /// </remarks>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <returns><c>true</c> if the request is sent the same machine as the server.</returns>
        public static bool IsLocal(this HttpRequest request)
        {
            if (Debugger.IsAttached)
            {
                // If we have the access to debug the application, we may not be
                // local - but we sure pass any checks anybody will be needing this
                // method for anyway.
                return true;
            }

            var connection = request.HttpContext.Connection;

            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }

                return IsLoopbackIpAddress(connection.RemoteIpAddress);
            }

            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Whether or not an <see cref="IPAddress"/> is a loopback IP address.
        /// </summary>
        /// <remarks>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.isloopback
        /// </remarks>
        /// <param name="ipAddress">The <see cref="IPAddress"/>.</param>
        /// <returns><c>true</c> if the address is a loopback IP address.</returns>
        private static bool IsLoopbackIpAddress(IPAddress ipAddress)
        {
            if (!IPAddress.IsLoopback(ipAddress))
            {
                return false;
            }

            return ipAddress.AddressFamily == AddressFamily.InterNetwork
                || ipAddress.AddressFamily == AddressFamily.InterNetworkV6;
        }
    }
}
