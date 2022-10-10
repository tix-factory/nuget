using System;
using System.Runtime.Serialization;

namespace TixFactory.Http.Service;

/// <summary>
/// Class for serializing log information.
/// </summary>
/// <remarks>
/// Thin for now.. more information in the future.
/// </remarks>
[DataContract]
internal class ServiceLog
{
    /// <summary>
    /// The log message.
    /// </summary>
    /// <remarks>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-base.html#field-message
    /// </remarks>
    [DataMember(Name = "message")]
    public string Message { get; set; }

    /// <summary>
    /// The logger that logged the message.
    /// </summary>
    /// <remarks>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-log.html#field-log-logger
    /// </remarks>
    [DataMember(Name = "log.logger", EmitDefaultValue = false)]
    public string Category { get; set; }

    /// <summary>
    /// The log level.
    /// </summary>
    /// <remarks>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-log.html#field-log-level
    /// </remarks>
    [DataMember(Name = "log.level")]
    public string LogLevel { get; set; }

    /// <summary>
    /// The <see cref="Exception.Message"/>.
    /// </summary>
    /// <remarks>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-error.html#field-error-message
    /// </remarks>
    [DataMember(Name = "error.message", EmitDefaultValue = false)]
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// The <see cref="Exception"/> type.
    /// </summary>
    /// <remarks>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-error.html#field-error-type
    /// </remarks>
    [DataMember(Name = "error.type", EmitDefaultValue = false)]
    public string ExceptionType { get; set; }

    /// <summary>
    /// The serialized exception with its stack trace.
    /// </summary>
    /// <remarks>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-error.html#field-error-stack-trace
    /// </remarks>
    [DataMember(Name = "error.stack_trace", EmitDefaultValue = false)]
    public string ExceptionStackTrace { get; set; }

    /// <summary>
    /// When the log was logged.
    /// </summary>
    /// <remarks>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-base.html#field-timestamp
    /// </remarks>
    [DataMember(Name = "@timestamp")]
    public DateTime Created { get; } = DateTime.UtcNow;
}
