using System;

namespace Waveface.Stream.WindowsClient
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
