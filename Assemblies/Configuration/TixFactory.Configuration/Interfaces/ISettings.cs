using System;

namespace TixFactory.Configuration
{
    /// <summary>
    /// The base interface for a settings interfaces implemented via <see cref="ISettingsInitializer"/>.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Creates an <see cref="ISetting{T}"/> instance from the interface properties.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="settingName">The property name.</param>
        /// <returns>The <see cref="ISetting{T}"/>.</returns>
        /// <exception cref="ArgumentException">
        /// - <paramref name="settingName"/> does not match property name on interface.
        /// - <typeparamref name="T"/> does not match property type.
        /// </exception>
        ISetting<T> ExtractSetting<T>(string settingName);
    }
}
