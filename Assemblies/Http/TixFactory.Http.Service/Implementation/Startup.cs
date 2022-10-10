using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Prometheus;
using Prometheus.HttpMetrics;
using TixFactory.ApplicationContext;

namespace TixFactory.Http.Service;

/// <summary>
/// A base class for the application start up/entry.
/// </summary>
/// <remarks>
/// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-5.0
/// </remarks>
public abstract class Startup
{
    /// <summary>
    /// Configures the <see cref="IApplicationBuilder"/> for the application.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    /// <param name="env">The <see cref="IHostingEnvironment"/>.</param>
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseHttpMetrics(ConfigureMetrics);
        app.UseMiddleware<UnhandledExceptionMiddleware>();
        app.UseEndpoints(ConfigureEndpoints);
    }

    /// <summary>
    /// Configures the <see cref="IServiceCollection"/> for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IApplicationContext>(ApplicationContext.ApplicationContext.Singleton);
        services.AddSingleton<IOperationExecutor, OperationExecutor>();

        services.AddLogging(ConfigureLogging);

        services.AddControllers(ConfigureMvc)
            .AddNewtonsoftJson(ConfigureJson);
    }

    /// <summary>
    /// Intended to be called by the entry point for the service.
    /// </summary>
    /// <typeparam name="TStartup">The startup class.</typeparam>
    /// <param name="args">The command line arguments used to start the service.</param>
    public static Task Main<TStartup>(string[] args)
        where TStartup : class
    {
        var webHost = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webHostBuilder =>
        {
            webHostBuilder.UseStartup<TStartup>();
        }).Build();

        return webHost.RunAsync();
    }

    /// <summary>
    /// Configures <see cref="JsonOptions"/> for the application.
    /// </summary>
    /// <param name="options">The <see cref="JsonOptions"/>.</param>
    protected virtual void ConfigureJson(MvcNewtonsoftJsonOptions options)
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
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
        endpointRouteBuilder.MapMetrics();
        endpointRouteBuilder.MapControllers();
    }

    /// <summary>
    /// Configures how things are logged.
    /// </summary>
    /// <param name="loggingBuilder">The <see cref="ILoggingBuilder"/>.</param>
    protected virtual void ConfigureLogging(ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.AddConsoleFormatter<ServiceLoggingFormatter, ServiceLoggingFormatterOptions>();
    }

    /// <summary>
    /// Configure HTTP metrics exported to prometheus.
    /// </summary>
    /// <param name="options">The <see cref="HttpMiddlewareExporterOptions"/>.</param>
    protected virtual void ConfigureMetrics(HttpMiddlewareExporterOptions options)
    {
    }
}
