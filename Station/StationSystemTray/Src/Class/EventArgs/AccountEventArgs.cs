using System;

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
