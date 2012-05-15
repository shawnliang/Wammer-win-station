using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public class AccountEventArgs:EventArgs
	{
		public String EMail { get; private set; }

		public AccountEventArgs(string email)
		{
			this.EMail = email;
		}
	}
}
