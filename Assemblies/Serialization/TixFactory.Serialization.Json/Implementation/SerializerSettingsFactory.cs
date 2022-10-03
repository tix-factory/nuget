using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TixFactory.Serialization.Json
{
    /// <inheritdoc cref="ISerializerSettingsFactory"/>
    public class SerializerSettingsFactory : ISerializerSettingsFactory
    {
        private readonly JsonSerializerSettings _JsonSerializerSettings;

        /// <summary>
        /// Initializes a new <see cref="SerializerSettingsFactory"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="StringEnumConverter"/> and <see cref="KindAwareDateTimeConverter"/> are essentially added to <paramref name="globalConverters"/> by default.
        /// </remarks>
        /// <param name="globalConverters"><see cref="JsonConverter"/>s to add to the settings.</param>
        public SerializerSettingsFactory(params JsonConverter[] globalConverters)
        {
            var globalConvertersList = new List<JsonConverter>
            {
                new StringEnumConverter(),
                new KindAwareDateTimeConverter()
            };

            globalConvertersList.AddRange(globalConverters);

            JsonSerializerSettings serializerSettings = null;

            // TODO: Why does this throw a StackOverflowException when called from GetJsonSerializerSettings
            if (JsonConvert.DefaultSettings != null)
            {
                serializerSettings = JsonConvert.DefaultSettings();
            }

            if (serializerSettings == null)
            {
                serializerSettings = new JsonSerializerSettings();
            }

            serializerSettings.Converters = serializerSettings.Converters ?? new List<JsonConverter>();

            foreach (var converter in globalConvertersList)
            {
                if (!HasConverter(serializerSettings.Converters, converter.GetType()))
                {
                    serializerSettings.Converters.Add(converter);
                }
            }

            _JsonSerializerSettings = serializerSettings;
        }

        /// <inheritdoc cref="ISerializerSettingsFactory.GetJsonSerializerSettings"/>
        public JsonSerializerSettings GetJsonSerializerSettings()
        {
            return _JsonSerializerSettings;
        }

        private bool HasConverter(ICollection<JsonConverter> converters, Type converterType)
        {
            return converters.Any(c => c.GetType() == converterType);
        }
    }
}
