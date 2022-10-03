using System;
using System.Collections.Concurrent;

namespace TixFactory.Configuration
{
    /// <summary>
    /// An <see cref="ISettingValueSource"/> that starts all values at default until overridden.
    /// </summary>
    /// <seealso cref="ISettingValueSource"/>
    public class BaselessSettingValueSource : ISettingValueSource
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> _SettingsGroups;

        /// <inheritdoc cref="ISettingValueSource.SettingValueChanged"/>
        public event Action<string, string> SettingValueChanged;

        static BaselessSettingValueSource()
        {
            _SettingsGroups = new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc cref="ISettingValueSource.TryGetSettingValue{T}"/>
        public bool TryGetSettingValue<T>(string groupName, string settingName, out T value)
        {
            value = default;

            if (!_SettingsGroups.TryGetValue(groupName, out var settings))
            {
                return false;
            }

            if (!settings.TryGetValue(settingName, out var rawValue))
            {
                return false;
            }

            if (rawValue is T parsedValue)
            {
                value = parsedValue;
                return true;
            }

            return false;
        }

        /// <inheritdoc cref="ISettingValueSource.WriteSettingValue{T}"/>
        public void WriteSettingValue<T>(string groupName, string settingName, T value)
        {
            var settingsGroup = _SettingsGroups.GetOrAdd(groupName, CreateSettingsGroup);
            settingsGroup[settingName] = value;

            SettingValueChanged?.Invoke(groupName, settingName);
        }

        private ConcurrentDictionary<string, object> CreateSettingsGroup(string groupName)
        {
            return new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
