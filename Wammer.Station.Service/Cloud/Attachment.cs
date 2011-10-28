using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using Wammer.MultiPart;

namespace Wammer.Cloud
{
	public enum ImageMeta
	{
		None = 0,
		/// <summary>
		/// 128x128 pixels
		/// </summary>
		Square = 128,
		/// <summary>
		/// 120 pixels
		/// </summary>
		Small = 120,
		/// <summary>
		/// 720 pixels
		/// </summary>
		Medium = 720,
		/// <summary>
		/// 1024 pixels
		/// </summary>
		Large = 1024,
		/// <summary>
		/// Original image size
		/// </summary>
		Origin = 50 * 1024 * 1024
	}

	public enum AttachmentType
	{
		image,
		doc
	}

	public static class Attachment
	{
		public static ObjectUploadResponse UploadImage(string url, byte[] imageData, string objectId,
												string fileName, string contentType, ImageMeta meta)
		{

			Dictionary<string, object> pars = new Dictionary<string, object>();
			pars["type"] = "image";
			pars["image_meta"] = meta.ToString().ToLower();
			pars["session_token"] = CloudServer.SessionToken;
			if (objectId != null)
				pars["object_id"] = objectId;
			pars["file"] = imageData;
			HttpWebResponse _webResponse = Waveface.MultipartFormDataPostHelper.MultipartFormDataPost(
				url,
				"Mozilla 4.0+",
				pars,
				fileName,
				contentType);

			using (StreamReader reader = new StreamReader(_webResponse.GetResponseStream()))
			{
				return fastJSON.JSON.Instance.ToObject<ObjectUploadResponse>(reader.ReadToEnd());
			}			
		}

		public static ObjectUploadResponse UploadImage(string url, byte[] imageData,
												string fileName, string contentType, ImageMeta meta)
		{
			return UploadImage(url, imageData, null, fileName, contentType, meta);
		}

		public static ObjectUploadResponse UploadImage(byte[] imageData, string objectId,
												string fileName, string contentType, ImageMeta meta)
		{
			string url = string.Format("http://{0}:{1}/{2}/attachments/upload/",
				Cloud.CloudServer.HostName,
				Cloud.CloudServer.Port,
				CloudServer.DEF_BASE_PATH);

			return UploadImage(url, imageData, objectId, fileName, contentType, meta);
		}
	}
}