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
	public class ViewObjectHandler: HttpHandler
	{
		private string baseDir;
		public ViewObjectHandler(string resourceFolder)
		{
			baseDir = resourceFolder;
		}

		protected override void HandleRequest()
		{
			try
			{
				string objectId = Parameters["object_id"];
				if (objectId == null)
					throw new ArgumentException("missing required param: object_id");

				string fileType = Parameters["file_type"];
				if (fileType == null)
					throw new ArgumentException("missing required param: file_type");

				FileType type = (FileType)Enum.Parse(typeof(FileType), fileType);

				string filename = FileStorage.GetSavedFile(baseDir, objectId, type);

				Response.StatusCode = 200;
				Response.ContentType = "image/jpeg";

				using (Stream toStream = Response.OutputStream)
				using (FileStream fs = File.OpenRead(filename))
				{
					Wammer.IO.StreamHelper.Copy(fs, toStream);
				}
			}
			catch (ArgumentException e)
			{
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.BadRequest);
			}
			catch (FileNotFoundException e)
			{
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.NotFound);
			}
		}
	}
}
