using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Notify
{
	public interface INotifyChannel
	{
		string UserId { get; }
		string SessionToken { get; }
		string ApiKey { get; }
	}
}
