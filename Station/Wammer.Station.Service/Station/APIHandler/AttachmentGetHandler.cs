using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station
{
	public class AttachmentGetHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_OBJECT_ID);

			var object_id = Parameters[CloudServer.PARAM_OBJECT_ID];

			var doc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));

			if (doc == null)
				throw new WammerStationException("Resource not found: " + object_id, (int) StationLocalApiError.NotFound);


			RespondSuccess(new AttachmentResponse(doc));
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}
}