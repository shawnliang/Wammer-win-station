using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Wammer.MultiPart
{
	public class Serializer
	{
		private readonly Stream output;
		private readonly string boundary;
		private readonly byte[] boundaryData;

		public Serializer(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException();

			output = stream;
			boundary = Guid.NewGuid().ToString("N");
			boundaryData = Encoding.UTF8.GetBytes(boundary);
		}

		public void Put(Part part)
		{
			part.CopyTo(output, boundaryData);
		}

		public void Put(Part[] parts)
		{
			for (int i = 0; i < parts.Length; i++)
				Put(parts[i]);
		}

		public string Boundary
		{
			get { return boundary; }
		}

		public void Close()
		{
			output.Write(Part.CRLF, 0, Part.CRLF.Length);
			output.Write(Part.DASH_DASH, 0, Part.DASH_DASH.Length);
			output.Write(boundaryData, 0, boundaryData.Length);
			output.Write(Part.DASH_DASH, 0, Part.DASH_DASH.Length);
			output.Write(Part.CRLF, 0, Part.CRLF.Length);
		}
	}
}