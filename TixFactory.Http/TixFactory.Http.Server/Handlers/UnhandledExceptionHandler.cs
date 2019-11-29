using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TixFactory.Logging;
using TixFactory.Operations;

namespace TixFactory.Http.Server
{
	/// <summary>
	/// Middleware for logging unhandled exceptions and responding with <see cref="HttpStatusCode.InternalServerError"/>.
	/// </summary>
	public class UnhandledExceptionHandler
	{
		private readonly RequestDelegate _NextHandler;
		private readonly ILogger _Logger;

		/// <summary>
		/// Initializes a new <see cref="UnhandledExceptionHandler"/>.
		/// </summary>
		/// <param name="nextHandler">A delegate for triggering the next handler.</param>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="nextHandler"/>
		/// - <paramref name="logger"/>
		/// </exception>
		public UnhandledExceptionHandler(RequestDelegate nextHandler, ILogger logger)
		{
			_NextHandler = nextHandler ?? throw new ArgumentNullException(nameof(nextHandler));
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// The method to invoke the handler.
		/// </summary>
		/// <param name="context">An <see cref="HttpContext"/>.</param>
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _NextHandler(context).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				_Logger.Error(e);

				var responsePayload = new Payload<object>(null, new OperationError(InternalError.UnexpectedException));
				var json = JsonConvert.SerializeObject(responsePayload);
				var jsonBytes = Encoding.UTF8.GetBytes(json);

				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				context.Response.ContentType = "application/json";
				await context.Response.Body.WriteAsync(jsonBytes, 0, jsonBytes.Length).ConfigureAwait(false);
			}
		}
	}
}
