using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public interface IPersonalCloudStatusService
	{
		IEnumerable<StreamDevice> GetDeviceList(string user_id, string apikey, string session_token);
		PersonalCloudStatus GetStatus(string user_id);
	}
}
