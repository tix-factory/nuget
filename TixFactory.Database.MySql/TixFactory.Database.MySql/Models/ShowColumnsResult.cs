using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	/// <remarks>
	/// Documentation in remarks from: https://dev.mysql.com/doc/refman/8.0/en/show-columns.html
	/// </remarks>
	[DataContract]
	internal class ShowColumnsResult
	{
		/// <remarks>
		/// The name of the column.
		/// </remarks>
		[DataMember(Name = "Field")]
		public string Name { get; set; }

		/// <remarks>
		/// The column data type.
		/// </remarks>
		[DataMember(Name = "Type")]
		public string RawDataType { get; set; }

		/// <remarks>
		/// The column nullability. The value is YES if NULL values can be stored in the column, NO if not.
		/// </remarks>
		[DataMember(Name = "Null")]
		public string IsNullable { get; set; }

		/// <remarks>
		/// Whether the column is indexed:
		/// - If Key is empty, the column either is not indexed or is indexed only as a secondary column in a multiple-column, nonunique index.
		/// - If Key is PRI, the column is a PRIMARY KEY or is one of the columns in a multiple-column PRIMARY KEY.
		/// - If Key is UNI, the column is the first column of a UNIQUE index. (A UNIQUE index permits multiple NULL values, but you can tell whether the column permits NULL by checking the Null field.)
		///	- If Key is MUL, the column is the first column of a nonunique index in which multiple occurrences of a given value are permitted within the column.
		///
		/// If more than one of the Key values applies to a given column of a table, Key displays the one with the highest priority, in the order PRI, UNI, MUL.
		/// A UNIQUE index may be displayed as PRI if it cannot contain NULL values and there is no PRIMARY KEY in the table. A UNIQUE index may display as MUL if several
		/// columns form a composite UNIQUE index; although the combination of the columns is unique, each column can still hold multiple occurrences of a given value.
		/// </remarks>
		[DataMember(Name = "Key")]
		public string Key { get; set; }

		/// <remarks>
		/// The default value for the column. This is NULL if the column has an explicit default of NULL, or if the column definition includes no DEFAULT clause.
		/// </remarks>
		[DataMember(Name = "Default")]
		public object Default { get; set; }

		/// <remarks>
		/// Any additional information that is available about a given column. The value is nonempty in these cases:
		/// - auto_increment for columns that have the AUTO_INCREMENT attribute.
		/// - on update CURRENT_TIMESTAMP for TIMESTAMP or DATETIME columns that have the ON UPDATE CURRENT_TIMESTAMP attribute.
		/// - VIRTUAL GENERATED or VIRTUAL STORED for generated columns.
		/// - DEFAULT_GENERATED for columns that have an expression default value.
		/// </remarks>
		[DataMember(Name = "Extra")]
		public string Extra { get; set; }
	}
}
