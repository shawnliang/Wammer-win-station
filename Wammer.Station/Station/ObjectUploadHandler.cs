using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using Wammer.MultiPart;
using Wammer.Cloud;

namespace Wammer.Station
{
	public class ObjectUploadHandler : IHttpHandler
	{
		private FileStorage storage;
		private const string BOUNDARY = "boundary=";
		private const string NAME = "name=";

		public ObjectUploadHandler()
		{
			storage = new FileStorage("resource");
		}

		public void Handle(object state)
		{
			HttpListenerContext context = (HttpListenerContext)state;

			int idx = context.Request.ContentType.IndexOf(BOUNDARY);
			string boundary = context.Request.ContentType.Substring(
														idx + BOUNDARY.Length);

			MultiPart.Parser parser = new Parser(boundary);
			Part[] parts = parser.Parse(context.Request.InputStream);

			string filename = null;
			FileType type = FileType.ImgOriginal;
			string objectId = null;
			byte[] fileContent = null;

			for (int i = 0; i < parts.Length; i++)
			{
				string disposition = parts[i].Headers["Content-Disposition"];
				if (disposition != null)
				{
					int index = disposition.ToLower().IndexOf(NAME);
					if (index < 0)
						continue;
					string field = disposition.Substring(index + NAME.Length);

					switch (field)
					{
						case "\"file\"":
							fileContent = parts[i].Bytes;
							break;
						case "\"filename\"":
							filename = parts[i].Text;
							break;
						case "\"filetype\"":
							type = (FileType)Enum.Parse(typeof(FileType), parts[i].Text);
							break;
						case "\"object_id\"":
							objectId = parts[i].Text;
							break;
						default:
							continue;
					}
				}
			}

			storage.Save("space1", type, filename, fileContent);

			ObjectUploadResponse response = new ObjectUploadResponse();
			response.app_ret_code = 0;
			response.app_ret_msg = "Success";
			response.http_status = 200;
			response.object_id = objectId;
			response.timestamp = DateTime.Now.ToUniversalTime();

			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/json";

			using (StreamWriter w = new StreamWriter(context.Response.OutputStream))
			{
				string resText = fastJSON.JSON.Instance.ToJSON(response, false, false, false, false);
				context.Response.ContentLength64 = resText.Length;
				w.Write(resText);
			}
		}
	}
}
