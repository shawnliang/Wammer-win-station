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
		public RemoveOwnerHandler(string stationId, HttpServer functionServer)
		{
			this.stationId = stationId;
			this.functionServer = functionServer;
		}

		protected override void HandleRequest()
		{
			string email = Parameters["email"];
			string password = Parameters["password"];
			string stationToken = Parameters["session_token"];

			if (email == null || password == null || stationToken == null)
				throw new FormatException("One of parameters is missing: email/password/session_token");

			StationApi.SignOff(new WebClient(), stationId, stationToken);

			functionServer.Stop();

			Model.Drivers.collection.RemoveAll();
			Model.StationInfo.collection.RemoveAll();

			//TODO: move to ServiceCollection
			Model.Service service = Model.ServiceCollection.FindOne(Query.EQ("_id", "StationService"));
			if (service != null)
			{
				service.State = Model.ServiceState.Offline;
				Model.ServiceCollection.Save(service);
			}

			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
