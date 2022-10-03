using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TixFactory.Http.Server
{
	/// <summary>
	/// Middleware for making sure the only requests that are accepted come locally and responds with an <see cref="HttpStatusCode.Unauthorized"/> if it's not.
	/// </summary>
	public class LocalRequestOnlyHandler
	{
		private readonly RequestDelegate _NextHandler;

		/// <summary>
		/// Initializes a new <see cref="UnhandledExceptionHandler"/>.
		/// </summary>
		/// <param name="nextHandler">A delegate for triggering the next handler.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="nextHandler"/>
		/// </exception>
		public LocalRequestOnlyHandler(RequestDelegate nextHandler)
		{
			_NextHandler = nextHandler ?? throw new ArgumentNullException(nameof(nextHandler));
		}

		/// <summary>
		/// The method to invoke the handler.
		/// </summary>
		/// <param name="context">An <see cref="HttpContext"/>.</param>
		public Task Invoke(HttpContext context)
		{
			if (!context.Request.IsLocal())
			{
				var jsonBytes = Encoding.UTF8.GetBytes("{}");
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				context.Response.ContentType = "application/json";
				return context.Response.Body.WriteAsync(jsonBytes, 0, jsonBytes.Length);
			}

			return _NextHandler(context);
		}
	}
}
