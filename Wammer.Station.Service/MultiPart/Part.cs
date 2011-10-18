using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace Wammer.MultiPart
{
	public class Part
	{
		public static byte[] DASH_DASH = Encoding.UTF8.GetBytes("--");
		public static byte[] CRLF = Encoding.UTF8.GetBytes("\r\n");

		private byte[] data;
		private int start;
		private int len;

		private string text;
		private byte[] bytes;
		private NameValueCollection headers;
		private Disposition disposition;

		public Part(byte[] data, int start, int len, NameValueCollection headers)
		{
			if (data == null || headers == null)
				throw new ArgumentNullException();

			this.data = data;
			this.start = start;
			this.len = len;
			this.headers = headers;

			if (headers["content-disposition"] != null)
			{
				disposition = Disposition.Parse(headers["content-disposition"]);
			}
		}

		public Part(byte[] data, int start, int len)
		{
			if (data == null)
				throw new ArgumentNullException();

			this.data = data;
			this.start = start;
			this.len = len;
			this.headers = new NameValueCollection();
		}


		public string Text
		{
			get
			{
				if (headers["content-transfer-encoding"] != null &&
					headers["content-transfer-encoding"].Equals("binary"))
					return null;
				if (text == null)
					text = Encoding.UTF8.GetString(data, start, len);

				return text;
			}
		}

		public byte[] Bytes
		{
			get
			{
				if (bytes == null)
				{
					bytes = new byte[this.len];
					for (int i = 0; i < this.len; i++)
						bytes[i] = this.data[this.start + i];
				}

				return bytes;
			}
		}

		public void CopyTo(Stream output, byte[] boundaryData)
		{
			output.Write(CRLF, 0, CRLF.Length);
			output.Write(DASH_DASH, 0, DASH_DASH.Length);
			output.Write(boundaryData, 0, boundaryData.Length);
			output.Write(CRLF, 0, CRLF.Length);

			if (disposition != null)
				disposition.CopyTo(output);

			foreach (string name in headers.AllKeys)
			{
				if (disposition != null && name.ToLower().Equals(
														"content-disposition"))
					continue;


				string hdr = name + ":" + headers[name];
				byte[] hdrData = Encoding.UTF8.GetBytes(hdr);

				output.Write(hdrData, 0, hdr.Length);
				output.Write(Part.CRLF, 0, Part.CRLF.Length);
			}

			output.Write(CRLF, 0, CRLF.Length);

			output.Write(data, start, len);
		}

		public NameValueCollection Headers
		{
			get { return headers; }
		}

		public Disposition ContentDisposition
		{
			get { return disposition; }
			set { disposition = value; }
		}
	}
}
