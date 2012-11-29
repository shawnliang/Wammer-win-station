using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Notify
{
	public interface INotifyChannels
	{
		void NotifyToUserChannels(string user_id, string exceptSessionToken);
		void NotifyToAllChannels(string exceptSessionToken);
	}
}
