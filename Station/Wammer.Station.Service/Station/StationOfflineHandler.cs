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

		public StationOfflineHandler(HttpServer functionServer)
		{
			this.functionServer = functionServer;
		}

		protected override void HandleRequest()
		{
			string session_token = Parameters["session_token"];

			if (session_token == null)
			{
				logger.Error("session_token is missing");
				throw new FormatException("session_token is missing");
			}

			LogOutStationFromCloud(session_token);
			
			functionServer.Stop();

			WriteOfflineStateToDB();

			RespondSuccess();
		}

		private static void WriteOfflineStateToDB()
		{
			Model.Service svc = ServiceCollection.FindOne(Query.EQ("_id", "StationService"));
			if (svc == null)
			{
				svc = new Model.Service { Id = "StationService", State = ServiceState.Offline };
			}
			else
				svc.State = ServiceState.Offline;

			ServiceCollection.Save(svc);
		}

		private static void LogOutStationFromCloud(string session_token)
		{
			logger.DebugFormat("Station logout with session_token = {0}", session_token);
			Model.StationInfo station = Model.StationInfo.collection.FindOne();
			if (station == null)
				throw new InvalidOperationException("station is null in station collection");

			Cloud.StationApi stationApi = new Cloud.StationApi(station.Id, session_token);
			stationApi.Offline(new System.Net.WebClient());
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
