using System;
using System.Security.Cryptography;
using System.Text;

namespace StationSystemTray
{
	public class SecurityHelper
	{
		private static readonly byte[] secret = {1, 2, 3, 4, 5, 6};

		public static string EncryptPassword(string plainPassword)
		{
			return
				Convert.ToBase64String(ProtectedData.Protect(Encoding.Default.GetBytes(plainPassword), secret,
				                                             DataProtectionScope.CurrentUser));
		}

		public static string DecryptPassword(string cipherPassword)
		{
			return
				Encoding.Default.GetString(ProtectedData.Unprotect(Convert.FromBase64String(cipherPassword), secret,
				                                                   DataProtectionScope.CurrentUser));
		}
	}
}