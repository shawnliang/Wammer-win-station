
namespace Wammer.Station.Notify
{
	public interface INotifyChannels
	{
		void NotifyToUserChannels(string user_id, string exceptSessionToken);
		void NotifyToAllChannels(string exceptSessionToken);
	}
}
