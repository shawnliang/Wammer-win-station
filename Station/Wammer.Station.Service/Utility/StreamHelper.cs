using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Wammer.Utility
{
	static class StreamHelper
	{
		public static void Copy(Stream from, Stream to)
		{
			byte[] buffer = new byte[32768];
			int nRead;

			while ((nRead = from.Read(buffer, 0, buffer.Length))>0)
			{
				to.Write(buffer, 0, nRead);
			}
		}
	}
}
