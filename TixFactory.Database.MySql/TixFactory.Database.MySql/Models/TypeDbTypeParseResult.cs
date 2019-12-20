using MySql.Data.MySqlClient;
using System;
using System.Text.RegularExpressions;

namespace TixFactory.Database.MySql
{
	/// <summary>
	/// Represents a database type and its mapping to a <see cref="Type"/>.
	/// </summary>
	/// <remarks>
	/// - https://dev.mysql.com/doc/refman/8.0/en/numeric-type-overview.html
	/// - https://dev.mysql.com/doc/refman/8.0/en/date-and-time-types.html
	/// - https://dev.mysql.com/doc/refman/8.0/en/string-types.html
	/// - https://dev.mysql.com/doc/refman/8.0/en/json.html
	/// </remarks>
	public class TypeDbTypeParseResult
	{
		private const string _UnsignedSuffix = " unsigned";
		private static readonly Regex _TypeParse = new Regex(@"^([^\(]+)\(?(\d*)\)?");

		/// <summary>
		/// The database type name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The length.
		/// </summary>
		/// <example>
		/// BIGINT(20) -> 20
		/// </example>
		public int? Length { get; }

		/// <summary>
		/// The <see cref="Type"/>.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// The <see cref="MySqlDbType"/>.
		/// </summary>
		public MySqlDbType MySqlType { get; }

		/// <summary>
		/// Whether or not the DB type is nullable.
		/// </summary>
		public bool Nullable { get; }

		/// <summary>
		/// Whether or not the DB type is unsigned.
		/// </summary>
		public bool Unsigned { get; }

		/// <summary>
		/// Initializes a new <see cref="TypeDbTypeParseResult"/>.
		/// </summary>
		/// <param name="rawValue">The raw DB value.</param>
		/// <param name="nullable">Whether or not the DB value is nullable.</param>
		public TypeDbTypeParseResult(string rawValue, bool nullable)
		{
			rawValue = rawValue?.Trim();
			var typeParseMatch = _TypeParse.Match(rawValue ?? string.Empty);
			if (!typeParseMatch.Success)
			{
				throw new ArgumentException($"Could not parse type from '{nameof(rawValue)}': '{rawValue}'", nameof(rawValue));
			}

			Name = typeParseMatch.Groups[1].ToString();
			Nullable = nullable;
			Unsigned = rawValue.EndsWith(_UnsignedSuffix);

			if (int.TryParse(typeParseMatch.Groups[2].ToString(), out var length))
			{
				Length = length;
			}

			switch (Name.ToUpper())
			{
				case "BIT":
				case "BOOL":
				case "BOOLEAN":
					Type = nullable ? typeof(bool?) : typeof(bool);
					MySqlType = MySqlDbType.Bit;
					break;
				case "TINYINT":
					if (Unsigned)
					{
						Type = nullable ? typeof(byte?) : typeof(byte);
						MySqlType = MySqlDbType.UByte;
					}
					else
					{
						Type = nullable ? typeof(sbyte?) : typeof(sbyte);
						MySqlType = MySqlDbType.Byte;
					}

					break;
				case "SMALLINT":
					if (Unsigned)
					{
						Type = nullable ? typeof(ushort?) : typeof(ushort);
						MySqlType = MySqlDbType.UInt16;
					}
					else
					{
						Type = nullable ? typeof(short?) : typeof(short);
						MySqlType = MySqlDbType.Int16;
					}

					break;
				case "MEDIUMINT":
					if (Unsigned)
					{
						Type = nullable ? typeof(uint?) : typeof(uint);
						MySqlType = MySqlDbType.UInt24;
					}
					else
					{
						Type = nullable ? typeof(int?) : typeof(int);
						MySqlType = MySqlDbType.Int24;
					}

					break;
				case "INT":
				case "INTEGER":
					if (Unsigned)
					{
						Type = nullable ? typeof(uint?) : typeof(uint);
						MySqlType = MySqlDbType.UInt32;
					}
					else
					{
						Type = nullable ? typeof(int?) : typeof(int);
						MySqlType = MySqlDbType.Int32;
					}

					break;
				case "BIGINT":
					Type = nullable ? typeof(long?) : typeof(long);
					MySqlType = MySqlDbType.Int64;
					break;
				case "DEC":
				case "DECIMAL":
					Type = nullable ? typeof(decimal?) : typeof(decimal);
					MySqlType = MySqlDbType.Decimal; // There is no unsigned decimal in C# or MySqlDbType..

					break;
				case "FLOAT":
					Type = nullable ? typeof(float?) : typeof(float);
					MySqlType = MySqlDbType.Float; // There is no unsigned float in C# or MySqlDbType..

					break;
				case "DOUBLE":
					Type = nullable ? typeof(double?) : typeof(double);
					MySqlType = MySqlDbType.Double; // There is no unsigned double in C# or MySqlDbType..

					break;
				case "DATE":
					Type = nullable ? typeof(DateTime?) : typeof(DateTime);
					MySqlType = MySqlDbType.Date;

					break;
				case "TIME":
					Type = nullable ? typeof(TimeSpan?) : typeof(TimeSpan);
					MySqlType = MySqlDbType.Time;

					break;
				case "DATETIME":
					Type = nullable ? typeof(DateTime?) : typeof(DateTime);
					MySqlType = MySqlDbType.DateTime;

					break;
				case "TIMESTAMP":
					Type = nullable ? typeof(DateTime?) : typeof(DateTime);
					MySqlType = MySqlDbType.Timestamp;

					break;
				case "YEAR":
					Type = nullable ? typeof(int?) : typeof(int); // int for convenience? though it would fit in a short..
					MySqlType = MySqlDbType.Year;

					break;
				case "CHAR":
					Type = typeof(string);
					throw new NotImplementedException($"'CHAR' type not supported: does not exist in {nameof(MySqlDbType)}.");
				case "VARCHAR":
					Type = typeof(string);
					MySqlType = MySqlDbType.VarChar;

					break;
				case "TINYBLOB":
					Type = typeof(string);
					MySqlType = MySqlDbType.TinyBlob;

					break;
				case "BLOB":
					Type = typeof(string);
					MySqlType = MySqlDbType.Blob;

					break;
				case "MEDIUMBLOB":
					Type = typeof(string);
					MySqlType = MySqlDbType.MediumBlob;

					break;
				case "LONGBLOB":
					Type = typeof(string);
					MySqlType = MySqlDbType.LongBlob;

					break;
				case "TINYTEXT":
					Type = typeof(string);
					MySqlType = MySqlDbType.TinyText;

					break;
				case "TEXT":
					Type = typeof(string);
					MySqlType = MySqlDbType.Text;

					break;
				case "MEDIUMTEXT":
					Type = typeof(string);
					MySqlType = MySqlDbType.MediumText;

					break;
				case "LONGTEXT":
					Type = typeof(string);
					MySqlType = MySqlDbType.LongText;

					break;
				case "ENUM":
					Type = typeof(string);
					MySqlType = MySqlDbType.Enum;

					break;
				case "SET":
					Type = typeof(string);
					MySqlType = MySqlDbType.Set;

					break;
				case "BINARY":
					Type = typeof(string);
					MySqlType = MySqlDbType.Binary;

					break;
				case "VARBINARY":
					Type = typeof(string);
					MySqlType = MySqlDbType.VarBinary;

					break;
				case "JSON":
					Type = typeof(object);
					MySqlType = MySqlDbType.JSON;

					break;
				default:
					throw new NotImplementedException($"Unsupported type: {rawValue}");
			}
		}
	}
}
