﻿using System;
using System.Runtime.Serialization;

namespace TixFactory.Queueing.Remote
{
	/// <summary>
	/// The response for leasing an item from a remote queue.
	/// </summary>
	[DataContract]
	public class QueueItemResult
	{
		/// <summary>
		/// The ID of the queue item.
		/// </summary>
		[DataMember(Name = "id")]
		public long Id { get; set; }

		/// <summary>
		/// The queued data.
		/// </summary>
		[DataMember(Name = "data")]
		public string Data { get; set; }

		/// <summary>
		/// The lease ID.
		/// </summary>
		[DataMember(Name = "leaseId")]
		public Guid LeaseId { get; set; }
	}
}
