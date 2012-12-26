using log4net;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	public class StatusChecker : NonReentrantTimer
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof(StatusChecker));

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
								ws_location = "ws://" + new Uri(baseurl).Host + ":9983",
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
			sendHeartbeat();
			checkIfUserPaid();
		}

		private void checkIfUserPaid()
		{
			foreach (var user in DriverCollection.Instance.FindAll())
			{
				try
				{
					if (string.IsNullOrEmpty(user.session_token))
						continue;

					var cloudUser = User.GetInfo(user.user_id, CloudServer.APIKey, user.session_token);

					if (becomePaidUser(user, cloudUser))
					{
						DriverCollection.Instance.Update(Query.EQ("_id", user.user_id), Update.Set("isPaidUser", true));
						backupAttachmentsToCloud(user);
					}
					else if (becomeNonPaidUser(user, cloudUser))
					{
						DriverCollection.Instance.Update(Query.EQ("_id", user.user_id), Update.Set("isPaidUser", false));
					}
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Unable to check if user paid: " + user.email, e);
				}
			}
		}

		private static bool becomeNonPaidUser(Driver user, GetUserResponse cloudUser)
		{
			return user.isPaidUser && !cloudUser.IsPaidUser();
		}

		private static bool becomePaidUser(Driver user, GetUserResponse cloudUser)
		{
			return !user.isPaidUser && cloudUser.IsPaidUser();
		}

		private static void backupAttachmentsToCloud(Driver user)
		{
			var notBackupFiles = AttachmentCollection.Instance.Find(
								  Query.And(
									  Query.EQ("group_id", user.groups[0].group_id),
									  Query.Exists("saved_file_name"),
									  Query.NE("body_on_cloud", true)));

			foreach (var file in notBackupFiles)
			{
				var util = new AttachmentUpload.AttachmentUtility();
				util.UpstreamAttachmentAsync(file.object_id, ImageMeta.Origin, TaskPriority.VeryLow);
			}
		}

		private void sendHeartbeat()
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