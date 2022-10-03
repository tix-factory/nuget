using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TixFactory.Http.Service
{
    /// <summary>
    /// This handler exists because Microsoft decided nullable DateTimes weren't worth supporting?
    /// </summary>
    internal class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var dateTime = DateTime.Parse(reader.GetString());
                    switch (dateTime.Kind)
                    {
                        case DateTimeKind.Local:
                            dateTime = dateTime.ToUniversalTime();
                            break;
                        case DateTimeKind.Unspecified:
                            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                            break;
                    }

                    return dateTime;
                case JsonTokenType.Null:
                    return null;
                default:
                    throw new NotSupportedException($"{nameof(NullableDateTimeConverter)}.{nameof(Read)}: Unsupported {nameof(JsonTokenType)} ({reader.TokenType})");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                var date = value.Value;
                switch (date.Kind)
                {
                    case DateTimeKind.Local:
                        date = date.ToUniversalTime();
                        break;
                    case DateTimeKind.Unspecified:
                        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                        break;
                }

                writer.WriteStringValue(date.ToString("o"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
