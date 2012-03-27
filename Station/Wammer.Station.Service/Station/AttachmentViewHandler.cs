using System;
using System.IO;
using System.Net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;

namespace Wammer.Station
{
	public class AttachmentViewHandler: HttpHandler
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("AttachView");
		private string AdditionalParam;

		public AttachmentViewHandler(string stationId)
			:base()
		{
			this.AdditionalParam = "station_id=" + stationId;
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		protected override void HandleRequest()
		{
			ImageMeta imageMeta = ImageMeta.None;

			try
			{
				string objectId = Parameters["object_id"];
				if (objectId == null)
					throw new ArgumentException("missing required param: object_id");

				
				if (Parameters["image_meta"] == null)
					imageMeta = ImageMeta.Origin;
				else
					imageMeta = (ImageMeta)Enum.Parse(typeof(ImageMeta),
																	Parameters["image_meta"], true);

				// "target" parameter is used to request cover image or slide page.
				// In this version station has no such resources so station always forward this
				// request to cloud.
                if (Parameters["target"] != null)
                {
                    TunnelToCloud(AdditionalParam);                    
                    return;
                }

				string namePart = objectId;
				string metaStr = "";
				if (imageMeta != ImageMeta.Origin)
				{
					metaStr = imageMeta.ToString().ToLower();
					namePart += "_" + metaStr;
				}

                Attachment doc = null;

                if (imageMeta == ImageMeta.Origin)
                    doc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", objectId));
                else
                    doc = AttachmentCollection.Instance.FindOne(Query.And(Query.EQ("_id", objectId), Query.Exists("image_meta." + imageMeta.ToString().ToLower(), true)));

                if (doc == null)
                {
                    TunnelToCloud(AdditionalParam);
                    return;
                }

				Driver driver = DriverCollection.Instance.FindOne(Query.ElemMatch("groups", Query.EQ("group_id", doc.group_id)));
				FileStorage storage = new FileStorage(driver);
				FileStream fs = storage.LoadByNameWithNoSuffix(namePart);
				Response.StatusCode = 200;
				Response.ContentLength64 = fs.Length;
				Response.ContentType = doc.mime_type;

				if (doc.type == AttachmentType.image && imageMeta != ImageMeta.Origin)
					Response.ContentType = doc.image_meta.GetThumbnailInfo(imageMeta).mime_type;

				Wammer.Utility.StreamHelper.BeginCopy(fs, Response.OutputStream, CopyComplete,
					new CopyState(fs, Response, objectId));
				
			}
			catch (ArgumentException e)
			{
				logger.Warn("Bad request: " + e.Message);
				HttpHelper.RespondFailure(Response, e, (int)HttpStatusCode.BadRequest);
			}
		}


        protected void TunnelToCloud(string additionalParam)
        {
            if (additionalParam == null || additionalParam.Length == 0)
                throw new ArgumentException("param cannot be null or empty. If you really need it blank, change the code.");

            logger.Debug("Forward to cloud");

            Uri baseUri = new Uri(Cloud.CloudServer.BaseUrl);

            string queryString = Request.Url.Query;
            Boolean IsGetRequest = Request.HttpMethod.Equals("GET", StringComparison.CurrentCultureIgnoreCase);

            if (IsGetRequest)
                if (queryString == null || queryString.Length == 0)
					queryString = additionalParam + "&return_meta=true";
                else
					queryString += "&" + additionalParam + "&return_meta=true";

            UriBuilder uri = new UriBuilder(baseUri.Scheme, baseUri.Host, baseUri.Port,
                Request.Url.AbsolutePath, queryString);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri.Uri);
            req.Method = Request.HttpMethod;
            req.ContentType = Request.ContentType;
            req.AllowAutoRedirect = false;

            if (!IsGetRequest)
            {
                using (Stream reqStream = req.GetRequestStream())
                {
                    Wammer.Utility.StreamHelper.Copy(
                        new MemoryStream(this.RawPostData),
                        reqStream);

                    StreamWriter w = new StreamWriter(reqStream);
                    w.Write("&" + additionalParam);
					w.Write("&return_meta=true");
                    w.Flush();
                }
            }

            Action<HttpWebResponse> errorResponseProcess = (HttpWebResponse response) => 
            {
                Response.StatusCode = (int)response.StatusCode;
                Response.ContentType = response.GetResponseHeader("content-type");
                Response.OutputStream.Write(response.GetResponseStream(), 1024);
                Response.Close();
            };

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();                
            }
            catch (WebException e)
            {
                errorResponseProcess((HttpWebResponse)e.Response);
                return;
            }

            var responseStream = resp.GetResponseStream();
            
            string responseMsg;
            using (var sr = new StreamReader(responseStream))
            {
                responseMsg = sr.ReadToEnd();                
            }

            Response.StatusCode = (int)resp.StatusCode;

