namespace Wammer.Station.Retry
{
	public interface IRetryQueue
	{
		void Enqueue(IRetryTask task);
	}
}