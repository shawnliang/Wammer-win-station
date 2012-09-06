using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.PostUpload;

namespace Wammer.Station.Notify
{
	public interface IPostUpsertNotifierDB
	{
		string GetUserIdByGroupId(string group_id);
	}

	public class PostUpsertNotifier
	{
		private INotifyChannels channels;
		private IPostUpsertNotifierDB db;
		public PostUpsertNotifier(INotifyChannels channels, IPostUpsertNotifierDB db)
		{
			this.channels = channels;
			this.db = db;
		}

		public void NotifyUser(string user_id)
		{
			try
			{
				channels.NotifyToUserChannels(user_id, "");
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("web socket notification failed", ex);
			}
		}

		public void OnPostUpserted(object sender, PostUpsertEventArgs evt)
		{
			try
			{
				channels.NotifyToUserChannels(evt.UserId, evt.SessionToken);
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("web socket notification failed", ex);
			}
		}

		public void OnPostRequestBypassed(object sender, BypassedEventArgs evt)
		{
			try
			{
				var respText = Encoding.UTF8.GetString(evt.BypassedResponse);
				var respObj = fastJSON.JSON.Instance.ToObject<MinimalPostResponse>(respText);

				if (respObj.post != null)
				{
					var userId = db.GetUserIdByGroupId(respObj.post.group_id);
					channels.NotifyToUserChannels(userId, respObj.session_token);
				}
				else
				{
					channels.NotifyToAllChannels(respObj.session_token);
				}
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("web socket notification failed", ex);
			}
		}
	}


	public class MinimalPostResponse
	{
		public MinimalPostResponse()
		{
			session_token = string.Empty;
		}

		public class Post
		{
			public string group_id { get; set; }
		}

		public string session_token { get; set; }
		public Post post { get; set; }
	}
}
