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
	public class StatusChecker : NonReentrantTimer
	{
		//private Timer timer;
		//private long timerPeriod;
		private bool logon = false;  // logOn is needed for every time service start
		private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StatusChecker));

		public StatusChecker(long timerPeriod)
			:base(timerPeriod)
		{
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

		protected override void ExecuteOnTimedUp(object state)
		{
			SendHeartbeat(state);
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

					using (WebClient client = new DefaultWebClient())
					{
						// use any driver's session token to send heartbeat
						foreach (var user in DriverCollection.Instance.FindAll())
						{
							Cloud.StationApi api = new Cloud.StationApi(sinfo.Id, user.session_token);
							if (logon == false || DateTime.Now - sinfo.LastLogOn > TimeSpan.FromDays(1))
							{
								logger.Debug("cloud logon start");
								api.LogOn(client, detail);
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
							api.Heartbeat(client, detail);
							break;
						}
					}
				}
				catch (Exception ex)
				{
					logger.Debug("cloud send heartbeat error", ex);
				}
			}
		}

		public override void Stop()
		{
			base.Stop();
			using (WebClient client = new DefaultWebClient())
			{
				Model.StationInfo sinfo = Model.StationCollection.Instance.FindOne();
				if (sinfo != null)
				{
					try
					{
						Cloud.StationApi api = new Cloud.StationApi(sinfo.Id, sinfo.SessionToken);
						api.Offline(client);
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
