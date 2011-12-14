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

		public StationOnlineHandler(HttpServer functionServer)
		{
			this.functionServer = functionServer;
		}

		protected override void HandleRequest()
		{
			string email = Parameters["email"];
			string password = Parameters["password"];

			if (email == null || password == null)
			{
				logger.Error("email or password is missing");
				throw new FormatException("email or password is missing");
			}

			Drivers driver = Drivers.collection.FindOne();
			if (driver == null)
			{
				logger.Error("No driver detected");
				
				// function server should be stopped if driver's info is removed
				logger.Debug("Try to stop function server");
				functionServer.Stop();
				WriteServiceStateToDB(ServiceState.Offline);

				throw new ServiceUnavailableException("Station cannot work without driver", (int)StationApiError.ServiceUnavailable);
			}

			if (driver.email != email)
			{
				logger.Error("Driver is null or email inconsistent");
				throw new WammerStationException("Invalid driver", (int)StationApiError.InvalidDriver);
			}

			StationInfo stationInfo = StationInfo.collection.FindOne();
			if (stationInfo == null)
			{
				logger.Error("Station has no info");
				throw new InvalidOperationException("Station collection is empty");
			}

			try
			{
				logger.DebugFormat("Station logon with stationId = {0}, email = {1}", stationInfo.Id, email);
				StationLogOnResponse logonRes = StationApi.LogOn(new WebClient(), stationInfo.Id, email, password);
				
				logger.Debug("Station logon successfully, start function server");
				functionServer.BlockAuth(false);
				functionServer.Start();
				WriteServiceStateToDB(ServiceState.Online);

				logger.Debug("Start function server successfully");
				RespondSuccess(new StationOnlineResponse { session_token = logonRes.session_token, status = 200, timestamp = DateTime.UtcNow, api_ret_code = 0, api_ret_msg = "success" });
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError == 0x4000 + 3) // driver already registered another station
				{
					logger.Error("Driver already registered another station");

					// remove driver and cloudstorage info since driver already registered another station
					Drivers.collection.RemoveAll();
					CloudStorage.collection.RemoveAll();
					StationInfo.collection.RemoveAll();
					
					// function server should be stopped if driver's info is removed
					logger.Debug("Try to stop function server");
					functionServer.Stop();
					WriteServiceStateToDB(ServiceState.Offline);

					throw new ServiceUnavailableException("Driver already registered another station", (int)StationApiError.ServiceUnavailable);
				}
				else
					throw;
			}
		}

		private static void WriteServiceStateToDB(ServiceState state)
		{
			Model.Service svc = ServiceCollection.FindOne(Query.EQ("_id", "StationService"));
			if (svc == null)
			{
				svc = new Model.Service { Id = "StationService", State = state };
			}
			else
				svc.State = state;

			ServiceCollection.Save(svc);
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
