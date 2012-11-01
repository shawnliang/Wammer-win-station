using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public class PersonalCloudStatus
	{
		public int PhotoCount { get; private set; }
		public int EventCount { get; private set; }

		public PersonalCloudStatus(int photos, int events)
		{
			PhotoCount = photos;
			EventCount = events;
		}
	}
}
