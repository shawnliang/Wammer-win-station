using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using MongoDB.Bson;

namespace Wammer.Station
{
	public class StatusChecker
	{
		private Timer timer;
		private bool logon = false;  // logOn is needed for every time service start

		public StatusChecker(long timerPeriod)
		{
			TimerCallback tcb = SendHeartbeat;
			timer = new Timer(tcb, null, 0, timerPeriod);
		}

		public static StationStatus GetStatus()
		{
			string baseurl = NetworkHelper.GetBaseURL();

			StationStatus status = new StationStatus
			{
				location = baseurl,
				diskusage = new List<DiskUsage>()
			};

			MongoDB.Driver.MongoCursor<Drivers> drivers = Drivers.collection.FindAll();

			foreach (Drivers driver in drivers)
			{
				FileStorage storage = new FileStorage(driver.folder);
				foreach (UserGroup group in driver.groups)
				{
					status.diskusage.Add(new DiskUsage { group_id = group.group_id,
														 used = storage.GetUsedSize(),
														 avail = storage.GetAvailSize() });
				}
			}

			return status;
		}

		private void SendHeartbeat(Object obj)
		{
			StationStatus status = GetStatus();

			using (WebClient agent = new WebClient())
			{
				Model.StationInfo sinfo = Model.StationInfo.collection.FindOne();
				if (sinfo != null)
				{
					Cloud.Station station = new Cloud.Station(sinfo.Id, sinfo.SessionToken);
					if (logon == false || DateTime.Now - sinfo.LastLogOn > TimeSpan.FromDays(1))
					{
						try
						{
							string baseurl = NetworkHelper.GetBaseURL();
							if (baseurl != sinfo.Location)
							{
								// update location if baseurl changed
								sinfo.Location = baseurl;
								Dictionary<object, object> locParam = new Dictionary<object, object> { { "location", baseurl } };
								station.LogOn(agent, locParam);
							}
							else
							{
								station.LogOn(agent);
							}

							logon = true;

							// update station info in database
							sinfo.LastLogOn = DateTime.Now;
							Model.StationInfo.collection.Save(sinfo);
						}
						catch (Exception ex)
						{
							log4net.LogManager.GetLogger(typeof(StatusChecker)).Warn("cloud logon error", ex);
						}
					}

					Dictionary<object, object> statusParam = new Dictionary<object, object> {
						{ "status", fastJSON.JSON.Instance.ToJSON(status, false, false, false, false) }
					};
					try
					{
						station.Heartbeat(agent, statusParam);
					}
					catch (Exception ex)
					{
						log4net.LogManager.GetLogger(typeof(StatusChecker)).Warn("cloud heartbeat error", ex);
					}
				}
			}
		}

		public void Stop()
		{
			timer.Dispose();
			using (WebClient agent = new WebClient())
			{
				Model.StationInfo sinfo = Model.StationInfo.collection.FindOne();
				if (sinfo != null)
				{
					try
					{
						Cloud.Station station = new Cloud.Station(sinfo.Id, sinfo.SessionToken);
						station.Offline(agent);
					}
					catch (Exception ex)
					{
						log4net.LogManager.GetLogger(typeof(StatusChecker)).Warn("cloud offline error", ex);
					}
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
