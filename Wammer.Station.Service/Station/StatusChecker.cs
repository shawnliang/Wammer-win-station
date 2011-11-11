using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

using MongoDB.Bson;

namespace Wammer.Station
{
	public class StatusChecker
	{
		private AtomicDictionary<string, FileStorage> groupFolderMap;
		private Timer timer;
		private const long TIMER_PERIOD = 10 * 60 * 1000; 

		public StatusChecker(AtomicDictionary<string, FileStorage> groupFolderMap)
		{
			this.groupFolderMap = groupFolderMap;
			TimerCallback tcb = SendHeartbeat;
			timer = new Timer(tcb, null, 0, TIMER_PERIOD);
		}

		public StationStatus GetStatus()
		{
			StationStatus status = new StationStatus
			{
				location = "http://" + StationInfo.IPv4Address + ":9981/",
				diskusage = new List<DiskUsage>()
			};

			Dictionary<string, FileStorage> gfmap = groupFolderMap.GetAll();
			foreach (KeyValuePair<string, FileStorage> pair in gfmap)
			{
				status.diskusage.Add(new DiskUsage { group_id = pair.Key, used = pair.Value.GetUsedSize(), avail = pair.Value.GetAvailSize() });
			}

			return status;
		}

		private void SendHeartbeat(Object obj)
		{
			StationStatus status = GetStatus();

			using (WebClient agent = new WebClient())
			{
				Dictionary<object, object> param = new Dictionary<object, object>{{"status", status.ToJson()}};
				Cloud.Station.Heartbeat(agent, param);
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
