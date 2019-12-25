using System;
using Newtonsoft.Json;

namespace TixFactory.Serialization.Json
{
	/// <summary>
	/// A JSON converter for serializing byte arrays with base64.
	/// </summary>
	public class Base64Converter : JsonConverter
	{
		/// <inheritdoc cref="JsonConverter.WriteJson"/>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var writeBytes = GetBytes(value);

			if (writeBytes == null)
			{
				// TODO: Is this necessary? What happens if we write nothing?
				writer.WriteValue((string)null);
			}
			else
			{
				writer.WriteValue(Convert.ToBase64String(writeBytes));
			}
		}

		/// <inheritdoc cref="JsonConverter.ReadJson"/>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var readString = GetBase64String(existingValue ?? reader.Value);
			if (readString == null)
			{
				return null;
			}

			var readBytes = Convert.FromBase64String(readString);
			return System.Text.Encoding.UTF8.GetString(readBytes);
		}

		/// <inheritdoc cref="JsonConverter.CanConvert"/>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(string) 
			       || objectType == typeof(byte[]);
		}

		/// <summary>
		/// Gets the string from the <see cref="object"/> value.
		/// </summary>
		/// <remarks>
		/// For <see cref="ReadJson"/>
		/// TODO: Improve documentation.
		/// </remarks>
		/// <param name="value">The value.</param>
		/// <returns>The parsed string.</returns>
		protected virtual string GetBase64String(object value)
		{
			if (value == null)
			{
				return null;
			}

			if (value is byte[] valueBytes)
			{
				return System.Text.Encoding.UTF8.GetString(valueBytes);
			}
			
			if (value is string valueString)
			{
				return valueString;
			}

			throw new NotSupportedException($"Only byte[] and string are supported for reading with {nameof(Base64Converter)}.");
		}

		/// <summary>
		/// Gets the byte array from the <see cref="object"/> value.
		/// </summary>
		/// <remarks>
		/// For <see cref="WriteJson"/>
		/// TODO: Improve documentation.
		/// </remarks>
		/// <param name="value">The value.</param>
		/// <returns>The parsed bytes.</returns>
		protected virtual byte[] GetBytes(object value)
		{
			if (value == null)
			{
				return null;
			}

			if (value is byte[] valueBytes)
			{
				return valueBytes;
			}

			if (value is string valueString)
			{
				return System.Text.Encoding.UTF8.GetBytes(valueString);
			}

			throw new NotSupportedException($"Only byte[] and string are supported for writing with {nameof(Base64Converter)}.");
		}
	}
}
