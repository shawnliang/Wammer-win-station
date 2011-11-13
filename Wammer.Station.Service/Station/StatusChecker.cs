using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

using Wammer.Cloud;
using MongoDB.Bson;

namespace Wammer.Station
{
	public class StatusChecker
	{
		private Timer timer;

		public StatusChecker(long timerPeriod)
		{
			TimerCallback tcb = SendHeartbeat;
			timer = new Timer(tcb, null, 0, timerPeriod);
		}

		public static StationStatus GetStatus()
		{
			StationStatus status = new StationStatus
			{
				location = "http://" + StationInfo.IPv4Address + ":9981/",
				diskusage = new List<DiskUsage>()
			};

			MongoDB.Driver.MongoCursor<StationDriver> drivers =
				Database.mongodb.GetDatabase("wammer").GetCollection<StationDriver>("drivers").FindAll();

			foreach (StationDriver driver in drivers)
			{
				FileStorage storage = new FileStorage(driver.folder);
				foreach (UserGroup group in driver.groups)
				{
					status.diskusage.Add(new DiskUsage { group_id = group.group_id, used = storage.GetUsedSize(), avail = storage.GetAvailSize() });
				}
			}

			return status;
		}

		private void SendHeartbeat(Object obj)
		{
			StationStatus status = GetStatus();

			using (WebClient agent = new WebClient())
			{
				StationDBDoc stationDoc = 
					Database.mongodb.GetDatabase("wammer").GetCollection<StationDBDoc>("station").FindOne();
				if (stationDoc != null)
				{
					Cloud.Station station = new Cloud.Station(stationDoc.Id, stationDoc.SessionToken);
					Dictionary<object, object> param = new Dictionary<object, object> { { "status", status.ToJson() } };
					station.Heartbeat(agent, param);				
				}
			}
		}
	}

	public class StationStatus
	{
		public string location { get; set; }
		public List<DiskUsage> diskusage { get; set; }
	}

	public class DiskUsage
	{
		public string group_id { get; set; }
		public long used { get; set; }
		public long avail { get; set; }
	}

}
