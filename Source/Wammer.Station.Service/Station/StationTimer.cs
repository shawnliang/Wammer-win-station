using log4net;
using System;
using System.Collections.Generic;
using System.Threading;
using Wammer.Station.Doc;
using Wammer.Station.Timeline;

namespace Wammer.Station
{
	public interface IStationTimer
	{
		void Start();
		void Stop();
		void Close();
	}

	internal class StationTimer
	{
#if DEBUG
		private const long STATUS_CHECK_PERIOD = 30 * 1000; // run heartbeat frequently in debug mode
#else
		private const long STATUS_CHECK_PERIOD = 60 * 1000; // TODO: remove hardcode
#endif

		private const long RESOURCE_SYNC_PEROID = 10 * 1000;

		private readonly List<IStationTimer> timers;

		public StationTimer(ITaskEnqueuable<IResourceDownloadTask> bodySyncQueue)
		{
			var resourceSyncer = new ResourceSyncer(RESOURCE_SYNC_PEROID, bodySyncQueue);
			var statusChecker = new StatusChecker(STATUS_CHECK_PERIOD);
			statusChecker.IsPrimaryChanged += resourceSyncer.OnIsPrimaryChanged;
			var taskRetryTimer = new TaskRetryTimer();
			var docMonitor = new CheckDocumentChangeTimer();

			timers = new List<IStationTimer> {
				resourceSyncer,
				statusChecker,
				taskRetryTimer,
				docMonitor
			};
		}

		public void Start()
		{
			foreach (IStationTimer timer in timers)
			{
				timer.Start();
			}
		}

		public void Stop()
		{
			foreach (IStationTimer timer in timers)
			{
				timer.Stop();
			}
		}

		public void Close()
		{
			foreach (IStationTimer timer in timers)
			{
				timer.Close();
			}
		}
	}

	public abstract class NonReentrantTimer : IStationTimer
	{
		private readonly Timer timer;
		private readonly long timerPeriod;

		protected NonReentrantTimer(long timerPeriod)
		{
			timer = new Timer(OnTimedUp);
			this.timerPeriod = timerPeriod;
		}

		#region IStationTimer Members

		public void Start()
		{
			timer.Change(0, Timeout.Infinite);
		}

		public virtual void Stop()
		{
			timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		public void Close()
		{
			timer.Dispose();
		}

		#endregion

		private void OnTimedUp(object state)
		{
			try
			{
				ExecuteOnTimedUp(state);
			}
			catch (Exception e)
			{
				LogManager.GetLogger(typeof(NonReentrantTimer)).Warn(e);
			}
			finally
			{
				timer.Change(timerPeriod, Timeout.Infinite);
			}
		}

		protected abstract void ExecuteOnTimedUp(object state);
	}
}