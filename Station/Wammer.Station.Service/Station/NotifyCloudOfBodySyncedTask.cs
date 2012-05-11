using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Utility;

namespace Wammer.Station
{
	[Serializable]
	class NotifyCloudOfBodySyncedTask: ITask
	{
		private readonly string object_id;
		private readonly string session_token;

		public NotifyCloudOfBodySyncedTask(string object_id, string session_token)
		{
			if (object_id == null || session_token == null)
				throw new ArgumentNullException();

			this.object_id = object_id;
			this.session_token = session_token;
		}

		public void Execute()
		{
			using (var agent = new DefaultWebClient())
			{
				Cloud.AttachmentApi.SetSync(agent, object_id, session_token);
			}
		}
	}
}
