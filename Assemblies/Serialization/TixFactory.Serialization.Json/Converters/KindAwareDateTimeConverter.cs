using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TixFactory.Serialization.Json
{
    /// <summary>
    /// Extended IsoDateTimeConverter which handles converting <see cref="DateTime"/>s to specified <see cref="DateTimeKind"/>.
    /// </summary>
    public class KindAwareDateTimeConverter : IsoDateTimeConverter
    {
        private const string _NotSupportedKindExceptionMessage = "DateTimeKind Unspecified is not supported.";
        private readonly DateTimeKind _TargetReadDateTimeKind;
        private readonly DateTimeKind _TargetWriteDateTimeKind;

        /// <summary>
        /// Initializes a new <see cref="KindAwareDateTimeConverter"/>.
        /// </summary>
        /// <remarks>
        /// Json reads and writes will be <see cref="DateTimeKind.Utc"/>.
        /// </remarks>
        public KindAwareDateTimeConverter()
            : this(DateTimeKind.Utc, DateTimeKind.Utc)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="KindAwareDateTimeConverter"/>.
        /// </summary>
        /// <param name="targetReadDateTimeKind">The <see cref="DateTimeKind"/> to parse (read) the Json <see cref="DateTime"/> as.</param>
        /// <param name="targetWriteDateTimeKind">The <see cref="DateTimeKind"/> to write the Json <see cref="DateTime"/> as.</param>
        /// <exception cref="ArgumentException">
        /// - <paramref name="targetReadDateTimeKind"/> is <see cref="DateTimeKind.Unspecified"/>.
        /// - <paramref name="targetWriteDateTimeKind"/> is <see cref="DateTimeKind.Unspecified"/>.
        /// </exception>
        public KindAwareDateTimeConverter(DateTimeKind targetReadDateTimeKind, DateTimeKind targetWriteDateTimeKind)
        {
            if (targetReadDateTimeKind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException(_NotSupportedKindExceptionMessage, nameof(targetReadDateTimeKind));
            }

            if (targetWriteDateTimeKind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException(_NotSupportedKindExceptionMessage, nameof(targetWriteDateTimeKind));
            }

            _TargetReadDateTimeKind = targetReadDateTimeKind;
            _TargetWriteDateTimeKind = targetWriteDateTimeKind;
        }

        /// <inheritdoc cref="IsoDateTimeConverter.ReadJson"/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = base.ReadJson(reader, objectType, existingValue, serializer);

            if (result != null && result is DateTime resultDateTime)
            {
                return _TargetReadDateTimeKind == DateTimeKind.Utc
                    ? TranslateToUtc(resultDateTime)
                    : TranslateToLocal(resultDateTime);

            }

            return result;
        }

        /// <inheritdoc cref="IsoDateTimeConverter.WriteJson"/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null && value is DateTime dateTimeToWrite)
            {
                dateTimeToWrite = _TargetWriteDateTimeKind == DateTimeKind.Utc ? TranslateToUtc(dateTimeToWrite) : TranslateToLocal(dateTimeToWrite);
                base.WriteJson(writer, dateTimeToWrite, serializer);
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }

        private DateTime TranslateToUtc(DateTime dateTime)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Utc:
                    return dateTime;
                case DateTimeKind.Unspecified:
                    return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
                default:
                    return dateTime.ToUniversalTime();
            }
        }

        private DateTime TranslateToLocal(DateTime dateTime)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Local:
                    return dateTime;
                case DateTimeKind.Unspecified:
                    return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                default:
                    return dateTime.ToLocalTime();
            }
        }
    }
}
