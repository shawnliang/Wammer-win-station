using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class StatusChecker : IStationTimer
	{
		private Timer timer;
		private long timerPeriod;
		private bool logon = false;  // logOn is needed for every time service start
		private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StatusChecker));
		private readonly HttpServer functionServer;

		public StatusChecker(long timerPeriod, HttpServer functionServer)
		{
			TimerCallback tcb = SendHeartbeat;
			this.timer = new Timer(tcb);
			this.timerPeriod = timerPeriod;
			this.functionServer = functionServer;
		}

		public static StationDetail GetDetail()
		{
			string baseurl = NetworkHelper.GetBaseURL();

			StationDetail status = new StationDetail
			{
				location = baseurl,
				diskusage = new List<DiskUsage>(),
				upnp = PublicPortMapping.Instance.GetUPnPInfo(),
				computer_name = Environment.MachineName,
				version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
			};

			MongoDB.Driver.MongoCursor<Driver> drivers = DriverCollection.Instance.FindAll();

			foreach (Driver driver in drivers)
			{
				FileStorage storage = new FileStorage(driver);
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
			StationDetail detail = GetDetail();

			Model.StationInfo sinfo = Model.StationCollection.Instance.FindOne();
			if (sinfo != null)
			{
				try
				{
					bool locChange = false;
					string baseurl = NetworkHelper.GetBaseURL();
					if (baseurl != sinfo.Location)
					{
						// update location if baseurl changed
						logger.DebugFormat("station location changed: {0}", baseurl);
						sinfo.Location = baseurl;
						locChange = true;
					}

					WebClient agent = new WebClient();
					Cloud.StationApi api = new Cloud.StationApi(sinfo.Id, sinfo.SessionToken);
					if (logon == false || DateTime.Now - sinfo.LastLogOn > TimeSpan.FromDays(1))
					{
						logger.Debug("cloud logon start");
						api.LogOn(agent, detail);
						logon = true;

						// update station info in database
						logger.Debug("update station information");
						sinfo.LastLogOn = DateTime.Now;
						Model.StationCollection.Instance.Save(sinfo);
					}

					if (locChange)
					{
						// update station info in database
						logger.Debug("update station information");
						Model.StationCollection.Instance.Save(sinfo);
					}
					api.Heartbeat(agent, detail);
				}
				catch (WammerCloudException ex)
				{
					WebException webex = (WebException)ex.InnerException;
					if (webex != null)
					{
						HttpWebResponse response = (HttpWebResponse)webex.Response;
						if (response != null)
						{
							if (response.StatusCode == HttpStatusCode.Unauthorized)
							{
								// station's session token expired, it might be caused by:
								// 1. server maintenance
								// 2. driver registered another station
								// in this situation, client has to re-login/re-register the station
								functionServer.BlockAuth(true);
							}
						}
					}
					logger.Warn("cloud send heartbeat error", ex);
				}
				catch (Exception ex)
				{
					logger.Warn("cloud send heartbeat error", ex);
				}
			}
		}

		public void Start()
		{
			timer.Change(0, timerPeriod);
		}

		public void Stop()
		{
			timer.Change(Timeout.Infinite, Timeout.Infinite);
			using (WebClient agent = new WebClient())
			{
				Model.StationInfo sinfo = Model.StationCollection.Instance.FindOne();
				if (sinfo != null)
				{
					try
					{
						Cloud.StationApi api = new Cloud.StationApi(sinfo.Id, sinfo.SessionToken);
						api.Offline(agent);
					}
					catch (Exception ex)
					{
						logger.Warn("cloud offline error", ex);
					}
				}
			}
		}

		public void Close()
		{
			timer.Dispose();
		}
	}
}
