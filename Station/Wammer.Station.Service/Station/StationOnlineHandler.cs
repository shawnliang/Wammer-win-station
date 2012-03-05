using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;

using Wammer.Model;
using Wammer.Cloud;
using MongoDB.Driver.Builders;

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

			Driver driver = DriverCollection.Instance.FindOne();
			if (driver == null)
			{
				logger.Error("No driver detected");
				
				// function server should be stopped if driver's info is removed
				logger.Debug("Try to stop function server");
				functionServer.Stop();
				stationTimer.Stop();

				throw new InvalidOperationException("Station cannot work without driver");
			}

			if (email != null && password != null && driver.email != email)
			{
				logger.Error("Invalid driver");
				throw new WammerStationException("Invalid driver", (int)StationApiError.InvalidDriver);
			}

			StationInfo stationInfo = StationCollection.Instance.FindOne();
			if (stationInfo == null)
			{
				logger.Error("Station has no info");
				throw new InvalidOperationException("Station collection is empty");
			}

			try
			{
				logger.Debug("Start function server");

				functionServer.Start();
				stationTimer.Start();

				logger.DebugFormat("Station logon with stationId = {0}", stationInfo.Id);

				StationLogOnResponse logonRes;

				if (email != null && password != null)
				{
					using (WebClient agent = new WebClient())
					{
						// station.logon must be called before user.login to handle non-existing driver case
						logonRes = StationApi.LogOn(new WebClient(), stationInfo.Id, email, password, StatusChecker.GetDetail());

						User user = User.LogIn(agent, email, password);

						// update user's session token
						driver.session_token = user.Token;
						DriverCollection.Instance.Save(driver);
					}
				}
				else
				{
					StationApi api = new StationApi(stationInfo.Id, stationInfo.SessionToken);
					logonRes = api.LogOn(new WebClient(), StatusChecker.GetDetail());
				}

				// update session in DB
				stationInfo.SessionToken = logonRes.session_token;
				StationCollection.Instance.Save(stationInfo);
				
				logger.Debug("Station logon successfully, disable block auth of function server");
				functionServer.BlockAuth(false);

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
