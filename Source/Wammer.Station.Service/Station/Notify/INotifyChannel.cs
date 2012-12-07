
namespace Wammer.Station.Notify
{
	public interface INotifyChannel
	{
		string UserId { get; }
		string SessionToken { get; }
		string ApiKey { get; }

		void Notify();
		void Close(int close, string reason);
	}
}
