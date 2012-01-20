using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Wammer.Cloud;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	class RemoveOwnerHandler : HttpHandler
	{
		private readonly string stationId;
		private readonly HttpServer functionServer;
		private readonly StationTimer stationTimer;

		public RemoveOwnerHandler(string stationId, HttpServer functionServer, StationTimer stationTimer)
		{
			this.stationId = stationId;
			this.functionServer = functionServer;
			this.stationTimer = stationTimer;
		}

		protected override void HandleRequest()
		{
			string stationToken = Parameters["session_token"];

			if (stationToken == null)
				throw new FormatException("One of parameters is missing: email/password/session_token");

			StationApi.SignOff(new WebClient(), stationId, stationToken);

			functionServer.Stop();
			stationTimer.Stop();

			Model.DriverCollection.Instance.RemoveAll();
			Model.StationCollection.Instance.RemoveAll();

			//TODO: move to ServiceCollection
			Model.Service service = Model.ServiceCollection.Instance.FindOne(Query.EQ("_id", "StationService"));
			if (service != null)
			{
				service.State = Model.ServiceState.Offline;
				Model.ServiceCollection.Instance.Save(service);
			}

			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
