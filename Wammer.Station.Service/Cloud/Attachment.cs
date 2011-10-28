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
			Part filePart = new Part(imageData, 0, imageData.Length);
			filePart.Headers.Add("Content-Disposition",
											"form-data; name=\"file\"; filename=\"" + fileName + "\"");

			filePart.Headers.Add("Content-Type", contentType);

			Part metaPart = new Part(meta.ToString().ToLower());
			metaPart.Headers.Add("Content-Disposition", "form-data; name=\"image_meta\"");

			Part typePart = new Part("image");
			typePart.Headers.Add("Content-Disposition", "form-data; name=\"type\"");

			Part objIdPart = null;
			if (objectId != null)
			{
				objIdPart = new Part(objectId);
				objIdPart.Headers.Add("Content-Disposition", "form-data; name=object_id");
			}

			string boundary = Guid.NewGuid().ToString();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Timeout = 10000;
			request.Method = "POST";
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			request.CookieContainer = CloudServer.Cookies;

			using (Stream m = new MemoryStream())
			{
				Serializer writer = new Serializer(m, boundary);
				writer.Put(filePart);
				writer.Put(metaPart);
				writer.Put(typePart);

				if (objIdPart != null)
					writer.Put(objIdPart);

				writer.PutNoMoreData();

				request.ContentLength = m.Length;

				using (Stream s = request.GetRequestStream())
				{
					m.Position = 0;
					Utility.StreamHelper.Copy(m, s);
				}
			}
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				string resp = reader.ReadToEnd();
				return fastJSON.JSON.Instance.ToObject<ObjectUploadResponse>(resp);
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