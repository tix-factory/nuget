using Microsoft.Extensions.Logging.Console;

namespace TixFactory.Http.Service;

/// <summary>
/// Logger formatter options for the application.
/// </summary>
/// <remarks>
/// Set Logging.Console.FormatterName = "tix-factory-json" to use the custom formatter.
/// </remarks>
public class ServiceLoggingFormatterOptions : ConsoleFormatterOptions
{
}
