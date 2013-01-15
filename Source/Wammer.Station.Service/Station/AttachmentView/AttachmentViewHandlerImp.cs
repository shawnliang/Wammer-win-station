using System;
using System.Collections.Specialized;
using System.IO;
using Wammer.Model;
using Waveface.Stream.Model;

namespace Wammer.Station.AttachmentView
{

	public interface IAttachmentViewHandlerDB
	{
		Attachment GetAttachment(string object_id);
		Driver GetUserByGroupId(string group_id);
		void UpdateLastAccessTime(string object_id);
	}

	public interface IAttachmentViewStorage
	{
		Stream GetAttachmentStream(ImageMeta meta, Driver user, string fileName);
	}

	public class ViewResult
	{
		public Stream Stream { get; set; }
		public string MimeType { get; set; }
	}

	public class AttachmentViewHandlerImp
	{
		public IAttachmentViewHandlerDB DB { get; set; }
		public IAttachmentViewStorage Storage { get; set; }

		public AttachmentViewHandlerImp()
		{
			DB = new AttachmentViewHandlerDB();
			Storage = new AttachmentViewStorage();
		}

		public ViewResult GetAttachmentStream(NameValueCollection Parameters)
		{
			if ("preview".Equals(Parameters["target"]))
				return getDocPreview(Parameters);
			else if ("static_map".Equals(Parameters["target"]))
				return getStaticMap(Parameters);
			else
				return getImageStream(Parameters);
		}

		private ViewResult getStaticMap(NameValueCollection Parameters) //暫時先借用attachment/view
		{
			var object_id = Parameters["object_id"];
			if (string.IsNullOrEmpty(object_id))
				throw new FormatException("missing parameter: object_id");

			var location = LocationDBDataCollection.Instance.FindOneById(object_id);
			if(location == null)
				throw new WammerStationException("object_id not found", -1);

			var userID = location.CreatorID;


			var mapFile = FileStorage.GetStaticMap(userID, object_id);

			if (!File.Exists(mapFile))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(mapFile));
				DownloadMapPhoto(location.Latitude.Value, location.Longitude.Value, location.ZoomLevel.Value, mapFile);
			}

			return new ViewResult
			{
				MimeType = "image/jpeg",
				Stream = File.OpenRead(mapFile)
			};
		}

		private void DownloadMapPhoto(double latitude, double longitude, int zoomeLevel, string file)
		{
			if (string.IsNullOrEmpty(file))
				throw new ArgumentNullException("file");

			if (File.Exists(file))
				return;

			try
			{
				var urlFormat = @"http://maps.google.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=640x640&scale=2&format=jpg&sensor=false&markers=color:red%7Csize:mid%7Clabel:A%7C{0},{1}";
				var url = String.Format(urlFormat, latitude.ToString(), longitude.ToString(), zoomeLevel.ToString());

				using (var wc = new WebClientEx())
				{
					wc.DownloadFile(url, file);
				}
			}
			catch (Exception)
			{
			}
		} 

		private ViewResult getDocPreview(NameValueCollection Parameters)
		{
			var object_id = Parameters["object_id"];
			if (string.IsNullOrEmpty(object_id))
				throw new FormatException("missing parameter: object_id");

			var attDoc = DB.GetAttachment(object_id);
			if (attDoc == null)
				throw new WammerStationException("object_id not found", -1);

			if (attDoc.type != AttachmentType.doc)
				throw new WammerStationException("This type of attachment has no review: " + attDoc.type.ToString(), -1);

			var pageNum = Parameters["page"];
			if (string.IsNullOrEmpty(pageNum))
				throw new FormatException("missing parameter: page");

			if (attDoc.doc_meta == null || attDoc.doc_meta.preview_files == null)
				throw new WammerStationException("previews are not yet ready", -1);

			var page = int.Parse(pageNum);
			var pageIndex = page - 1;
			if (pageIndex < 0 || attDoc.doc_meta.preview_files.Count <= pageIndex)
				throw new WammerStationException("page is out of range", -1);

			var filename = attDoc.doc_meta.preview_files[pageIndex];
			return new ViewResult
			{
				MimeType = "image/jpeg",
				Stream = File.OpenRead(filename)
			};
		}

		private ViewResult getImageStream(NameValueCollection Parameters)
		{
			var object_id = Parameters["object_id"];

			if (string.IsNullOrEmpty(object_id))
				throw new FormatException("missing parameter: object_id");

			var metaStr = Parameters["image_meta"];

			var meta = string.IsNullOrEmpty(metaStr) ? ImageMeta.Origin : (ImageMeta)Enum.Parse(typeof(ImageMeta), metaStr, true);
			var dbDoc = DB.GetAttachment(object_id);

			if (dbDoc == null)
				throw new FileNotFoundException("attachment db record not found: " + object_id);

			if (dbDoc.type == AttachmentType.webthumb)
				meta = ImageMeta.Origin;

			var user = DB.GetUserByGroupId(dbDoc.group_id);

			if (user == null)
				throw new WammerStationException("User group " + dbDoc.group_id + " is not found", (int)StationLocalApiError.InvalidDriver);

			string filename = getSavedFileName(meta, dbDoc);

			if (meta == ImageMeta.Origin || meta == ImageMeta.None)
				DB.UpdateLastAccessTime(object_id);

			return new ViewResult
			{
				Stream = Storage.GetAttachmentStream((dbDoc.type == AttachmentType.webthumb) ? ImageMeta.Medium : meta, user, filename),
				MimeType = getMimeType(meta, dbDoc)
			};
		}

		private static string getSavedFileName(ImageMeta meta, Attachment dbDoc)
		{
			try
			{
				var fileInfo = dbDoc.GetInfoByMeta(meta);
				if (fileInfo == null || string.IsNullOrEmpty(fileInfo.saved_file_name))
					throw new FileNotFoundException();

				return fileInfo.saved_file_name;
			}
			catch
			{
				throw new FileNotFoundException("attachment " + dbDoc.object_id + " " + meta + "is not found");
			}
		}

		private static string getMimeType(ImageMeta meta, Attachment dbDoc)
		{
			var fileInfo = dbDoc.GetInfoByMeta(meta);
			return fileInfo.mime_type;
		}
	}
}
