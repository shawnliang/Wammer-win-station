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
		private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StatusChecker));

		public StatusChecker(long timerPeriod)
		{
			TimerCallback tcb = SendHeartbeat;
			timer = new Timer(tcb, null, 0, timerPeriod);
		}

		public static StationDetail GetDetail()
		{
			string baseurl = NetworkHelper.GetBaseURL();

			StationDetail status = new StationDetail
			{
				location = baseurl,
				diskusage = new List<DiskUsage>(),
				upnp = PublicPortMapping.Instance.GetUPnPInfo()
			};

			MongoDB.Driver.MongoCursor<Drivers> drivers = Drivers.collection.FindAll();

			foreach (Drivers driver in drivers)
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
			string detailJson = detail.ToFastJSON();

			using (WebClient agent = new WebClient())
			{
				Model.StationInfo sinfo = Model.StationInfo.collection.FindOne();
				if (sinfo != null)
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

					Cloud.StationApi api = new Cloud.StationApi(sinfo.Id, sinfo.SessionToken);
					if (logon == false || DateTime.Now - sinfo.LastLogOn > TimeSpan.FromDays(1))
					{
						logger.Debug("cloud logon start");
						try
						{
							api.LogOn(agent, new Dictionary<object, object> { { "detail", detailJson } });
							logon = true;

							// update station info in database
							logger.Debug("update station information");
							sinfo.LastLogOn = DateTime.Now;
							Model.StationInfo.collection.Save(sinfo);
						}
						catch (Exception ex)
						{
							logger.Warn("cloud logon error", ex);
						}
					}

					try
					{
						if (locChange)
						{
							// update station info in database
							logger.Debug("update station information");
							Model.StationInfo.collection.Save(sinfo);
						}
						api.Heartbeat(agent, new Dictionary<object, object> { { "detail", detailJson } });
					}
					catch (Exception ex)
					{
						logger.Warn("cloud heartbeat error", ex);
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
	}
}
