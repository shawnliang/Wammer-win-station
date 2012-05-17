using System;
using System.IO;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class RemoveOwnerHandler : HttpHandler
	{
		private readonly string stationId;

		public RemoveOwnerHandler(string stationId)
		{
			this.stationId = stationId;
		}

		public override void HandleRequest()
		{
			string userID = Parameters["user_ID"];
			bool removeResource = bool.Parse(Parameters["remove_resource"]);

			if (userID == null)
				throw new FormatException("user_ID is missing");

			//Try to find existing driver
			Driver existingDriver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
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
			using (WebClient client = new DefaultWebClient())
			{
				try
				{
					StationApi.SignOff(client, stationId, existingDriver.session_token, userID);
				}
				catch (WammerCloudException e)
				{
					this.LogWarnMsg(string.Format("Unable to notify cloud to unlink user {0} from this computer", userID), e);

					// continue removing user even if session expired
					if (e.WammerError != (int)GeneralApiError.SessionNotExist)
						throw;
				}
			}

			//Remove the user from db, and stop service this user
			DriverCollection.Instance.Remove(Query.EQ("_id", userID));

			//Remove login session if existed
			LoginedSessionCollection.Instance.Remove(Query.EQ("_id", existingDriver.session_token));

			//Remove all user data
			if (removeResource)
			{
				if (Directory.Exists(existingDriver.folder))
				{
					Directory.Delete(existingDriver.folder, true);
				}
				foreach (PostInfo post in PostCollection.Instance.Find(Query.EQ("creator_id", userID)))
				{
					foreach (String attachmentId in post.attachment_id_array)
					{
						AttachmentCollection.Instance.Remove(Query.EQ("_id", attachmentId));
					}
				}
				PostCollection.Instance.Remove(Query.EQ("creator_id", userID));
			}

			//All driver removed => Remove station from db
			Driver driver = DriverCollection.Instance.FindOne();
			if (driver == null)
				StationCollection.Instance.RemoveAll();

			RespondSuccess();
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}
}