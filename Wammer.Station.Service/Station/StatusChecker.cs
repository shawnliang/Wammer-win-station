using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

using Wammer.Cloud;
using Wammer.Model;
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

		public static IPAddress[] GetIPAddressesV4()
		{
			IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
			List<IPAddress> ret = new List<IPAddress>();

			foreach (IPAddress ip in ips)
			{
				if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
					!IPAddress.IsLoopback(ip))
					ret.Add(ip);
			}

			return ret.ToArray();
		}
		
		public static StationStatus GetStatus()
		{
			IPAddress ip = GetIPAddressesV4()[0];

			StationStatus status = new StationStatus
			{
				location = "http://" + ip + ":9981/",
				diskusage = new List<DiskUsage>()
			};

			MongoDB.Driver.MongoCursor<Drivers> drivers = Drivers.collection.FindAll();

			foreach (Drivers driver in drivers)
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
				Model.StationInfo sinfo = Model.StationInfo.collection.FindOne();
				if (sinfo != null)
				{
					Cloud.Station station = new Cloud.Station(sinfo.Id, sinfo.SessionToken);
					if (logon == false || DateTime.Now - sinfo.LastLogOn > TimeSpan.FromDays(1))
					{
						try
						{
							IPAddress ip = GetIPAddressesV4()[0];
							if (ip != sinfo.Location)
							{
								// update location if ip changed
								sinfo.Location = ip;
								Dictionary<object, object> locParam = new Dictionary<object, object> { { "location", "http://" + ip + ":9981/" } };
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

					Dictionary<object, object> statusParam = new Dictionary<object, object> { { "status", status.ToJson() } };
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
