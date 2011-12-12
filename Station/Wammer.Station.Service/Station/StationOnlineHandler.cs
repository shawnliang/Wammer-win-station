using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;

using Wammer.Model;
using Wammer.Cloud;

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
			string stationId = Parameters["station_id"];

			if (email == null || password == null || stationId == null)
			{
				logger.Error("email, password, or stationId is missing");
				throw new FormatException("email or password or stationId is missing");
			}

			Drivers driver = Drivers.collection.FindOne();
			if (driver == null || driver.email != email)
			{
				logger.Error("Driver is null or email inconsistent");
				throw new WammerStationException("Invalid driver", (int)StationApiError.InvalidDriver);
			}

			logger.DebugFormat("Station login with stationId = {0}, email = {1}", stationId, email);
			StationApi.LogOn(new WebClient(), stationId, email, password);

			this.functionServer.Start();

			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		} 
	}
}
