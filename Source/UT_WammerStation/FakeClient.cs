using System.IO;
using System.Net;
using System.Text;

namespace UT_WammerStation
{
	class FakeClientResult
	{
		public byte[] RawData { get; set; }
		public HttpStatusCode Status { get; set; }


		public FakeClientResult(HttpStatusCode status, byte[] rawData)
		{
			RawData = rawData;
			Status = status;
		}

		public string ResponseAsText
		{
			get
			{
				return Encoding.UTF8.GetString(RawData);
			}
		}
	}


	class FakeClient
	{
		public string ContentType { get; set; }
		public string Url { get; set; }


		public FakeClient(string url)
		{
			this.Url = url;
		}

		public FakeClient(string url, string contentType)
		{
			this.Url = url;
			this.ContentType = contentType;
		}

		public FakeClientResult PostFile(string filename)
		{
			HttpWebRequest requst = (HttpWebRequest)WebRequest.Create(Url);
			requst.ContentType = this.ContentType;
			requst.Method = "POST";

			using (FileStream fs = new FileStream(filename, FileMode.Open))
			using (Stream outStream = requst.GetRequestStream())
			{
				fs.CopyTo(outStream);
			}

			HttpWebResponse response = null;
			HttpStatusCode status = HttpStatusCode.OK;

			try
			{
				response = (HttpWebResponse)requst.GetResponse();
			}
			catch (WebException e)
			{
				response = (HttpWebResponse)e.Response;
				status = ((HttpWebResponse)e.Response).StatusCode;
			}

			using (MemoryStream output = new MemoryStream())
			using (Stream input = response.GetResponseStream())
			{
				input.CopyTo(output);
				return new FakeClientResult(status, output.ToArray());
			}
		}
	}
}
