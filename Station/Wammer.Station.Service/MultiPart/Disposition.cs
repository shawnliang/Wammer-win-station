using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace Wammer.MultiPart
{
	public class Disposition
	{
		private string value;
		private NameValueCollection parameters = new NameValueCollection();

		private static char[] SEPARATOR = new char[] { ';' };

		public Disposition(string value)
		{
			this.value = value;
		}

		private Disposition()
		{

		}

		public static Disposition Parse(string text)
		{
			try
			{
				string[] segments = text.Split(SEPARATOR,
										StringSplitOptions.RemoveEmptyEntries);

				Disposition disp = new Disposition();
				disp.value = segments[0].Trim();
				for (int i = 1; i < segments.Length; i++)
				{
					string[] nameValue = segments[i].Split('=');

					string paramValue = RemoveDoubleQuote(nameValue[1].Trim());
					disp.parameters.Add(nameValue[0].Trim(), paramValue);
				}

				return disp;
			}
			catch (Exception e)
			{
				throw new FormatException(
							"Incorrect content disposition format: " + text, e);
			}
		}

		public static string RemoveDoubleQuote(string str)
		{
			if (str.StartsWith("\"") && str.EndsWith("\""))
				return str.Substring(1, str.Length - 2);
			else
				return str;
		}

		public string Value
		{
			get { return value; }
		}

		public NameValueCollection Parameters
		{
			get { return parameters; }
		}

		public void CopyTo(Stream output)
		{
			StringBuilder buff = new StringBuilder();
			buff.Append("Content-Disposition: ");
			buff.Append(this.value);
			if (parameters.Count>0)
			{
				foreach (string key in parameters.AllKeys)
				{
					buff.Append(";");
					buff.Append(key);
					buff.Append("=");
					buff.Append("\"");
					buff.Append(parameters[key]);
					buff.Append("\"");
				}
			}
			buff.Append("\r\n");

			byte[] data = Encoding.UTF8.GetBytes(buff.ToString());
			output.Write(data, 0, data.Length);

		}
	}
}
