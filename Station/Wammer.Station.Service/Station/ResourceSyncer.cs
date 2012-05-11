using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Drawing;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Cloud;
using Wammer.Utility;

namespace Wammer.Station
{
	class ResourceSyncer : NonReentrantTimer
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ResourceSyncer));
		private bool isFirstRun = true;
		private readonly Timeline.TimelineSyncer syncer;
		private readonly ResourceDownloader downloader;

		public ResourceSyncer(long timerPeriod, ITaskEnqueuable<INamedTask> bodySyncQueue, string stationId)
			: base(timerPeriod)
		{
			this.downloader = new ResourceDownloader(bodySyncQueue, stationId);
			this.syncer = new Timeline.TimelineSyncer(new Timeline.PostProvider(), new Timeline.TimelineSyncerDB(), new UserTracksApi());
			syncer.PostsRetrieved += new EventHandler<Timeline.TimelineSyncEventArgs>(downloader.PostRetrieved);
			syncer.BodyAvailable += new EventHandler<Timeline.BodyAvailableEventArgs>(syncer_BodyAvailable);
		}

		void syncer_BodyAvailable(object sender, Timeline.BodyAvailableEventArgs e)
		{
			try
			{
				Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", e.user_id));
				if (user != null && user.isPrimaryStation)
				{
					using (WebClient agent = new DefaultWebClient())
					{
						AttachmentInfo info = Cloud.AttachmentApi.GetInfo(agent, e.object_id, user.session_token);
						downloader.EnqueueDownstreamTask(info, user, ImageMeta.Origin);
					}
				}
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("Unable to enqueue body download task", ex);
			}
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			if (isFirstRun)
			{
				downloader.ResumeUnfinishedDownstreamTasks();
				isFirstRun = false;
			}

			PullTimeline();
		}

		public void OnIsPrimaryChanged(object sender, IsPrimaryChangedEvtArgs args)
		{
			if (args.driver.isPrimaryStation)
			{
				// just upgraded to primary station
				foreach (var attachment in AttachmentCollection.Instance.FindAll())
				{
					downloader.EnqueueDownstreamTask(new AttachmentInfo(attachment), args.driver, ImageMeta.Origin);
				}
			}
		}

		private void PullTimeline()
		{
			MongoCursor<Driver> users = DriverCollection.Instance.FindAll();

			foreach (Driver user in users)
			{
				try
				{
					syncer.PullTimeline(user);
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Unable to sync timeline of user " + user.email, e);
				}
			}
		}
	}
}
