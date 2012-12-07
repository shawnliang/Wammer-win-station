using MongoDB.Driver.Builders;
using System;

namespace Wammer.Station.Notify
{
	class ConnectionUpdator
	{
		public void OnClientConnected(object sender, NotifyChannelEventArgs arg)
		{
			try
			{
				var connectionInfo = Cloud.User.GetLoginInfo(arg.UserId, arg.ApiKey, arg.SessionToken);
				Model.ConnectionCollection.Instance.Save(connectionInfo);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Unable to write connection info", e);
			}
		}

		public void OnClientDisconnected(object sender, NotifyChannelEventArgs arg)
		{
			try
			{
				Model.ConnectionCollection.Instance.Remove(Query.EQ("_id", arg.SessionToken));
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Unable to remove connection info", e);
			}
		}

		public void ClearAll()
		{
			Model.ConnectionCollection.Instance.RemoveAll();
		}
	}
}
