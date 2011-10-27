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
		private readonly FileStorage fileStorage;

		public ViewObjectHandler(FileStorage fileStorage)
			:base()
		{
			this.fileStorage = fileStorage;
		}

		public override object Clone()
		{
			return new ViewObjectHandler(fileStorage);
		}

		protected override void HandleRequest()
		{
			try
			{
				string objectId = Parameters["object_id"];
				if (objectId == null)
					throw new ArgumentException("missing required param: object_id");

				ImageMeta imageMeta;

				if (Parameters["image_meta"] == null)
					imageMeta = ImageMeta.Original;
				else
					imageMeta = (ImageMeta)Enum.Parse(typeof(ImageMeta),
																	Parameters["image_meta"], true);

				//FIXME : should return image based on imageMeta
				using (FileStream fs = fileStorage.LoadById(objectId))
				{
					Response.StatusCode = 200;
					Response.ContentType = "image/jpeg"; //FIXME: query db to get correct content type

					Wammer.Utility.StreamHelper.Copy(fs, Response.OutputStream);
					Response.OutputStream.Close();
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
