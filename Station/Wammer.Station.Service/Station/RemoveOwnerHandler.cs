using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Wammer.Cloud;
using MongoDB.Driver.Builders;
using Wammer.Model;

namespace Wammer.Station
{
	public class RemoveOwnerHandler : HttpHandler
	{
		private readonly string stationId;

		public RemoveOwnerHandler(string stationId)
		{
			this.stationId = stationId;
		}

		protected override void HandleRequest()
		{
			string stationToken = Parameters["session_token"];
            string userID = Parameters["user_ID"];

            if (stationToken == null || userID == null)
                throw new FormatException("One of parameters is missing: email/password/session_token/userID");

            StationApi.SignOff(new WebClient(), stationId, stationToken, userID);

      

            Model.DriverCollection.Instance.Remove(Query.EQ("_id", userID));

			Driver driver = Model.DriverCollection.Instance.FindOne();
			if (driver == null)
				Model.StationCollection.Instance.RemoveAll();


			RespondSuccess();
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
