using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using Wammer.Cloud;
using System.Net;
using Wammer.Utility;
using Wammer.Model;
using System.IO;
using System.Security.Cryptography;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Drawing;

namespace Wammer.Station.Timeline
{
	public class ResourceDownloadTask : INamedTask
	{
		private static ILog logger = LogManager.GetLogger(typeof(ResourceDownloadTask));

		private ResourceDownloadEventArgs evtargs;
		private readonly ITaskEnqueuable<ResourceDownloadTask> bodySyncQueue;
		private TaskPriority priority;

		public ResourceDownloadTask(ResourceDownloadEventArgs arg, ITaskEnqueuable<ResourceDownloadTask> bodySyncQueue, TaskPriority pri)
		{
			this.evtargs = arg;
			this.bodySyncQueue = bodySyncQueue;
			this.priority = pri;
		}

		private static bool AttachmentExists(ResourceDownloadEventArgs evtargs)
		{
			bool alreadyExist;

			if (evtargs.imagemeta == ImageMeta.Origin || evtargs.imagemeta == ImageMeta.None)
				// origin image and non-image attachment here
				alreadyExist = AttachmentCollection.Instance.FindOne(
					Query.And(
						Query.EQ("_id", evtargs.attachment.object_id),
						Query.Exists("saved_file_name", true))) != null;
			else
				// thumbnails here
				alreadyExist = AttachmentCollection.Instance.FindOne(
					Query.And(
						Query.EQ("_id", evtargs.attachment.object_id),
						Query.Exists("image_meta." + evtargs.imagemeta.ToString().ToLower() + ".saved_file_name", true))) != null;

			return alreadyExist;
		}

		private static void DownloadComplete(ResourceDownloadEventArgs args)
		{
			try
			{
				Driver driver = args.driver;
				AttachmentInfo attachment = args.attachment;
				ImageMeta imagemeta = args.imagemeta;
				string filepath = args.filepath;

				ThumbnailInfo thumbnail;
				string savedFileName;
				var rawdata = new ArraySegment<byte>(File.ReadAllBytes(filepath));

				var fs = new FileStorage(driver);

				switch (imagemeta)
				{
					case ImageMeta.Origin:
						savedFileName = attachment.object_id + Path.GetExtension(attachment.file_name);
						fs.SaveFile(savedFileName, rawdata);
						int width = 0;
						int height = 0;
						using (var img = Image.FromStream(fs.Load(savedFileName)))
						{
							width = img.Width;
							height = img.Height;
						}
						File.Delete(filepath);

						MD5 md5 = MD5.Create();
						byte[] hash = md5.ComputeHash(rawdata.Array);
						var md5buff = new StringBuilder();
						for (var i = 0; i < hash.Length; i++)
							md5buff.Append(hash[i].ToString("x2"));

						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id),
							Update.Set("group_id", attachment.group_id
								).Set("file_name", attachment.file_name
								).Set("title", attachment.title
								).Set("description", attachment.description
								).Set("type", (int)(AttachmentType)Enum.Parse(typeof(AttachmentType), attachment.type)
								).Set("url", "/v2/attachments/view?object_id=" + attachment.object_id
								).Set("mime_type", "application/octet-stream"
								).Set("saved_file_name", savedFileName
								).Set("md5", md5buff.ToString()
								).Set("image_meta.width", width
								).Set("image_meta.height", height
								).Set("file_size", rawdata.Count
								).Set("modify_time", TimeHelper.ConvertToDateTime(attachment.modify_time)
								).Set("is_body_upstreamed", true
								).Set("is_thumb_upstreamed", true),
							UpdateFlags.Upsert
							);

						TaskQueue.Enqueue(new NotifyCloudOfBodySyncedTask(attachment.object_id, driver.session_token), TaskPriority.Low,
								  true);

						break;

					case ImageMeta.Small:
						savedFileName = attachment.object_id + "_small.dat";
						fs.SaveFile(savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=small",
							width = attachment.image_meta.small.width,
							height = attachment.image_meta.small.height,
							mime_type = attachment.image_meta.small.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.small.modify_time),
							file_size = attachment.image_meta.small.file_size,
							file_name = attachment.image_meta.small.file_name,
							md5 = attachment.image_meta.small.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.small", thumbnail.ToBsonDocument())
																	.Set("is_thumb_upstreamed", true),
							UpdateFlags.Upsert
							);
						break;

