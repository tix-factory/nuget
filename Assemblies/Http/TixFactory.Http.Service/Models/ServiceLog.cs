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
    [DataMember(Name = "message")]
    public string Message { get; set; }

    [DataMember(Name = "category", EmitDefaultValue = false)]
    public string Category { get; set; }

    [DataMember(Name = "log.level")]
    public string LogLevel { get; set; }

    [DataMember(Name = "@timestamp")]
    public DateTime Created { get; } = DateTime.UtcNow;
}
