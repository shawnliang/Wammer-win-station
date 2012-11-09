using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;
using WebSocketSharp;
using Wammer.Station.Notify;
using Wammer.Utility;
using Wammer.Station;

namespace StationSystemTray
{
	class PersonalCloudStatusService : IPersonalCloudStatus
	{
		public IEnumerable<PersonalCloudNode> GetNodes(string user_id, string session_token, string apikey)
		{
			yield return new PersonalCloudNode()
			{
				Name = "Stream Cloud",
				Id = Guid.Empty.ToString(),
				Profile = "Connected",
				Type = NodeType.Station 
			};

			var session = Wammer.Cloud.User.GetLoginInfo(user_id, apikey, session_token);

			foreach(var x in session.user.devices)
			{
				var connection = ConnectionCollection.Instance.FindOne(Query.EQ("device.device_id", x.device_id));
				bool isConnected = (x.device_id == StationRegistry.GetValue("stationId", "") as string) ? true : connection != null;

				var item = new PersonalCloudNode
				{
					Name = x.device_name,
					Id = x.device_id
				};

				switch (x.device_type)
				{
					case "Android Tablet":
					case "iPad":
						item.Type = NodeType.Tablet;
						break;

					case "Android":
					case "iPhone":
					case "Windows Phone":
						item.Type = NodeType.Phone;
						break;

					default:
						item.Type = NodeType.Station;
						break;
				}


				if (item.Type == NodeType.Station)
				{
					if (item.Name.Equals(Environment.MachineName))
					{
						item.Profile = "Connected";

						var upload = Wammer.PerfMonitor.PerfCounter.GetCounter(Wammer.PerfMonitor.PerfCounter.UP_REMAINED_COUNT, false).NextValue();
						var download = Wammer.PerfMonitor.PerfCounter.GetCounter(Wammer.PerfMonitor.PerfCounter.DW_REMAINED_COUNT, false).NextValue();

						if (upload == 0 && download == 0)
							item.Profile = "Synced at " + x.last_visit;
						else
							item.Profile = "Syncing: " + (upload + download) + " files";
					}
					else
					{
						item.Profile = "Last seen: " + x.last_visit;
					}
				}
				else
				{
					if (isConnected)
					{
						item.Profile = "Connected (Local Hyper Mode)";
					}
					else
					{
						item.Profile = "Last seen: " + x.last_visit;
					}
				}
				yield return item;

			};

		}
	}
}
