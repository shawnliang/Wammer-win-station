using System;
using Wammer.Cloud;
using Wammer.Utility;

namespace Wammer.Station
{
	[Serializable]
	internal class NotifyCloudOfBodySyncedTask : ITask
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

		#region ITask Members

		public void Execute()
		{
			using (var agent = new DefaultWebClient())
			{
				AttachmentApi.SetSync(agent, object_id, session_token);
			}
		}

		#endregion
	}
}