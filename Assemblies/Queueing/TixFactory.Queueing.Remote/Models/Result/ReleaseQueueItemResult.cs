namespace TixFactory.Queueing.Remote
{
	internal enum ReleaseQueueItemResult
	{
		Unknown = 0,
		Released = 1,
		Removed = 2,
		InvalidLeaseHolder = 3,
	}
}
