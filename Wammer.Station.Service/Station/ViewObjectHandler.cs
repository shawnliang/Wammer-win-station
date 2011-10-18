using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;

namespace Wammer.Station
{
	public class ViewObjectHandler: IHttpHandler
	{
		private string baseDir;

		public ViewObjectHandler(string resourceFolder)
		{
			baseDir = resourceFolder;
		}

		public void Handle(object state)
		{
			HttpListenerContext ctx = (HttpListenerContext)state;

			NameValueCollection parameters = new NameValueCollection();
			if (ctx.Request.HttpMethod.ToUpper().Equals("POST"))
			{
				using (StreamReader s = new StreamReader(ctx.Request.InputStream))
				{
					string postData = s.ReadToEnd();
					parameters = HttpUtility.ParseQueryString(postData);
				}
			}

			string objectId = parameters["object_id"];
			FileType type = (FileType)Enum.Parse(typeof(FileType), parameters["file_type"]);

			string spaceDir = Path.Combine(baseDir, "space1");
			string typeDir = Path.Combine(spaceDir, type.ToString("d"));
			string[] files = Directory.GetFiles(typeDir, objectId + ".*");

			string filename = files[0];

			ctx.Response.StatusCode = 200;
			ctx.Response.ContentType = "image/jpeg";

			byte[] buffer = new byte[32768];
			int nRead = 0;
			using (FileStream fs = File.OpenRead(filename))
			{
				while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
				{
					ctx.Response.OutputStream.Write(buffer, 0, nRead);
				}
			}
			ctx.Response.OutputStream.Close();
		}
	}
}
