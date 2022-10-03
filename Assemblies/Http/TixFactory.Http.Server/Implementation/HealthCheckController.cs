using Microsoft.AspNetCore.Mvc;

namespace TixFactory.Http.Server
{
    /// <summary>
    /// A default controller for verifying the application is up and running.
    /// </summary>
    public class HealthCheckController : Controller
    {
        /// <summary>
        /// For verification the application is started and responding to requests.
        /// </summary>
        /// <returns>A <see cref="HealthCheckResponse"/>.</returns>
        [Route(""), Route("CheckHealth")]
        [AcceptVerbs("GET")]
        public HealthCheckResponse CheckHealth()
        {
            return new HealthCheckResponse();
        }
    }
}
