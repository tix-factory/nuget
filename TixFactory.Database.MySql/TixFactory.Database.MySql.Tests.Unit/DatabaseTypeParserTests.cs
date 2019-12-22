using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TixFactory.Database.MySql.Tests.Unit
{
	[TestFixture, ExcludeFromCodeCoverage]
	public class DatabaseTypeParserTests
	{
		private DatabaseTypeParser _DatabaseTypeParser;

		[SetUp]
		public void SetUp()
		{
			_DatabaseTypeParser = new DatabaseTypeParser();
		}

		[TestCase(typeof(byte), ExpectedResult = MySqlDbType.UByte)]
		[TestCase(typeof(byte?), ExpectedResult = MySqlDbType.UByte)]
		[TestCase(typeof(sbyte), ExpectedResult = MySqlDbType.Byte)]
		[TestCase(typeof(sbyte?), ExpectedResult = MySqlDbType.Byte)]
		[TestCase(typeof(short), ExpectedResult = MySqlDbType.Int16)]
		[TestCase(typeof(short?), ExpectedResult = MySqlDbType.Int16)]
		[TestCase(typeof(ushort), ExpectedResult = MySqlDbType.UInt16)]
		[TestCase(typeof(ushort?), ExpectedResult = MySqlDbType.UInt16)]
		[TestCase(typeof(int), ExpectedResult = MySqlDbType.Int32)]
		[TestCase(typeof(int?), ExpectedResult = MySqlDbType.Int32)]
		[TestCase(typeof(uint), ExpectedResult = MySqlDbType.UInt32)]
		[TestCase(typeof(uint?), ExpectedResult = MySqlDbType.UInt32)]
		[TestCase(typeof(long), ExpectedResult = MySqlDbType.Int64)]
		[TestCase(typeof(long?), ExpectedResult = MySqlDbType.Int64)]
		[TestCase(typeof(ulong), ExpectedResult = MySqlDbType.UInt64)]
		[TestCase(typeof(ulong?), ExpectedResult = MySqlDbType.UInt64)]
		[TestCase(typeof(float), ExpectedResult = MySqlDbType.Float)]
		[TestCase(typeof(float?), ExpectedResult = MySqlDbType.Float)]
		[TestCase(typeof(decimal), ExpectedResult = MySqlDbType.Decimal)]
		[TestCase(typeof(decimal?), ExpectedResult = MySqlDbType.Decimal)]
		[TestCase(typeof(double), ExpectedResult = MySqlDbType.Double)]
		[TestCase(typeof(double?), ExpectedResult = MySqlDbType.Double)]
		[TestCase(typeof(string), ExpectedResult = MySqlDbType.VarBinary)]
		[TestCase(typeof(object), ExpectedResult = MySqlDbType.JSON)]
		[TestCase(typeof(bool), ExpectedResult = MySqlDbType.Bit)]
		[TestCase(typeof(bool?), ExpectedResult = MySqlDbType.Bit)]
		[TestCase(typeof(DateTime), ExpectedResult = MySqlDbType.DateTime)]
		[TestCase(typeof(DateTime?), ExpectedResult = MySqlDbType.DateTime)]
		[TestCase(typeof(TimeSpan), ExpectedResult = MySqlDbType.Time)]
		[TestCase(typeof(TimeSpan?), ExpectedResult = MySqlDbType.Time)]
		public MySqlDbType GetMySqlType(Type type)
		{
			return _DatabaseTypeParser.GetMySqlType(type);
		}
	}
}
