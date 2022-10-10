using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace TixFactory.Http.Service;

/// <summary>
/// A custom logger formatter for services.
/// </summary>
public class ServiceLoggingFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable _OptionsReloadToken;

    internal ServiceLoggingFormatterOptions FormatterOptions { get; private set; }

    /// <summary>
    /// Initializes a new <see cref="ServiceLoggingFormatter"/>.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{TOptions}"/> for <see cref="ServiceLoggingFormatterOptions"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="options"/>
    /// </exception>
    public ServiceLoggingFormatter(IOptionsMonitor<ServiceLoggingFormatterOptions> options)
        : base("tix-factory-json")
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        ReloadLoggerOptions(options.CurrentValue);
        _OptionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <inheritdoc cref="ConsoleFormatter.Write{TState}"/>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        if (logEntry.Formatter == null)
        {
            return;
        }

        var serializableLog = new ServiceLog
        {
            LogLevel = TranslateLogLevel(logEntry.LogLevel)
        };

        if (serializableLog.LogLevel == null)
        {
            return;
        }

        serializableLog.Message = logEntry.Formatter(logEntry.State, logEntry.Exception);

        if (string.IsNullOrWhiteSpace(serializableLog.Message))
        {
            serializableLog.Message = "[ Empty ]";

            if (logEntry.Exception == null)
            {
                // No message, no exception.
                // Nothing to log.
                return;
            }
        }

        if (logEntry.Exception != null)
        {
            if (!string.IsNullOrWhiteSpace(logEntry.Exception.Message))
            {
                serializableLog.ExceptionMessage = logEntry.Exception.Message;
            }

            var exceptionType = logEntry.Exception.GetType().FullName ?? string.Empty;
            serializableLog.ExceptionType = exceptionType;

            var serializedException = logEntry.Exception.ToString();
            var stackTraceStartIndex = exceptionType.Length + logEntry.Exception.Message.Length + 2;
            serializableLog.ExceptionStackTrace = serializedException.Length > stackTraceStartIndex ? serializedException[stackTraceStartIndex..].Trim() : serializedException;
        }

        if (!string.IsNullOrWhiteSpace(logEntry.Category))
        {
            serializableLog.Category = logEntry.Category;
        }

        textWriter.WriteLine(JsonConvert.SerializeObject(serializableLog));
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _OptionsReloadToken?.Dispose();
    }

    private static string TranslateLogLevel(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Critical:
            case LogLevel.Error:
                return "error";
            case LogLevel.Warning:
                return "warn";
            case LogLevel.Information:
                return "info";
            case LogLevel.Debug:
            case LogLevel.Trace:
                return "debug";
            default:
                return null;
        }
    }

    private void ReloadLoggerOptions(ServiceLoggingFormatterOptions options)
    {
        FormatterOptions = options;
    }
}
