using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Wammer.Utility
{
	static class MD5Helper
	{
		public static string ComputeMD5(byte[] data)
		{
			return ComputeMD5(data, 0, data.Length);
		}

		public static string ComputeMD5(byte[] data, int offset, int count)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(data, offset, count);
				var buff = new StringBuilder();
				for (int i = 0; i < hash.Length; i++)
					buff.Append(hash[i].ToString("x2"));

				return buff.ToString();
			}
		}
	}
}
