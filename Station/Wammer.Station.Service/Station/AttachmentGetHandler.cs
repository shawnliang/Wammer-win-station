using System;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class AttachmentGetHandler: HttpHandler
	{
		protected override void HandleRequest()
		{
			string object_id = Parameters["object_id"];
			//string session_token = Parameters["session_token"];
			//string apikey = Parameters["apikey"];

			if (object_id == null)
				throw new FormatException("missing parameter: object_id");

			Attachment doc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", object_id));
			if (doc == null)
				throw new WammerStationException("Resource not found: " + object_id, (int)StationApiError.NotFound);

			
			RespondSuccess(new AttachmentResponse(doc));
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