					case ImageMeta.Medium:
						savedFileName = attachment.object_id + "_medium.dat";
						fs.SaveFile(savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=medium",
							width = attachment.image_meta.medium.width,
							height = attachment.image_meta.medium.height,
							mime_type = attachment.image_meta.medium.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.medium.modify_time),
							file_size = attachment.image_meta.medium.file_size,
							file_name = attachment.image_meta.medium.file_name,
							md5 = attachment.image_meta.medium.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.medium", thumbnail.ToBsonDocument())
																	.Set("is_thumb_upstreamed", true),
							UpdateFlags.Upsert
							);
						break;

					case ImageMeta.Large:
						savedFileName = attachment.object_id + "_large.dat";
						fs.SaveFile(savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=large",
							width = attachment.image_meta.large.width,
							height = attachment.image_meta.large.height,
							mime_type = attachment.image_meta.large.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.large.modify_time),
							file_size = attachment.image_meta.large.file_size,
							file_name = attachment.image_meta.large.file_name,
							md5 = attachment.image_meta.large.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.large", thumbnail.ToBsonDocument())
																	.Set("is_thumb_upstreamed", true),
							UpdateFlags.Upsert
							);
						break;

					case ImageMeta.Square:
						savedFileName = attachment.object_id + "_square.dat";
						fs.SaveFile(savedFileName, rawdata);
						File.Delete(filepath);

						thumbnail = new ThumbnailInfo
						{
							url = "/v2/attachments/view?object_id=" + attachment.object_id + "&image_meta=square",
							width = attachment.image_meta.square.width,
							height = attachment.image_meta.square.height,
							mime_type = attachment.image_meta.square.mime_type,
							modify_time = TimeHelper.ConvertToDateTime(attachment.image_meta.square.modify_time),
							file_size = attachment.image_meta.square.file_size,
							file_name = attachment.image_meta.square.file_name,
							md5 = attachment.image_meta.square.md5,
							saved_file_name = savedFileName
						};
						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id), Update
																	.Set("group_id", attachment.group_id)
																	.Set("file_name", attachment.file_name)
																	.Set("type",
																		 (int)
																		 (AttachmentType)
																		 Enum.Parse(typeof(AttachmentType), attachment.type))
																	.Set("image_meta.square", thumbnail.ToBsonDocument())
																	.Set("is_thumb_upstreamed", true),
							UpdateFlags.Upsert
							);
						break;

					default:
						logger.WarnFormat("Unknown image meta type {0}", imagemeta);
						break;
				}

			}
			catch (Exception ex)
			{
				logger.Warn("Unable to save attachment, ignore it.", ex);
			}
		}

		public void Execute()
		{
			if (evtargs == null || bodySyncQueue == null)
				return; // evtargs and bodySyncQueue can be null for stopping TaskRunner.

			string meta = evtargs.imagemeta.ToString();
			string oldFile = evtargs.filepath;

			try
			{
				bool alreadyExist = AttachmentExists(evtargs);

				if (alreadyExist)
				{
					logger.DebugFormat("Attachment {0} meta {1} already exists. Skip downstreaming it.", evtargs.attachment.object_id,
									   meta);
					return;
				}

				var api = new AttachmentApi(evtargs.driver);
				using (WebClient client = new DefaultWebClient())
				{
					api.AttachmentView(client, evtargs, StationRegistry.StationId);
				}
				DownloadComplete(evtargs);
			}
			catch (WammerCloudException ex)
			{
				++evtargs.failureCount;

				if (evtargs.failureCount >= 3)
				{
					logger.WarnFormat("Unable to download attachment object_id={0}, image_meta={1}", evtargs.attachment.object_id, meta);
					logger.Warn("Detail exception:", ex);
				}
				else
				{
					logger.WarnFormat("Unable to download attachment. Enqueue download task again: attachment object_id={0}, image_meta={1}",
									   evtargs.attachment.object_id, meta);
					logger.Warn("Detail exception:", ex);
					evtargs.filepath = FileStorage.GetTempFile(evtargs.driver);
					bodySyncQueue.Enqueue(this, this.priority);
				}
			}
			finally
			{
				File.Delete(oldFile);
			}
		}

		public string Name
		{
			get
			{
				return evtargs.attachment.object_id + evtargs.imagemeta;
			}
		}

		public string UserId
		{
			get
			{
				return evtargs.driver.user_id;
			}
		}
	}
}
