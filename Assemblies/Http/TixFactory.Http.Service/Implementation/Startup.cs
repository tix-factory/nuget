using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TixFactory.ApplicationContext;

namespace TixFactory.Http.Service
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
        /// The application's <see cref="Logging.ILogger"/>.
        /// </summary>
        protected Logging.ILogger Logger { get; }

        /// <summary>
        /// The application's <see cref="IOperationExecuter"/>.
        /// </summary>
        protected IOperationExecuter OperationExecuter { get; }

        /// <summary>
        /// The application's <see cref="IApplicationContext"/>.
        /// </summary>
        protected IApplicationContext ApplicationContext { get; }

        /// <summary>
        /// Initializes a new <see cref="Startup"/>.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="logger"/>
        /// </exception>
        protected Startup(Logging.ILogger logger)
            : this(logger, TixFactory.ApplicationContext.ApplicationContext.Singleton)
        {
            TixFactory.ApplicationContext.ApplicationContext.SetEntryClass(GetType());
        }

        /// <summary>
        /// Initializes a new <see cref="Startup"/>.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <param name="applicationContext">An <see cref="IApplicationContext"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="logger"/>
        /// - <paramref name="applicationContext"/>
        /// </exception>
        protected Startup(Logging.ILogger logger, IApplicationContext applicationContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            OperationExecuter = new OperationExecuter();

            logger.Verbose($"Starting {applicationContext.Name}...");
        }

        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> for the application.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(s => ApplicationContext);
            services.AddTransient(s => OperationExecuter);
            services.AddTransient(s => Logger);
            services.AddLogging(lb => lb.ClearProviders());
            services.AddControllers(ConfigureMvc).AddJsonOptions(ConfigureJson);
        }

        /// <summary>
        /// Configures the <see cref="IApplicationBuilder"/> for the application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">The <see cref="IHostingEnvironment"/>.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<UnhandledExceptionHandler>(Logger);
            app.UseRouting();
            app.UseEndpoints(ConfigureEndpoints);
        }

        /// <summary>
        /// Configures <see cref="JsonOptions"/> for the application.
        /// </summary>
        /// <param name="options">The <see cref="JsonOptions"/>.</param>
        protected virtual void ConfigureJson(JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
        }

        /// <summary>
        /// Configures <see cref="MvcOptions"/> for the application.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/>.</param>
        protected virtual void ConfigureMvc(MvcOptions options)
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
        }

        /// <summary>
        /// Configures endpoint routing for the application.
        /// </summary>
        /// <param name="endpointRouteBuilder">The <see cref="IEndpointRouteBuilder"/>.</param>
        protected virtual void ConfigureEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllers();
        }
    }
}
