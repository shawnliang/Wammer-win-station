using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;

using Wammer.Cloud;

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

			try
			{
				NameValueCollection parameters = GetRequestParams(ctx.Request);

				string objectId = parameters["object_id"];
				if (objectId == null)
					throw new ArgumentException("missing required param: object_id");

				string fileType = parameters["file_type"];
				if (fileType == null)
					throw new ArgumentException("missing required param: file_type");

				FileType type = (FileType)Enum.Parse(typeof(FileType), fileType);

				string filename = FileStorage.GetSavedFile(baseDir, objectId, type);

				ctx.Response.StatusCode = 200;
				ctx.Response.ContentType = "image/jpeg";

				using (Stream toStream = ctx.Response.OutputStream)
				{
					CopyFileToStream(filename, toStream);
				}
			}
			catch (ArgumentException e)
			{
				HttpHelper.RespondCloudFailure(ctx.Response, e, 400);
			}
			catch (FileNotFoundException e)
			{
				HttpHelper.RespondCloudFailure(ctx.Response, e, 404);
			}
			catch (Exception e)
			{
				HttpHelper.RespondCloudFailure(ctx.Response, e, 500);
			}
		}

		private static NameValueCollection GetRequestParams(HttpListenerRequest req)
		{
			if (req.HttpMethod.ToUpper().Equals("POST"))
			{
				using (StreamReader s = new StreamReader(req.InputStream))
				{
					string postData = s.ReadToEnd();
					return HttpUtility.ParseQueryString(postData);
				}
			}
			else if (req.HttpMethod.ToUpper().Equals("GET"))
			{
				return req.QueryString;
			}
			else
				throw new NotSupportedException(
									"Method is not support: " + req.HttpMethod);
		}

		private static void CopyFileToStream(string filename, Stream dest)
		{
			byte[] buffer = new byte[32768];
			int nRead = 0;
			using (FileStream fs = File.OpenRead(filename))
			{
				while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
				{
					dest.Write(buffer, 0, nRead);
				}
			}
		}


	}
}
