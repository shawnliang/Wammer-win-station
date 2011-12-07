using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class WammerStationException : Exception
	{
		private int wammerError;

		public WammerStationException(string msg, int wammerError)
			:base(msg)
		{
			this.wammerError = wammerError;
		}

		public int WammerError
		{
			get { return wammerError; }
		}
	}
}
