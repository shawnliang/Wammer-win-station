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
        //private readonly HttpServer functionServer;
        //private readonly StationTimer stationTimer;

		public RemoveOwnerHandler(string stationId, HttpServer functionServer, StationTimer stationTimer)
		{
			this.stationId = stationId;
            //this.functionServer = functionServer;
            //this.stationTimer = stationTimer;
		}

		protected override void HandleRequest()
		{
			string stationToken = Parameters["session_token"];
            string userID = Parameters["userID"];

            if (stationToken == null || userID == null)
                throw new FormatException("One of parameters is missing: email/password/session_token/userID");

            StationApi.SignOff(new WebClient(), stationId, stationToken, userID);

            //functionServer.Stop();
            //stationTimer.Stop();

            Model.DriverCollection.Instance.Remove(Query.EQ("_id", userID));
			Model.StationCollection.Instance.RemoveAll();

            //Larry 2012/03/02 Mark, multiple user use the same service
            ////TODO: move to ServiceCollection
            //Model.Service service = Model.ServiceCollection.Instance.FindOne(Query.EQ("_id", "StationService"));
            //if (service != null)
            //{
            //    service.State = Model.ServiceState.Offline;
            //    Model.ServiceCollection.Instance.Save(service);
            //}

			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
