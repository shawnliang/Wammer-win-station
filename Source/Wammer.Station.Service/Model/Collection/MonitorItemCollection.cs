
namespace Wammer.Model
{
	public class MonitorItemCollection : Collection<MonitorItem>
	{
		private static MonitorItemCollection instance = new MonitorItemCollection();

		private MonitorItemCollection()
			: base("monitor_items")
		{
		}

		public static MonitorItemCollection Instance
		{
			get { return instance; }
		}


	}
}
