using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

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

						var importTasks = TaskStatusCollection.Instance.Find(Query.EQ("UserId", user_id));
						var importStatus = string.Join(", ", importTasks.Where(t => t.IsComplete).Select(t => formatTaskString(t)).ToArray());

						var upload = PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false).NextValue();
						var download = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false).NextValue();

						var uploadStatus = "";
						if (upload > 0)
							uploadStatus = string.Format("Uploading {0} files. ", upload);

						var downloadStatus = "";
						if (download > 0)
							downloadStatus = string.Format("Downloading {0} files. ", download);

						var syncRange = DriverCollection.Instance.FindOneById(user_id).sync_range;


						if (string.IsNullOrEmpty(importStatus) && uploadStatus.Length == 0 && downloadStatus.Length == 0)
						{
							if (!string.IsNullOrEmpty(syncRange.error))
								item.Profile = Resources.SYNC_ERROR + syncRange.error;
							else if (syncRange.syncing)
								item.Profile = Resources.DOWNLOAD_INDEX;
							else
								item.Profile = "Synced";
						}
						else
						{
							item.Profile = importStatus + uploadStatus + downloadStatus;
						}
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

		private string formatTaskString(ImportTaskStaus t)
		{
			if (!t.IsComplete && t.TotalFiles == 0)
				return "Indexing files...";
			else
			{
				if (t.FailedFiles == null || t.FailedFiles.Count == 0)
				{
					return string.Format("{0}/{1} files imported. ",
						t.SuccessCount,
						t.TotalFiles);
				}
				else
				{
					return string.Format("{0}/{1} files imported, {2} import failures. ",
					t.SuccessCount,
					t.TotalFiles,
					t.FailedFiles.Count);
				}
			}
		}
	}
}
