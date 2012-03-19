
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
		private const long RESOURCE_SYNC_PEROID = 10 * 20 * 1000;
#endif

		private List<IStationTimer> timers;

		public StationTimer(HttpServer functionServer)
		{
			timers = new List<IStationTimer> {
				new StatusChecker(STATUS_CHECK_PERIOD, functionServer), 
				new ResourceSyncer(RESOURCE_SYNC_PEROID)
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
}
