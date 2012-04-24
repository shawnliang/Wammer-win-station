using System;
using System.Net;
using log4net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using System.Linq;
using Wammer.Utility;
using Wammer.PostUpload;

namespace Wammer.Station
{
	class ResumeSyncHandler : HttpHandler
	{
		private readonly PostUploadTaskRunner postUploadRunner;
		private readonly StationTimer stationTimer;
		private readonly AbstrackTaskRunner[] bodySyncRunners;

		public ResumeSyncHandler(PostUploadTaskRunner postUploadRunner, StationTimer stationTimer, AbstrackTaskRunner[] bodySyncRunners)
		{
			this.postUploadRunner = postUploadRunner;
			this.stationTimer = stationTimer;
			this.bodySyncRunners = bodySyncRunners;
		}

		protected override void HandleRequest()
		{
			string email = Parameters["email"];
			string password = Parameters["password"];

			try
			{
				StationInfo stationInfo = StationCollection.Instance.FindOne();
				if (stationInfo != null)
				{
					this.LogDebugMsg(string.Format("Station logon with stationId = {0}", stationInfo.Id));

					StationLogOnResponse logonRes;
					if (email != null && password != null)
					{
						using (WebClient client = new DefaultWebClient())
						{
							Driver driver = DriverCollection.Instance.FindOne(Query.EQ("email", email));
							if (driver == null)
							{
								throw new InvalidOperationException("User is not registered to this station");
							}

							// station.logon must be called before user.login to handle non-existing driver case
							logonRes = StationApi.LogOn(client, stationInfo.Id, email, password, StatusChecker.GetDetail());

							User user = User.LogIn(client, email, password);

							// update user's session token
							driver.session_token = user.Token;
							DriverCollection.Instance.Save(driver);
						}
					}
					else
					{
						using (WebClient client = new DefaultWebClient())
						{
							StationApi api = new StationApi(stationInfo.Id, stationInfo.SessionToken);
							logonRes = api.LogOn(client, StatusChecker.GetDetail());
						}
					}

					// update session in DB
					stationInfo.SessionToken = logonRes.session_token;
					StationCollection.Instance.Save(stationInfo);

					this.LogDebugMsg("Station logon successfully, disable block auth of function server");
				}

				postUploadRunner.Start();
				stationTimer.Start();
				Array.ForEach(bodySyncRunners, (taskRunner) => taskRunner.Start());
				this.LogDebugMsg("Start function server successfully");

				RespondSuccess();
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError == 0x4000 + 3) // driver already registered another station
				{
					this.LogErrorMsg("Driver already registered another station");

					// force user re-register the station on next startup
					CleanDB();

					postUploadRunner.Stop();
					stationTimer.Stop();
					Array.ForEach(bodySyncRunners, (taskRunner) => taskRunner.Stop());

					throw new ServiceUnavailableException("Driver already registered another station", (int)StationApiError.AlreadyHasStaion);
				}
				else if (e.WammerError == 0x4000 + 4) // user does not exist
				{
					this.LogErrorMsg("Driver account does not exist");

					// force user re-register the station on next startup
					CleanDB();

					postUploadRunner.Stop();
					stationTimer.Stop();
					Array.ForEach(bodySyncRunners, (taskRunner) => taskRunner.Stop());

					throw;
				}
				else
					throw;
			}
		}

		private static void CleanDB()
		{
			DriverCollection.Instance.RemoveAll();
			CloudStorageCollection.Instance.RemoveAll();
			StationCollection.Instance.RemoveAll();
		}
	}

	public class StationOnlineResponse : CloudResponse
	{
		public string session_token { get; set; }

		public StationOnlineResponse()
			: base()
		{
		}
	}
}