			string redirectURL = resp.GetResponseHeader("location");
			//var attachmentView = new Wammer.Station.JSONClass.AttachmentView(responseMsg);
			var attachmentView = fastJSON.JSON.Instance.ToObject<Wammer.Station.JSONClass.AttachmentView>(responseMsg);
			Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", attachmentView.creator_id));

            if (driver == null)
            {
                errorResponseProcess(resp);
                return;
            }


            resp.Close();

            ImageMeta imageMeta = ImageMeta.None;
            if (Parameters["image_meta"] == null)
                imageMeta = ImageMeta.Origin;
            else
                imageMeta = (ImageMeta)Enum.Parse(typeof(ImageMeta), Parameters["image_meta"], true);

			var fileName = GetSavedFile(Parameters["object_id"], redirectURL, imageMeta);
            var file = Path.Combine(driver.folder, fileName);

            if (!Directory.Exists(driver.folder))
                Directory.CreateDirectory(driver.folder);


            string tempFile = System.IO.Path.GetTempFileName();

			WebClient wc = null;
			try
			{
				PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false).Increment();
				wc = new WebClient();
				wc.DownloadFile(redirectURL, tempFile);				
			}
			finally
			{
				var counter = PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, false);

				if (counter.Sample.RawValue > 0)
					counter.Decrement();
			}			

            System.IO.File.Move(tempFile, file);

            using (var fs = File.Open(file, FileMode.Open))
            {
				PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, false).IncrementBy(fs.Length);

                if (imageMeta == ImageMeta.Origin)
                {
                    AttachmentCollection.Instance.Update(Query.EQ("_id", Parameters["object_id"]), Update
						.Set("file_name", attachmentView.file_name)
                        .Set("mime_type", wc.ResponseHeaders["content-type"])
                        .Set("url", "/v2/attachments/view/?object_id=" + Parameters["object_id"])
                        .Set("file_size", fs.Length)
                        .Set("modify_time", DateTime.UtcNow)
						.Set("image_meta.width", attachmentView.image_meta.width)
						.Set("image_meta.height", attachmentView.image_meta.height)
						.Set("md5", attachmentView.md5)
						.Set("type", attachmentView.type)
						.Set("group_id", attachmentView.group_id)
                        .Set("saved_file_name", fileName), UpdateFlags.Upsert);
                }
                else
                {
                    AttachmentCollection.Instance.Update(Query.EQ("_id", Parameters["object_id"]), Update.Set("image_meta." + imageMeta.ToString().ToLower(), new ThumbnailInfo()
                    {
                        mime_type = wc.ResponseHeaders["content-type"],
                        modify_time = DateTime.UtcNow,
                        url = "/v2/attachments/view/?object_id=" + Parameters["object_id"] + "&image_meta=" + imageMeta.ToString().ToLower(),
                        file_size = fs.Length,
						file_name = attachmentView.file_name,
						width = attachmentView.image_meta.GetThumbnail(imageMeta).width,
						height = attachmentView.image_meta.GetThumbnail(imageMeta).height,
                        saved_file_name = fileName
                    }.ToBsonDocument()), UpdateFlags.Upsert);
                }
                    
                fs.Seek(0, SeekOrigin.Begin);
                Response.ContentType = wc.ResponseHeaders["content-type"];
                Response.OutputStream.Write(fs, 1024);
                Response.OutputStream.Close();
            }
        }



        private static string GetSavedFile(string objectID, string uri, ImageMeta meta)
        {
            string fileName = objectID;

            if (meta != ImageMeta.Origin && meta != ImageMeta.None)
            {
                fileName += "_" + meta.ToString().ToLower();
            }

            if (uri.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                uri = new Uri(uri).AbsolutePath;
            
            string extension = Path.GetExtension(uri);
            if (!string.IsNullOrEmpty(extension))
                fileName += extension;

            return fileName;
        }



		private static void CopyComplete(IAsyncResult ar)
		{
			CopyState state = (CopyState)ar.AsyncState;

			try
			{
				Wammer.Utility.StreamHelper.EndCopy(ar);	
			}
			catch (Exception e)
			{
				logger.Warn("Error responding attachment/view : " + state.attachmentId, e);
				HttpHelper.RespondFailure(state.response, new CloudResponse(400, -1, e.Message));
			}
			finally
			{
				try{
					state.source.Close();
					state.response.Close();
				}
				catch(Exception e)
				{
					logger.Warn("error closing source and response", e);
				}
			}
		}

		public override void OnTaskEnqueue(EventArgs e)
		{
		}
	}

	class CopyState
	{
		public Stream source { get; private set; }
		public HttpListenerResponse response { get; private set; }
		public string attachmentId { get; private set; }

		public CopyState(Stream src, HttpListenerResponse res, string attachmentId)
		{
			source = src;
			response = res;
			this.attachmentId = attachmentId;
		}
	}
}
