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
			if (driver == null || driver.email != email)
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

			logger.DebugFormat("Station logon with stationId = {0}, email = {1}", stationInfo.Id, email);
			StationLogOnResponse logonRes = StationApi.LogOn(new WebClient(), stationInfo.Id, email, password);

			logger.Debug("Station logon successfully, start function server");
			functionServer.Start();
			WriteOnlineStateToDB();

			logger.Debug("Start function server successfully");
			RespondSuccess(new StationOnlineResponse { session_token = logonRes.session_token, status = 200, timestamp = DateTime.UtcNow, api_ret_code = 0, api_ret_msg = "success" });
		}

		private static void WriteOnlineStateToDB()
		{
			Model.Service svc = ServiceCollection.FindOne(Query.EQ("_id", "StationService"));
			if (svc == null)
			{
				svc = new Model.Service { Id = "StationService", State = ServiceState.Online };
			}
			else
				svc.State = ServiceState.Online;

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
