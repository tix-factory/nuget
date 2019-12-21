using System;
using System.Runtime.Serialization;

namespace TixFactory.Database.MySql
{
	/// <remarks>
	/// Documentation in remarks from: https://dev.mysql.com/doc/refman/8.0/en/show-procedure-status.html
	/// </remarks>
	[DataContract]
	internal class ShowProcedureStatusResult
	{
		[DataMember(Name = "Db")]
		public string DatabaseName { get; set; }

		[DataMember(Name = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "Type")]
		public string Type { get; set; }

		[DataMember(Name = "Definer")]
		public string Definer { get; set; }

		[DataMember(Name = "Modified")]
		public DateTime Modified { get; set; }

		[DataMember(Name = "Created")]
		public DateTime Created { get; set; }

		[DataMember(Name = "Security_type")]
		public string SecurityType { get; set; }

		[DataMember(Name = "Comment")]
		public string Comment { get; set; }

		/// <remarks>
		/// character_set_client is the session value of the character_set_client system variable when the routine was created.
		/// </remarks>
		[DataMember(Name = "character_set_client")]
		public string CharacterSetClient { get; set; }

		/// <remarks>
		/// collation_connection is the session value of the collation_connection system variable when the routine was created.
		/// </remarks>
		[DataMember(Name = "collation_connection")]
		public string CollationConnection { get; set; }

		/// <remarks>
		/// Database Collation is the collation of the database with which the routine is associated.
		/// </remarks>
		[DataMember(Name = "Database_Collation")]
		public string DatabaseCollation { get; set; }
	}
}
