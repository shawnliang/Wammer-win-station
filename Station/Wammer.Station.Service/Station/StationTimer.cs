using System.Collections.Generic;
using System.Threading;
using System;

namespace Wammer.Station
{
	public interface IStationTimer
	{
		void Start();
		void Stop();
		void Close();
	}

	class StationTimer
	{
#if DEBUG
		private const long STATUS_CHECK_PERIOD = 30 * 1000; // run heartbeat frequently in debug mode
#else
		private const long STATUS_CHECK_PERIOD = 10 * 60 * 1000; // TODO: remove hardcode
#endif

		private const long RESOURCE_SYNC_PEROID = 10 * 1000;

		private List<IStationTimer> timers;

		public StationTimer(ITaskEnqueuable bodySyncQueue, string stationId)
		{
			var resourceSyncer = new ResourceSyncer(RESOURCE_SYNC_PEROID, bodySyncQueue, stationId);
			var statusChecker = new StatusChecker(STATUS_CHECK_PERIOD);
			statusChecker.IsPrimaryChanged += new EventHandler<IsPrimaryChangedEvtArgs>(resourceSyncer.OnIsPrimaryChanged);
			var taskRetryTimer = new TaskRetryTimer();

			timers = new List<IStationTimer> {
				resourceSyncer,
				statusChecker,
				taskRetryTimer
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

	abstract public class NonReentrantTimer: IStationTimer
	{
		private System.Threading.Timer timer;
		private long timerPeriod;

		protected NonReentrantTimer(long timerPeriod)
		{
			this.timer = new Timer(this.OnTimedUp);
			this.timerPeriod = timerPeriod;
		}

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

		private void OnTimedUp(object state)
		{
			try
			{
				ExecuteOnTimedUp(state);
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger(typeof(NonReentrantTimer)).Warn(e);
			}
			finally
			{
				timer.Change(timerPeriod, Timeout.Infinite);
			}
		}

		protected abstract void ExecuteOnTimedUp(object state);
	}
}
