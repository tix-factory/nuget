using System;

namespace TixFactory.Configuration
{
    /// <summary>
    /// Initializes settings instances.
    /// </summary>
    public interface ISettingsInitializer
    {
        /// <summary>
        /// Creates an instance of the specified interface that populates its properties with the setting values.
        /// </summary>
        /// <typeparam name="TSettingsInterface">The interface type.</typeparam>
        /// <returns>The implementated interface.</returns>
        /// <exception cref="ArgumentException"><typeparamref name="TSettingsInterface"/> is not an interface.</exception>
        TSettingsInterface CreateFromInterface<TSettingsInterface>()
            where TSettingsInterface : ISettings;
    }
}
