using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
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

			var session = JsonConvert.DeserializeObject<MR_users_get>(StationAPI.GetUser(session_token, user_id));

			foreach (var x in session.user.devices)
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

						var upload = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false).NextValue();
						var download = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false).NextValue();

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
