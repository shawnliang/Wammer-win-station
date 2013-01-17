using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;
using System.Net;

namespace Waveface.Stream.WindowsClient
{
	class PersonalCloudStatusService : IPersonalCloudStatus
	{
		public ICollection<PersonalCloudNode> GetNodes(string user_id, string session_token, string apikey)
		{
			var retNodes = new List<PersonalCloudNode>()
			{
				new PersonalCloudNode()
				{
					Name = "Stream Cloud",
					Id = Guid.Empty.ToString(),
					Profile = "Connected",
					Type = NodeType.Station
				}
			};

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
						Profile = err,
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

					default:
						item.Type = NodeType.Station;
						break;
				}



				if (item.Type == NodeType.Station)
				{
					if (item.Name.Equals(Environment.MachineName))
					{
						item.Profile = "Connected";

						var syncRange = DriverCollection.Instance.FindOneById(user_id).sync_range;

						var importStatus = ImportStatus.Lookup(user_id).Description;

						var upload = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false).NextValue();
						var download = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false).NextValue();

						var uploadStatus = "";
						if (upload > 0)
						{
							uploadStatus = string.Format("Uploading {0} files. ", upload);
						}

						var downloadStatus = "";
						if (download > 0)
						{
							downloadStatus = string.Format("Downloading {0} files. ", download);
						}


						if (string.IsNullOrEmpty(importStatus) && uploadStatus.Length == 0 && downloadStatus.Length == 0)
						{
							if (!string.IsNullOrEmpty(syncRange.download_index_error))
								item.Profile = Resources.SYNC_ERROR + syncRange.download_index_error;
							else if (syncRange.syncing)
								item.Profile = Resources.DOWNLOAD_INDEX;
							else
								item.Profile = "Synced";
						}
						else
						{
							item.Profile = importStatus + " " + uploadStatus + downloadStatus;

							var uploadDownloadError = syncRange.GetUploadDownloadError();
							if (!string.IsNullOrEmpty(uploadDownloadError))
								item.Profile = importStatus + Resources.SYNC_ERROR + uploadDownloadError;
						}
					}
					else
					{
						item.Profile = "Last seen: " + device.last_visit;
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
						item.Profile = "Last seen: " + device.last_visit;
					}
				}

				nodes.Add(item);
			};
			return nodes;
		}
	}
}
