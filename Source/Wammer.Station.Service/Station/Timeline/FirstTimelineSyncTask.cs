using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Model;
using MongoDB.Driver.Builders;

namespace Wammer.Station.Timeline
{
	public class FirstTimelineSyncTask : Retry.DelayedRetryTask
	{
		public Driver user { get; set; }

		public FirstTimelineSyncTask()
			:base(TaskPriority.High)
		{

		}

		protected override void Run()
		{
			if (DriverCollection.Instance.FindOneById(user.user_id) == null)
				return;

			var resSyncer = new ResourceSyncer(0, BodySyncQueue.Instance);
			resSyncer.SyncTimeline(user);

			ResourceSyncer.EnablePeriodicalSync(user.user_id);
		}

		public override void ScheduleToRun()
		{
			TaskQueue.Enqueue(this, TaskPriority.High, true);
		}
	}
}
