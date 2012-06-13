using MongoDB.Driver.Builders;
using Wammer.Cloud;
namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.FunctionAPI | APIHandlerType.ManagementAPI, "/availability/ping/")]
	internal class PingHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			var user_id = Parameters[CloudServer.PARAM_USER_ID];

			// For backward compatibility, reply "success" even if remote does not provide user_id
			if (string.IsNullOrEmpty(user_id))
				RespondSuccess();

			var user = Model.DriverCollection.Instance.FindOne(Query.EQ("_id", user_id));

			if (user != null)
				RespondSuccess();
			else
				RespondError("User not belonging to this station: " + user_id, (int)StationLocalApiError.InvalidDriver);
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}
}