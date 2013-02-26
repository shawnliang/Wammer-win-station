
namespace Wammer.Queue
{
	public interface IThreadPool
	{
		void QueueWorkItem(System.Threading.WaitCallback callback);
	}
}
