using System.Runtime.Serialization;

namespace TixFactory.Http.Server
{
    /// <summary>
    /// A response model for the <see cref="HealthCheckController.CheckHealth"/> endpoint.
    /// </summary>
    [DataContract]
    public class HealthCheckResponse
    {
        /// <summary>
        /// The application status.
        /// </summary>
        [DataMember(Name = "status")]
        public string Status { get; set; } = "Ok";
    }
}
