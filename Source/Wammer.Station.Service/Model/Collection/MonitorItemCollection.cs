﻿
namespace Wammer.Model
{
	public class MonitorItemCollection : Collection<MonitorItem>
	{
		private static MonitorItemCollection instance;

		private MonitorItemCollection()
			: base("monitor_items")
		{

		}

		static MonitorItemCollection()
		{
			instance = new MonitorItemCollection();
		}

		public static MonitorItemCollection Instance
		{
			get { return instance; }
		}


	}
}