using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Notify
{
	public class NotifyChannelEventArgs : EventArgs
	{
		public INotifyChannel Channel { get; private set; }

		public NotifyChannelEventArgs(INotifyChannel channel)
		{
			this.Channel = channel;
		}
	}
}
