using log4net;
using MongoDB.Driver.Builders;
using System;
using System.Text;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.PostUpload
{
	class MobileDevicePostActivity
	{
		private static ILog logger = LogManager.GetLogger("MobilePost");

		public void OnPostCreated(object sender, BypassedEventArgs args)
		{
			try
			{
				var responseText = Encoding.UTF8.GetString(args.BypassedResponse);
				var responseObj = fastJSON.JSON.Instance.ToObject<NewPostResponse>(responseText);

				PostCollection.Instance.Save(responseObj.post);
			}
			catch (Exception e)
			{
				logger.Warn("Unable to handle mobile device post event", e);
			}
		}

		public void OnPostUpdated(object sender, BypassedEventArgs args)
		{
			try
			{
				var responseText = Encoding.UTF8.GetString(args.BypassedResponse);
				var responseObj = fastJSON.JSON.Instance.ToObject<UpdatePostResponse>(responseText);

				PostCollection.Instance.Update(responseObj.post);
			}
			catch (Exception e)
			{
				logger.Warn("Unable to handle mobile device post event", e);
			}
		}

		public void OnPostHidden(object sender, BypassedEventArgs args)
		{
			try
			{
				var responseText = Encoding.UTF8.GetString(args.BypassedResponse);
				var responseObj = fastJSON.JSON.Instance.ToObject<HidePostResponse>(responseText);

				PostCollection.Instance.Update(Query.EQ("_id", responseObj.post_id), Update.Set("hidden", "true"));
			}
			catch (Exception e)
			{
				logger.Warn("Unable to handle mobile device post event", e);
			}
		}

		public void OnCommentCreated(object sender, BypassedEventArgs args)
		{
			try
			{
				var responseText = Encoding.UTF8.GetString(args.BypassedResponse);
				var responseObj = fastJSON.JSON.Instance.ToObject<NewPostCommentResponse>(responseText);

				PostCollection.Instance.Update(responseObj.post);
			}
			catch (Exception e)
			{
				logger.Warn("Unable to handle mobile device post event", e);
			}
		}
	}
}
