namespace TixFactory.Processors.Queueing
{
	public interface IQueueItemHandler<TItem>
	{
		bool ProcessItem(TItem item);
	}
}
