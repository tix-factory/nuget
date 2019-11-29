using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using Newtonsoft.Json.Converters;
using TixFactory.Logging;
using TixFactory.Serialization.Json;

namespace TixFactory.Http.Server
{
	/// <summary>
	/// A base class for the application start up/entry.
	/// </summary>
	/// <remarks>
	/// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-3.0
	/// </remarks>
	public abstract class Startup
	{
		/// <summary>
		/// The application's <see cref="ILogger"/>.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// The application's <see cref="IOperationExecuter"/>.
		/// </summary>
		protected IOperationExecuter OperationExecuter { get; }

		/// <summary>
		/// Initializes a new <see cref="Startup"/>.
		/// </summary>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="logger"/>
		/// </exception>
		protected Startup(ILogger logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			OperationExecuter = new OperationExecuter();

			logger.Verbose($"Starting {GetType().Namespace}...");
		}

		/// <summary>
		/// Configures the <see cref="IServiceCollection"/> for the application.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		public virtual void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient(s => OperationExecuter);
			services.AddTransient(s => Logger);
			services.AddMvc(ConfigureMvc).AddJsonOptions(ConfigureJson);
		}

		/// <summary>
		/// Configures the <see cref="IApplicationBuilder"/> for the application.
		/// </summary>
		/// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
		/// <param name="env">The <see cref="IHostingEnvironment"/>.</param>
		public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseMiddleware<UnhandledExceptionHandler>(Logger);
			app.UseMvc();
		}

		/// <summary>
		/// Configures <see cref="MvcJsonOptions"/> for the application.
		/// </summary>
		/// <param name="options">The <see cref="MvcJsonOptions"/>.</param>
		protected virtual void ConfigureJson(MvcJsonOptions options)
		{
			options.SerializerSettings.Converters.Add(new KindAwareDateTimeConverter());
			options.SerializerSettings.Converters.Add(new StringEnumConverter());
		}

		/// <summary>
		/// Configures <see cref="MvcOptions"/> for the application.
		/// </summary>
		/// <param name="options">The <see cref="MvcOptions"/>.</param>
		protected virtual void ConfigureMvc(MvcOptions options)
		{
		}
	}
}
