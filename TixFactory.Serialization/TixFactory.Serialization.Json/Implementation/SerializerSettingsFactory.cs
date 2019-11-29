using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TixFactory.Serialization.Json
{
	/// <inheritdoc cref="ISerializerSettingsFactory"/>
	public class SerializerSettingsFactory : ISerializerSettingsFactory
	{
		private readonly IList<JsonConverter> _GlobalConverters;

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

			_GlobalConverters = globalConvertersList;
		}

		/// <inheritdoc cref="ISerializerSettingsFactory.GetJsonSerializerSettings"/>
		public JsonSerializerSettings GetJsonSerializerSettings()
		{
			JsonSerializerSettings serializerSettings = null;

			if (JsonConvert.DefaultSettings != null)
			{
				serializerSettings = JsonConvert.DefaultSettings();
			}

			if (serializerSettings == null)
			{
				serializerSettings = new JsonSerializerSettings();
			}

			serializerSettings.Converters = serializerSettings.Converters ?? new List<JsonConverter>();

			foreach (var converter in _GlobalConverters)
			{
				if (!HasConverter(serializerSettings.Converters, converter.GetType()))
				{
					serializerSettings.Converters.Add(converter);
				}
			}

			return serializerSettings;
		}

		private bool HasConverter(ICollection<JsonConverter> converters, Type converterType)
		{
			return converters.Any(c => c.GetType() == converterType);
		}
	}
}
