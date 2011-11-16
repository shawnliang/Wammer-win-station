using System;
using System.Text;

namespace Waveface
{
	internal sealed class Base64Helper
	{
		internal static string FromBase64String(string input)
		{
			return FromBase64String(input, new UTF8Encoding());
		}
		
		internal static string FromBase64String(string input, Encoding encoding)
		{
			try
			{
				byte[] _bytes = Convert.FromBase64String(input);
				return encoding.GetString(_bytes, 0, _bytes.Length);
			}
			catch
			{
				return "Error";
			}
		}

		internal static string ToBase64String(string input)
		{
			return ToBase64String(input, new UTF8Encoding());
		}

		internal static string ToBase64String(string input, Encoding encoding)
		{
			try
			{
				byte[] _bytes = encoding.GetBytes(input);
				return Convert.ToBase64String(_bytes, 0, _bytes.Length);
			}
			catch
			{
				return "RXJyb31=";
			}
		}
	}
}