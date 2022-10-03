using System.Reflection;

namespace TixFactory.ApplicationContext
{
    /// <summary>
    /// Context about the running application.
    /// </summary>
    public interface IApplicationContext
    {
        /// <summary>
        /// The application assembly name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The running application <see cref="Assembly"/>.
        /// </summary>
        Assembly Assembly { get; }
    }
}
