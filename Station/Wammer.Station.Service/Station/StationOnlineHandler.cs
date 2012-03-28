using System;
using System.Net;
using log4net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station
{
	public class StationOnlineHandler : HttpHandler
	{
		private static ILog logger = LogManager.GetLogger("StationOnlineHandler");
		private readonly HttpServer functionServer;
		private readonly StationTimer stationTimer;

		public StationOnlineHandler(HttpServer functionServer, StationTimer stationTimer)
		{
			this.functionServer = functionServer;
			this.stationTimer = stationTimer;
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
					logger.DebugFormat("Station logon with stationId = {0}", stationInfo.Id);

					StationLogOnResponse logonRes;
					if (email != null && password != null)
					{
						using (WebClientProxy client = WebClientPool.GetFreeClient())
						{
							Driver driver = DriverCollection.Instance.FindOne(Query.EQ("email", email));
							if (driver == null)
							{
								throw new InvalidOperationException("User is not registered to this station");
							}

							// station.logon must be called before user.login to handle non-existing driver case
							logonRes = StationApi.LogOn(client.Agent, stationInfo.Id, email, password, StatusChecker.GetDetail());

							User user = User.LogIn(client.Agent, email, password);

							// update user's session token
							driver.session_token = user.Token;
							DriverCollection.Instance.Save(driver);
						}
					}
					else
					{
						using (WebClientProxy client = WebClientPool.GetFreeClient())
						{
							StationApi api = new StationApi(stationInfo.Id, stationInfo.SessionToken);
							logonRes = api.LogOn(client.Agent, StatusChecker.GetDetail());
						}
					}

					// update session in DB
					stationInfo.SessionToken = logonRes.session_token;
					StationCollection.Instance.Save(stationInfo);

					logger.Debug("Station logon successfully, disable block auth of function server");
					functionServer.BlockAuth(false);
				}

				logger.Debug("Start function server");
				functionServer.Start();
				stationTimer.Start();
				logger.Debug("Start function server successfully");

				RespondSuccess();
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError == 0x4000 + 3) // driver already registered another station
				{
					logger.Error("Driver already registered another station");

					// force user re-register the station on next startup
					CleanDB();

					// function server should be stopped if driver's info is removed
					logger.Debug("Try to stop function server");
					functionServer.Stop();
					stationTimer.Stop();

					throw new ServiceUnavailableException("Driver already registered another station", (int)StationApiError.AlreadyHasStaion);
				}
				else if (e.WammerError == 0x4000 + 4) // user does not exist
				{
					logger.Error("Driver account does not exist");

					// force user re-register the station on next startup
					CleanDB();

					// function server should be stopped if driver's info is removed
					logger.Debug("Try to stop function server");
					functionServer.Stop();
					stationTimer.Stop();

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

		public override object Clone()
		{
			return this.MemberwiseClone();
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
