using System;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using System.IO;

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
			bool removeResource = bool.Parse(Parameters["remove_resource"]);

			if (stationToken == null || userID == null)
				throw new FormatException("One of parameters is missing: email/password/session_token/userID");

			//Try to find existing driver
			Driver existingDriver = Model.DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
			Boolean isDriverExists = existingDriver != null;

			//Driver not exists => return
			if (!isDriverExists)
			{
				RespondSuccess();
				return;
			}

			//reference count > 1 => reference count decrease one
			if (existingDriver.ref_count > 1)
			{
				--existingDriver.ref_count;
				DriverCollection.Instance.Save(existingDriver);
				RespondSuccess();
				return;
			}
	  
			//Notify cloud server that the user signoff
			using (WebClient client = new Wammer.Utility.DefaultWebClient())
			{
				StationApi.SignOff(client, stationId, stationToken, userID);
			}

			//Remove the user from db, and stop service this user
			Model.DriverCollection.Instance.Remove(Query.EQ("_id", userID));

			if (removeResource && Directory.Exists(existingDriver.folder))
				Directory.Delete(existingDriver.folder, true);

			//All driver removed => Remove station from db
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
