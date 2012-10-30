using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StationSystemTray
{
	public interface IStreamSignUp
	{
		SignUpData ShowSignUpDialog();
	}

	public class SignUpData
	{
		public string user_id { get; set; }
		public string session_token { get; set; }
		public string email { get; set; }
		public string password { get; set; }
		public string account_type { get; set; }
	}
}
