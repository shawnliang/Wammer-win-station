using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Wammer.Utility
{
	static class MD5Helper
	{
		public static string ComputeMD5(Stream data)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(data);
				return getHashString(hash);
			}
		}

		public static string ComputeMD5(byte[] data)
		{
			return ComputeMD5(data, 0, data.Length);
		}

		public static string ComputeMD5(byte[] data, int offset, int count)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(data, offset, count);
				return getHashString(hash);
			}
		}

		private static string getHashString(byte[] hash)
		{
			var buff = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
				buff.Append(hash[i].ToString("x2"));

			return buff.ToString();
		}
	}
}
