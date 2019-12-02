using System;
using System.Threading.Tasks;

namespace TixFactory.Queueing
{
	internal class RunningItem<TItem>
	{
		public TItem Item { get; set; }
		public DateTime Expiration { get; set; }
		public Task Task { get; set; }
	}
}