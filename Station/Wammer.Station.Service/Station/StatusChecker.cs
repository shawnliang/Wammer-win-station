using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class IsPrimaryChangedEvtArgs : EventArgs
	{
		public Driver driver { get; set; }

		public IsPrimaryChangedEvtArgs(Driver driver)
		{
			this.driver = driver;
		}
	}

	public class StatusChecker : NonReentrantTimer
	{
		//private Timer timer;
		//private long timerPeriod;
		private static readonly ILog logger = LogManager.GetLogger(typeof (StatusChecker));

		public EventHandler<IsPrimaryChangedEvtArgs> IsPrimaryChanged;
		private bool logon; // logOn is needed for every time service start

		public StatusChecker(long timerPeriod)
			: base(timerPeriod)
		{
		}

		public static StationDetail GetDetail()
		{
			var baseurl = NetworkHelper.GetBaseURL();

			var status = new StationDetail
			             	{
			             		location = baseurl,
			             		diskusage = new List<DiskUsage>(),
			             		upnp = new UPnPInfo { status = false },
			             		computer_name = Environment.MachineName,
			             		version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
			             	};

			var drivers = DriverCollection.Instance.FindAll();

			foreach (var driver in drivers)
			{
				var storage = new FileStorage(driver);
				foreach (var group in driver.groups)
				{
					//TODO: storage.GetAvailSize() 這段有誤...
					status.diskusage.Add(new DiskUsage
					                     	{
					                     		group_id = group.group_id,
					                     		used = storage.GetUsedSize(),
					                     		avail = storage.GetAvailSize() 
					                     	});
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
			var detail = GetDetail();

			var sinfo = StationCollection.Instance.FindOne();
			if (sinfo != null)
			{
				try
				{
					var baseurl = NetworkHelper.GetBaseURL();
					if (baseurl != sinfo.Location)
					{
						// update location if baseurl changed
						logger.InfoFormat("station location changed: {0}", baseurl);
						sinfo.Location = baseurl;

						// update station info in database
						logger.Info("update station information");
						StationCollection.Instance.Save(sinfo);
					}

					LogonAndHeartbeat(sinfo, detail);
					UpdatePrimaryStationSetting();
				}
				catch (Exception ex)
				{
					logger.Info("cloud send heartbeat error", ex);
				}
			}
		}

		private void LogonAndHeartbeat(StationInfo sinfo, StationDetail detail)
		{
			try
			{
				// use any driver's session token to send heartbeat
				var user = DriverCollection.Instance.FindOne(Query.NE("session_token", string.Empty));
				if (user != null)
				{
					var api = new StationApi(sinfo.Id, user.session_token);

					if (logon == false || DateTime.Now - sinfo.LastLogOn > TimeSpan.FromDays(1))
					{
						logger.Info("cloud logon start");
						api.LogOn(detail);
						logon = true;

						// update station info in database
						logger.Info("update station information");
						sinfo.LastLogOn = DateTime.Now;
						StationCollection.Instance.Save(sinfo);
					}

					api.Heartbeat(detail);
				}
				else
				{
					this.LogDebugMsg("no available sessions for heartbeat");
				}
			}
			catch (WammerCloudException e)
			{
				this.LogDebugMsg("unable to send heartbeat", e);
			}
		}

		private void UpdatePrimaryStationSetting()
		{
			foreach (var user in DriverCollection.Instance.FindAll())
			{
				try
				{
					var res = User.FindMyStation(user.session_token);
					var currStation = (from station in res.stations
									   where station.station_id == StationRegistry.StationId
									   select station).FirstOrDefault();

					if (currStation == null)
						return;

					var isCurrPrimaryStation = (currStation.type == "primary");
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
				catch (WammerCloudException e)
				{
					if (CloudServer.IsSessionError(e))
					{
						user.session_token = string.Empty;
						DriverCollection.Instance.Save(user);
						LoginedSessionCollection.Instance.Remove(Query.EQ("user.email", user.email));
						this.LogDebugMsg(string.Format("cloud returns 401, clear session of {0}", user.email));
					}
					else
					{
						this.LogDebugMsg(string.Format("unable to update primary settings of user {0}", user.email), e);
					}
				}
			}
		}

		private void OnIsPrimaryChanged(IsPrimaryChangedEvtArgs args)
		{
			var handler = IsPrimaryChanged;

			if (handler != null)
				handler(this, args);
		}

		public override void Stop()
		{
			base.Stop();
			var sinfo = StationCollection.Instance.FindOne();
			if (sinfo != null)
			{
				try
				{
					var api = new StationApi(sinfo.Id, sinfo.SessionToken);
					api.Offline();
				}
				catch (Exception ex)
				{
					logger.Warn("cloud offline error", ex);
				}
			}
		}
	}
}