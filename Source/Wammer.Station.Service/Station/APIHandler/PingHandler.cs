using MongoDB.Driver.Builders;
using System;
using Wammer.Cloud;
namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI | APIHandlerType.ManagementAPI, "/availability/ping/")]
	internal class PingHandler : HttpHandler
	{
		private static volatile bool syncEnabled = true;

		public override void HandleRequest()
		{
			var user_id = Parameters[CloudServer.PARAM_USER_ID];

			// For backward compatibility, reply "success" even if remote does not provide user_id
			if (string.IsNullOrEmpty(user_id))
			{
				RespondSuccess();
				return;
			}

			var user = Model.DriverCollection.Instance.FindOne(Query.EQ("_id", user_id));

			if (user != null && syncEnabled)
				RespondSuccess();
			else
				RespondError("User not belonging to this station: " + user_id, (int)StationLocalApiError.InvalidDriver);
		}

		public void OnSyncSuspended(object sender, EventArgs e)
		{
			syncEnabled = false;
		}

		public void OnSyncResumed(object sender, EventArgs e)
		{
			syncEnabled = true;
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}
}