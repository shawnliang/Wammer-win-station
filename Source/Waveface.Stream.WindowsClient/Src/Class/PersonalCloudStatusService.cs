using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Net;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	class PersonalCloudStatusService : IPersonalCloudStatus
	{
		public ICollection<PersonalCloudNode> GetNodes(string user_id, string session_token, string apikey)
		{
			var retNodes = new List<PersonalCloudNode>();

			try
			{
				var nodes = queryDeviceNodesFromCloud(user_id, session_token);
				retNodes.AddRange(nodes);
			}
			catch (WebException e)
			{
				var err = e.GetDisplayDescription();

				retNodes.Add(
					new PersonalCloudNode
					{
						Name = Environment.MachineName,
						Id = StationRegistry.StationId,
						Profile = string.Format(Resources.SYNC_OFFLINE, err),
						Type = NodeType.Station
					});
			}

			return retNodes;
		}

		private List<PersonalCloudNode> queryDeviceNodesFromCloud(string user_id, string session_token)
		{
			var nodes = new List<PersonalCloudNode>();

			var userInfo = Waveface.Stream.ClientFramework.UserInfo.Instance;
			var devices = userInfo.Devices;

			foreach (var device in devices)
			{
				var connection = ConnectionCollection.Instance.FindOne(Query.EQ("device.device_id", device.device_id));
				bool isConnected = (device.device_id == StationRegistry.GetValue("stationId", "") as string) ? true : connection != null;


				var item = new PersonalCloudNode
				{
					Name = device.device_name,
					Id = device.device_id
				};

				switch (device.device_type)
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

					case "Windows":
						item.Type = NodeType.Station;
						break;

					default:
						continue;
				}



				if (item.Type == NodeType.Station)
				{
					if (item.Name.Equals(Environment.MachineName))
					{
						item.Profile = SyncStatus.GetSyncStatus();
					}
					else
					{
						item.Profile = string.Format(Resources.SYNC_LAST_SEEN, device.last_visit);
					}
				}
				else
				{
					if (isConnected)
					{
						item.Profile = Resources.SYNC_CONNECTED_LOCALLY;
					}
					else
					{
						item.Profile = string.Format(Resources.SYNC_LAST_SEEN, device.last_visit);
					}
				}

				nodes.Add(item);
			};
			return nodes;
		}
	}
}
