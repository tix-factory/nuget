using Newtonsoft.Json;

namespace TixFactory.Serialization.Json
{
	/// <summary>
	/// A factory for Json serializer settings.
	/// </summary>
	public interface ISerializerSettingsFactory
	{
		/// <summary>
		/// Creates a new <see cref="JsonSerializerSettings"/>.
		/// </summary>
		/// <returns>The <see cref="JsonSerializerSettings"/>.</returns>
		JsonSerializerSettings GetJsonSerializerSettings();
	}
}
