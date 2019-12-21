using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	/// <remarks>
	/// Documentation in remarks from: https://dev.mysql.com/doc/refman/8.0/en/show-index.html
	/// </remarks>
	[DataContract]
	internal class ShowIndexResult
	{
		/// <remarks>
		/// The name of the table.
		/// </remarks>
		[DataMember(Name = "Table")]
		public string TableName { get; set; }

		/// <summary>
		/// Exactly the opposite of what you'd expect. A reverse field.
		/// </summary>
		/// <remarks>
		/// 0 if the index cannot contain duplicates, 1 if it can.
		/// </remarks>
		[DataMember(Name = "Non_unique")]
		public bool NonUnique { get; set; }

		/// <remarks>
		/// The name of the index. If the index is the primary key, the name is always PRIMARY.
		/// </remarks>
		[DataMember(Name = "Key_name")]
		public string IndexName { get; set; }

		/// <remarks>
		/// The column sequence number in the index, starting with 1.
		/// </remarks>
		[DataMember(Name = "Seq_in_index")]
		public int Order { get; set; }

		/// <remarks>
		/// The column name.
		/// </remarks>
		[DataMember(Name = "Column_name")]
		public string ColumnName { get; set; }

		/// <remarks>
		/// How the column is sorted in the index. This can have values A (ascending), D (descending), or NULL (not sorted).
		/// </remarks>
		[DataMember(Name = "Collation")]
		public string Collation { get; set; }

		/// <remarks>
		/// An estimate of the number of unique values in the index. To update this number, run ANALYZE TABLE or (for MyISAM tables) myisamchk -a.
		///
		/// Cardinality is counted based on statistics stored as integers, so the value is not necessarily exact even for small tables.The higher the cardinality, the greater the chance that MySQL uses the index when doing joins.
		/// </remarks>
		[DataMember(Name = "Cardinality")]
		public int Cardinality { get; set; }

		/// <remarks>
		/// The index prefix. That is, the number of indexed characters if the column is only partly indexed, NULL if the entire column is indexed.
		///
		/// *Note*
		/// Prefix limits are measured in bytes.However, prefix lengths for index specifications in CREATE TABLE, ALTER TABLE, and CREATE INDEX statements
		/// are interpreted as number of characters for nonbinary string types(CHAR, VARCHAR, TEXT) and number of bytes for binary string types(BINARY, VARBINARY, BLOB).
		/// Take this into account when specifying a prefix length for a nonbinary string column that uses a multibyte character set.
		/// </remarks>
		[DataMember(Name = "Sub_part")]
		public string SubPart { get; set; }

		/// <remarks>
		/// Indicates how the key is packed. NULL if it is not.
		/// </remarks>
		[DataMember(Name = "Packed")]
		public string Packed { get; set; }

		/// <remarks>
		/// Contains YES if the column may contain NULL values and '' if not.
		/// </remarks>
		[DataMember(Name = "Null")]
		public string Null { get; set; }

		/// <remarks>
		/// The name of the table.
		/// </remarks>
		[DataMember(Name = "Index_type")]
		public string IndexType { get; set; }

		/// <remarks>
		/// Information about the index not described in its own column, such as disabled if the index is disabled.
		/// </remarks>
		[DataMember(Name = "Comment")]
		public string Comment { get; set; }

		/// <remarks>
		/// Any comment provided for the index with a COMMENT attribute when the index was created.
		/// </remarks>
		[DataMember(Name = "Index_comment")]
		public string IndexComment { get; set; }
	}
}
