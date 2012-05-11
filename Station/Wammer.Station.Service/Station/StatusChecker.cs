using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class IsPrimaryChangedEvtArgs : EventArgs
	{
		public Driver driver;

		public IsPrimaryChangedEvtArgs(Driver driver)
		{
			this.driver = driver;
		}
	}

	public class StatusChecker : NonReentrantTimer
	{
		//private Timer timer;
		//private long timerPeriod;
		private bool logon;  // logOn is needed for every time service start
		private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StatusChecker));

		public EventHandler<IsPrimaryChangedEvtArgs> IsPrimaryChanged;

		public StatusChecker(long timerPeriod)
			:base(timerPeriod)
		{
		}

		public static StationDetail GetDetail()
		{
			string baseurl = NetworkHelper.GetBaseURL();

			var status = new StationDetail
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
				var storage = new FileStorage(driver);
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
			SendHeartbeat();
		}

		private void SendHeartbeat()
		{
			StationDetail detail = GetDetail();

			Model.StationInfo sinfo = Model.StationCollection.Instance.FindOne();
			if (sinfo != null)
			{
				try
				{
					string baseurl = NetworkHelper.GetBaseURL();
					if (baseurl != sinfo.Location)
					{
						// update location if baseurl changed
						logger.DebugFormat("station location changed: {0}", baseurl);
						sinfo.Location = baseurl;

						// update station info in database
						logger.Debug("update station information");
						Model.StationCollection.Instance.Save(sinfo);
					}

					using (WebClient client = new DefaultWebClient())
					{
						LogonAndHeartbeat(client, sinfo, detail);
						UpdatePrimaryStationSetting(client);
					}
				}
				catch (Exception ex)
				{
					logger.Debug("cloud send heartbeat error", ex);
				}
			}
		}

		private void LogonAndHeartbeat(WebClient client, StationInfo sinfo, StationDetail detail)
		{
			// use any driver's session token to send heartbeat
			var user = DriverCollection.Instance.FindOne();
			var api = new StationApi(sinfo.Id, user.session_token);

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

			api.Heartbeat(client, detail);
		}

		private void UpdatePrimaryStationSetting(WebClient client)
		{
			foreach (var user in DriverCollection.Instance.FindAll())
			{
				var res = Cloud.User.FindMyStation(client, user.session_token);
				var currStation = (from station in res.stations 
								   where station.station_id == StationRegistry.StationId
								   select station).FirstOrDefault();

				Debug.Assert(currStation != null);

				if (currStation != null)
				{
					bool isCurrPrimaryStation = (currStation.type == "primary");
					if (user.isPrimaryStation != isCurrPrimaryStation)
					{
						user.isPrimaryStation = isCurrPrimaryStation;
						DriverCollection.Instance.Update(
							Query.EQ("_id", user.user_id),
							Update.Set("isPrimaryStation", isCurrPrimaryStation)
						);
						OnIsPrimaryChanged(new IsPrimaryChangedEvtArgs(user));
					}
				}
			}
		}

		private void OnIsPrimaryChanged(IsPrimaryChangedEvtArgs args)
		{
			var handler = this.IsPrimaryChanged;

			if (handler != null)
				handler(this, args);
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
						var api = new StationApi(sinfo.Id, sinfo.SessionToken);
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
