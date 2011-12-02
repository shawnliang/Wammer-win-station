using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace Wammer.MultiPart
{
	public class Parser
	{
		private static byte[] CRLF = Encoding.UTF8.GetBytes("\r\n");

		private byte[] head_boundry;
		private byte[] close_boundry;

		public Parser(string boundry)
		{
			head_boundry = Encoding.UTF8.GetBytes("--" + boundry);
			close_boundry = Encoding.UTF8.GetBytes("--" + boundry + "--");
		}

		public Part[] Parse(Stream stream)
		{
			List<Part> parts = new List<Part>();

			byte[] content = ToByteArray(stream);

			int startFrom = 0;
			while (startFrom < content.Length)
			{
				int index = IndexOf(content, startFrom, CRLF);
				if (index < 0)
					throw new FormatException("Not a wellformed multipart content");

				if (HasSubString(content, index + CRLF.Length, close_boundry))
				{
					return parts.ToArray();
				}
				else if (IsInFront(content, index, head_boundry))
				{
					int bodyLen;
					int bodyStartIdx = index + CRLF.Length;

					Part part = ParsePartBody(content, bodyStartIdx, out bodyLen);
					parts.Add(part);

					startFrom = bodyStartIdx + bodyLen;
					continue;
				}


				startFrom = index + 2;
			}

			throw new FormatException("Not a wellformed multipart content");
		}

		private Part ParsePartBody(byte[] data, int startIdx, out int partLen)
		{
			int index = 0;
			int startFrom = startIdx;

			bool headerFound = false;
			NameValueCollection headers = new NameValueCollection();
			int dataStartIndex = 0;
			while ((index = IndexOf(data, startFrom, CRLF)) >= 0)
			{
				if (!headerFound && IsInFront(data, index, CRLF))
				{
					headerFound = true;
					dataStartIndex = index + 2;

					if (index - startIdx > 0)
						ParseHeaders(headers, data, startIdx, index - startIdx);
				}

				if (headerFound && HasSubString(data, index + 2, head_boundry))
				{
					partLen = index - startIdx;
					return new Part(data, dataStartIndex, index - dataStartIndex, headers);
				}

				startFrom = index + 2;
			}

			throw new FormatException("Bad part body format");
		}

		private static void ParseHeaders(NameValueCollection collection, byte[] data, int from, int len)
		{
			string headerText = Encoding.UTF8.GetString(data, from, len);
			string[] stringSeparators = new string[] { "\r\n" };
			string[] headers = headerText.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

			foreach (string header in headers)
			{
				int delimitIdx = header.IndexOf(":");
				if (delimitIdx < 0)
					throw new FormatException("Bad header: " + header);

				string key = header.Substring(0, delimitIdx).Trim();
				string val = header.Substring(delimitIdx + 1).Trim();
				collection.Add(key, val);
			}
		}

		// is byte array "what" in front of EndInx?
		private static bool IsInFront(byte[] content, int endIdx, byte[] what)
		{
			int fromIdx = endIdx - what.Length;
			if (fromIdx < 0)
				return false;

			return CommonPrefixCount(content, fromIdx, what) == what.Length;
		}

		private static byte[] ToByteArray(Stream stream)
		{
			using (MemoryStream m = new MemoryStream())
			{
				Wammer.Utility.StreamHelper.Copy(stream, m);
				return m.ToArray();
			}
		}


		private static bool HasSubString(byte[] data, int startIdx, byte[] substr)
		{
			return CommonPrefixCount(data, startIdx, substr) == substr.Length;
		}

		// count the common prefix bytes of a byte array
		private static int CommonPrefixCount(byte[] data, int startIdx, byte[] what)
		{
			int commPrefixCount = 0;

			for (int i = 0; i < what.Length && startIdx + i < data.Length; i++)
			{
				if (data[startIdx + i] == what[i])
					commPrefixCount++;
				else
					break;
			}

			return commPrefixCount;
		}

		// find "what" in a byte array
		private static int IndexOf(byte[] data, int startIdx, byte[] whatBytes)
		{
			int startFrom = startIdx;
			while (startFrom < data.Length)
			{
				int commonPrefix = CommonPrefixCount(data, startFrom, whatBytes);
				if (commonPrefix == whatBytes.Length)
					return startFrom;

				if (commonPrefix > 0)
					startFrom += commonPrefix;
				else
					startFrom += 1;
			}

			return -1;
		}
	}
}
