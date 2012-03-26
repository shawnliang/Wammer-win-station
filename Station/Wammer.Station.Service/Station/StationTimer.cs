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

	public class StationTimer
	{
#if DEBUG
		private const long STATUS_CHECK_PERIOD = 30 * 1000; // run heartbeat frequently in debug mode
#else
		private const long STATUS_CHECK_PERIOD = 10 * 60 * 1000; // TODO: remove hardcode
#endif

#if DEBUG
		private const long RESOURCE_SYNC_PEROID = 10 * 1000;
#else
		private const long RESOURCE_SYNC_PEROID = 60 * 1000;
#endif

		private List<IStationTimer> timers;

		public StationTimer(HttpServer functionServer)
		{
			timers = new List<IStationTimer> {
				new StatusChecker(STATUS_CHECK_PERIOD, functionServer),
				new ResourceSyncer(RESOURCE_SYNC_PEROID),
				// Use a strange value to make ResourceSyncer and ChangeHistorySyncer not running at the same time.
				new ChangeHistorySyncer(RESOURCE_SYNC_PEROID * 2 - 10 * 1000)
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

		public void Stop()
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
