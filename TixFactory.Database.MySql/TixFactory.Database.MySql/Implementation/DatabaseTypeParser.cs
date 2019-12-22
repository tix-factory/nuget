using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="IDatabaseTypeParser"/>
	public class DatabaseTypeParser : IDatabaseTypeParser
	{
		private const string _Bit = "BIT";
		private const string _Bool = "BOOL";
		private const string _Boolean = "BOOLEAN";
		private const string _TinyInt = "TINYINT";
		private const string _SmallInt = "SMALLINT";
		private const string _MediumInt = "MEDIUMINT";
		private const string _Int = "INT";
		private const string _Integer = "INTEGER";
		private const string _BigInt = "BIGINT";
		private const string _Dec = "DEC";
		private const string _Decimal = "DECIMAL";
		private const string _Float = "FLOAT";
		private const string _Double = "DOUBLE";
		private const string _Date = "DATE";
		private const string _Time = "TIME";
		private const string _DateTime = "DATETIME";
		private const string _TimeStamp = "TIMESTAMP";
		private const string _Year = "YEAR";
		private const string _Char = "CHAR";
		private const string _VarChar = "VARCHAR";
		private const string _TinyBlob = "TINYBLOB";
		private const string _Blob = "BLOB";
		private const string _MediumBlob = "MEDIUMBLOB";
		private const string _LongBlob = "LONGBLOB";
		private const string _TinyText = "TINYTEXT";
		private const string _Text = "TEXT";
		private const string _MediumText = "MEDIUMTEXT";
		private const string _LongText = "LONGTEXT";
		private const string _Enum = "ENUM";
		private const string _Set = "SET";
		private const string _Binary = "BINARY";
		private const string _VarBinary = "VARBINARY";
		private const string _Json = "JSON";
		private const string _UnsignedSuffix = " unsigned";
		private static readonly Regex _TypeParse = new Regex(@"^([^\(]+)\(?(\d*)\)?");
		private readonly IReadOnlyDictionary<MySqlDbType, string> _DatabaseTypeNamesByMySqlType;
		private readonly IReadOnlyDictionary<Type, MySqlDbType> _MySqlTypesByType;

		/// <summary>
		/// Initializes a new <see cref="DatabaseTypeParser"/>.
		/// </summary>
		public DatabaseTypeParser()
		{
			// ADD/REMOVE VALUES CAREFULLY.
			// The order of this array also decides the output GetMySqlType
			// e.g. string currently outputs VarBinary as the MySqlDbType because _VarBinary is the last item in this array that represents strings.
			// Prefer MySqlDbType.DateTime https://stackoverflow.com/a/45632196/1663648 (it will store dates after January 2038).
			// Prefer MySqlDbType.VarBinary for string: https://dev.mysql.com/doc/refman/8.0/en/binary-varbinary.html (stores in bytes rather than characters)
			var parseableTypes = new[]
			{
				_Bit,
				_Bool,
				_Boolean,
				_Date,
				_Time,
				_TimeStamp,
				_DateTime,
				_Year,
				_TinyInt,
				_SmallInt,
				_MediumInt,
				_Int,
				_Integer,
				_BigInt,
				_Dec,
				_Decimal,
				_Float,
				_Double,
				// TODO: _Char,
				_VarChar,
				_TinyBlob,
				_Blob,
				_MediumBlob,
				_LongBlob,
				_TinyText,
				_Text,
				_MediumText,
				_LongText,
				_Enum,
				_Set,
				_Binary,
				_VarBinary,
				_Json
			};
			
			var databaseTypeNamesByMySqlType = new Dictionary<MySqlDbType, string>();
			var mySqlTypesByType = new Dictionary<Type, MySqlDbType>();

			foreach (var type in parseableTypes)
			{
				var types = new[]
				{
					ParseDatabaseType(type, nullable: false),
					ParseDatabaseType(type, nullable: true),
					ParseDatabaseType($"{type}() {_UnsignedSuffix}", nullable: false),
					ParseDatabaseType($"{type}() {_UnsignedSuffix}", nullable: true)
				};

				foreach (var parsedType in types)
				{
					databaseTypeNamesByMySqlType[parsedType.MySqlType] = parsedType.Name;
					mySqlTypesByType[parsedType.Type] = parsedType.MySqlType;
				}
			}

			_DatabaseTypeNamesByMySqlType = databaseTypeNamesByMySqlType;
			_MySqlTypesByType = mySqlTypesByType;
		}

		/// <inheritdoc cref="IDatabaseTypeParser.ParseDatabaseType"/>
		public TypeDbTypeParseResult ParseDatabaseType(string databaseType, bool nullable)
		{
			databaseType = databaseType?.Trim();
			var typeParseMatch = _TypeParse.Match(databaseType ?? string.Empty);
			if (!typeParseMatch.Success)
			{
				throw new ArgumentException($"Could not parse type from '{nameof(databaseType)}': '{databaseType}'", nameof(databaseType));
			}
			
			var name = typeParseMatch.Groups[1].ToString().ToUpper();
			var unsigned = databaseType.EndsWith(_UnsignedSuffix);
			int? length = null;
			Type type;
			MySqlDbType mySqlType;

			if (int.TryParse(typeParseMatch.Groups[2].ToString(), out var parsedLength))
			{
				length = parsedLength;
			}

			switch (name)
			{
				case _Bit:
				case _Bool:
				case _Boolean:
					type = nullable ? typeof(bool?) : typeof(bool);
					mySqlType = MySqlDbType.Bit;
					break;
				case _TinyInt:
					if (unsigned)
					{
						type = nullable ? typeof(byte?) : typeof(byte);
						mySqlType = MySqlDbType.UByte;
					}
					else
					{
						type = nullable ? typeof(sbyte?) : typeof(sbyte);
						mySqlType = MySqlDbType.Byte;
					}

					break;
				case _SmallInt:
					if (unsigned)
					{
						type = nullable ? typeof(ushort?) : typeof(ushort);
						mySqlType = MySqlDbType.UInt16;
					}
					else
					{
						type = nullable ? typeof(short?) : typeof(short);
						mySqlType = MySqlDbType.Int16;
					}

					break;
				case _MediumInt:
					if (unsigned)
					{
						type = nullable ? typeof(uint?) : typeof(uint);
						mySqlType = MySqlDbType.UInt24;
					}
					else
					{
						type = nullable ? typeof(int?) : typeof(int);
						mySqlType = MySqlDbType.Int24;
					}

					break;
				case _Int:
				case _Integer:
					if (unsigned)
					{
						type = nullable ? typeof(uint?) : typeof(uint);
						mySqlType = MySqlDbType.UInt32;
					}
					else
					{
						type = nullable ? typeof(int?) : typeof(int);
						mySqlType = MySqlDbType.Int32;
					}

					break;
				case _BigInt:
					if (unsigned)
					{
						type = nullable ? typeof(ulong?) : typeof(ulong);
						mySqlType = MySqlDbType.UInt64;
					}
					else
					{
						type = nullable ? typeof(long?) : typeof(long);
						mySqlType = MySqlDbType.Int64;
					}

					break;
				case _Dec:
				case _Decimal:
					type = nullable ? typeof(decimal?) : typeof(decimal);
					mySqlType = MySqlDbType.Decimal; // There is no unsigned decimal in C# or MySqlDbType..

					break;
				case _Float:
					type = nullable ? typeof(float?) : typeof(float);
					mySqlType = MySqlDbType.Float; // There is no unsigned float in C# or MySqlDbType..

					break;
				case _Double:
					type = nullable ? typeof(double?) : typeof(double);
					mySqlType = MySqlDbType.Double; // There is no unsigned double in C# or MySqlDbType..

					break;
				case _Date:
					type = nullable ? typeof(DateTime?) : typeof(DateTime);
					mySqlType = MySqlDbType.Date;

					break;
				case _Time:
					type = nullable ? typeof(TimeSpan?) : typeof(TimeSpan);
					mySqlType = MySqlDbType.Time;

					break;
				case _DateTime:
					type = nullable ? typeof(DateTime?) : typeof(DateTime);
					mySqlType = MySqlDbType.DateTime;

					break;
				case _TimeStamp:
					type = nullable ? typeof(DateTime?) : typeof(DateTime);
					mySqlType = MySqlDbType.Timestamp;

					break;
				case _Year:
					type = nullable ? typeof(int?) : typeof(int); // int for convenience? though it would fit in a short..
					mySqlType = MySqlDbType.Year;

					break;
				case _Char:
					type = typeof(string);
					throw new NotImplementedException($"'{_Char}' type not supported: does not exist in {nameof(MySqlDbType)}.");
				case _VarChar:
					type = typeof(string);
					mySqlType = MySqlDbType.VarChar;

					break;
				case _TinyBlob:
					type = typeof(string);
					mySqlType = MySqlDbType.TinyBlob;

					break;
				case _Blob:
					type = typeof(string);
					mySqlType = MySqlDbType.Blob;

					break;
				case _MediumBlob:
					type = typeof(string);
					mySqlType = MySqlDbType.MediumBlob;

					break;
				case _LongBlob:
					type = typeof(string);
					mySqlType = MySqlDbType.LongBlob;

					break;
				case _TinyText:
					type = typeof(string);
					mySqlType = MySqlDbType.TinyText;

					break;
				case _Text:
					type = typeof(string);
					mySqlType = MySqlDbType.Text;

					break;
				case _MediumText:
					type = typeof(string);
					mySqlType = MySqlDbType.MediumText;

					break;
				case _LongText:
					type = typeof(string);
					mySqlType = MySqlDbType.LongText;

					break;
				case _Enum:
					type = typeof(string);
					mySqlType = MySqlDbType.Enum;

					break;
				case _Set:
					type = typeof(string);
					mySqlType = MySqlDbType.Set;

					break;
				case _Binary:
					type = typeof(string);
					mySqlType = MySqlDbType.Binary;

					break;
				case _VarBinary:
					type = typeof(string);
					mySqlType = MySqlDbType.VarBinary;

					break;
				case _Json:
					type = typeof(object);
					mySqlType = MySqlDbType.JSON;

					break;
				default:
					throw new NotImplementedException($"Unsupported type: {databaseType}");
			}

			return new TypeDbTypeParseResult(name, nullable, length, type, mySqlType, nullable);
		}

		/// <inheritdoc cref="IDatabaseTypeParser.GetDatabaseTypeName"/>
		public string GetDatabaseTypeName(MySqlDbType mySqlType)
		{
			if (!_DatabaseTypeNamesByMySqlType.TryGetValue(mySqlType, out var databaseTypeName))
			{
				throw new InvalidEnumArgumentException(nameof(mySqlType), (int) mySqlType, typeof(MySqlDbType));
			}

			return databaseTypeName;
		}

		/// <inheritdoc cref="IDatabaseTypeParser.GetMySqlType"/>
		public MySqlDbType GetMySqlType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (!_MySqlTypesByType.TryGetValue(type, out var mySqlType))
			{
				throw new ArgumentException($"{nameof(Type)} ({type.Name}) does not map to {nameof(MySqlDbType)}", nameof(type));
			}

			return mySqlType;
		}
	}
}
