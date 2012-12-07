using System;

namespace Wammer.Station.Doc
{
	class MonitorAddHandlerUtility : IMonitorAddHandlerUtility
	{
		public void UpdateDocOpenTimeAsync(string session, string apikey, string object_id, DateTime openTime)
		{
			Cloud.AttachmentApi.updateDocMetadata(session, apikey, object_id, openTime);
		}
	}
}
