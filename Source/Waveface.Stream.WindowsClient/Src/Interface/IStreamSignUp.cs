using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface.Stream.WindowsClient
{
	public interface IStreamSignUp
	{
		void ShowSignUpPage(WebBrowser browser);
		SignUpData TryParseSignUpDataFromUrl(Uri uri);
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
