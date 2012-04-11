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
		private Timeline.TimelineSyncer syncer;
		private ResourceDownloader downloader;

		public ResourceSyncer(long timerPeriod, ITaskStore bodySyncQueue)
			: base(timerPeriod)
		{
			this.downloader = new ResourceDownloader(bodySyncQueue);
			this.syncer = new Timeline.TimelineSyncer(new Timeline.PostProvider(), new Timeline.TimelineSyncerDB(), new UserTracksApi());
			syncer.PostsRetrieved += new EventHandler<Timeline.TimelineSyncEventArgs>(downloader.PostRetrieved);
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
