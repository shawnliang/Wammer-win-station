using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Wammer.MultiPart
{
	public class Disposition
	{
		private string value;
		private NameValueCollection parameters = new NameValueCollection();

		private static char[] SEPARATOR = new char[] { ';' };

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
	}
}
