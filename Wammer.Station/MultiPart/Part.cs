using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Wammer.MultiPart
{
	public class Part
	{
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

		public NameValueCollection Headers
		{
			get { return headers; }
		}

		public Disposition ContentDisposition
		{
			get { return disposition; }
		}
	}
}
