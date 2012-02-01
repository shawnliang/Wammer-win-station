using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

using Wammer.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class StationOfflineHandler : HttpHandler
	{
		private static ILog logger = LogManager.GetLogger("StationOfflineHandler");
		private readonly HttpServer functionServer;
		private readonly StationTimer stationTimer;

		public StationOfflineHandler(HttpServer functionServer, StationTimer stationTimer)
		{
			this.functionServer = functionServer;
			this.stationTimer = stationTimer;
		}

		protected override void HandleRequest()
		{
			// station.offline always stop function server
			// no matter if successfully logout from cloud
			LogOutStationFromCloud();

			logger.Debug("Stop function server");

			functionServer.Stop();
			stationTimer.Stop();

			logger.Debug("Stop function server successfully");
			RespondSuccess();
		}

		private static void LogOutStationFromCloud()
		{
			try
			{
				Model.StationInfo stationInfo = Model.StationCollection.Instance.FindOne();
				if (stationInfo == null)
					throw new InvalidOperationException("station is null in station collection");

				logger.DebugFormat("Station logout with stationId = {0}", stationInfo.Id);
				Cloud.StationApi stationApi = new Cloud.StationApi(stationInfo.Id, stationInfo.SessionToken);
				stationApi.Offline(new System.Net.WebClient());
				
				logger.Debug("Station logout successfully");
			}
			catch (Exception ex)
			{
				logger.Debug("Unable to logout station", ex);
			}
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
