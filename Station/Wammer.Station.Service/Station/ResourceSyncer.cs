using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Drawing;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Model;
using Wammer.Cloud;
using Wammer.Utility;

namespace Wammer.Station
{
	public class ResourceDownloadEventArgs
	{
		public Driver driver { get; set; }
		public AttachmentInfo attachment { get; set; }
		public ImageMeta imagemeta { get; set; }
		public string filepath { get; set; }
	}

	public class ResourceSyncer : NonReentrantTimer
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ResourceSyncer));
		private bool isFirstRun = true;

		public ResourceSyncer(long timerPeriod)
			:base(timerPeriod)
		{
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			if (isFirstRun)
			{
				ResumeUnfinishedDownstreamTasks();
				isFirstRun = false;
			}

			PullTimeline(state);
		}

		private void ResumeUnfinishedDownstreamTasks()
		{
			MongoCursor<PostInfo> posts = PostCollection.Instance.Find(
				Query.Exists("attachments", true));

			foreach (PostInfo post in posts)
			{
				foreach (AttachmentInfo attachment in post.attachments)
				{
					Attachment savedDoc = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachment.object_id));
					Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", attachment.creator_id));

					// driver might be removed before download tasks completed
					if (driver == null)
						break;

					// origin
					if (driver.isPrimaryStation && 
						attachment.url != null && 
						(savedDoc == null || savedDoc.saved_file_name == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Origin);
					}

					if (attachment.image_meta == null)
						break;

					// small
					if (attachment.image_meta.small != null && 
						(savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.small == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Small);
					}

					// medium
					if (attachment.image_meta.medium != null &&
						(savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.medium == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Medium);
					}

					// large
					if (attachment.image_meta.large != null &&
						(savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.large == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Large);
					}

					// square
					if (attachment.image_meta.square != null &&
						(savedDoc == null || savedDoc.image_meta == null || savedDoc.image_meta.square == null))
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Square);
					}

				}
			}
		}

		private static void EnqueueDownstreamTask(AttachmentInfo attachment, Driver driver, ImageMeta meta)
		{
			ResourceDownloadEventArgs evtargs = new ResourceDownloadEventArgs
			{
				driver = driver,
				attachment = attachment,
				imagemeta = meta,
				filepath = Path.GetTempFileName()
			};
			TaskQueue.EnqueueLow(DownstreamResource, evtargs);
		}


		private void PullTimeline(Object obj)
		{

			using (WebClient agent = new WebClient())
			{
				foreach (Driver driver in DriverCollection.Instance.FindAll())
				{
					PostApi api = new PostApi(driver);
					if (driver.sync_range == null)
					{
						PostGetLatestResponse res = api.PostGetLatest(agent, 20);
						SavePosts(res.posts);
						DownloadMissedResource(res.posts);
						driver.sync_range = new SyncRange
						{
							start_time = res.posts.Last().timestamp,
							end_time = res.posts.First().timestamp
						};
						if (res.total_count == res.get_count)
						{
							driver.sync_range.first_post_time = driver.sync_range.start_time;
						}
					}
					else
					{
						PostFetchByFilterResponse newerRes = api.PostFetchByFilter(agent,
							new FilterEntity
							{
								limit = 20,
								timestamp = driver.sync_range.end_time
							}
						);

						if (newerRes.posts.Count > 0 &&
							newerRes.posts.First().timestamp != driver.sync_range.end_time)
						{
							SavePosts(newerRes.posts);
							DownloadMissedResource(newerRes.posts);
							driver.sync_range.end_time = newerRes.posts.First().timestamp;
						}

						if (driver.sync_range.start_time != driver.sync_range.first_post_time)
						{
							PostFetchByFilterResponse olderRes = api.PostFetchByFilter(agent,
								new FilterEntity
								{
									limit = -20,
									timestamp = driver.sync_range.start_time
								}
							);
							SavePosts(olderRes.posts);
							DownloadMissedResource(olderRes.posts);
							driver.sync_range.start_time = olderRes.posts.Last().timestamp;

							if (olderRes.remaining_count == 0)
							{
								driver.sync_range.first_post_time = driver.sync_range.start_time;
							}
						}
					}

					DriverCollection.Instance.Update(Query.EQ("_id", driver.user_id),
						Update.Set("sync_range.first_post_time", driver.sync_range.first_post_time));
					DriverCollection.Instance.Update(Query.EQ("_id", driver.user_id),
						Update.Set("sync_range.end_time", driver.sync_range.end_time));
					DriverCollection.Instance.Update(Query.EQ("_id", driver.user_id),
						Update.Set("sync_range.start_time", driver.sync_range.start_time));
				}
			}
		}


		private void SavePosts(List<PostInfo> posts)
		{
			if (posts == null)
				return;

			foreach (PostInfo post in posts)
				PostCollection.Instance.Save(post);
		}

		public static void DownloadMissedResource(List<PostInfo> posts)
		{
			foreach (PostInfo post in posts)
			{
				foreach (AttachmentInfo attachment in post.attachments)
				{
					Driver driver = DriverCollection.Instance.FindOne(Query.EQ("_id", attachment.creator_id));

					// original
					if (!string.IsNullOrEmpty(attachment.url) && driver.isPrimaryStation)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Origin);
					}

					if (attachment.image_meta == null)
						break;

					// small
					if (attachment.image_meta.small != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Small);
					}

					// medium
					if (attachment.image_meta.medium != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Medium);
					}

					// large
					if (attachment.image_meta.large != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Large);
					}

					// square
					if (attachment.image_meta.square != null)
					{
						EnqueueDownstreamTask(attachment, driver, ImageMeta.Square);
					}
				}
			}
		}

		private static void DownstreamResource(object state)
		{
			ResourceDownloadEventArgs evtargs = (ResourceDownloadEventArgs)state;
			try
			{
				bool alreadyExist = AttachmentExists(evtargs);

				if (alreadyExist)
				{
					logger.InfoFormat("Attachment {0} meta {1} already exists. Skip downstreaming it.", evtargs.attachment.object_id, evtargs.imagemeta);
					return;
				}

				AttachmentApi api = new AttachmentApi(evtargs.driver.user_id);
				api.AttachmentView(new WebClient(), evtargs);
				DownloadComplete(evtargs);
			}
			catch (WammerCloudException ex)
			{
				logger.WarnFormat("Unable to download attachment object_id={0}, image_meta={1}", evtargs.attachment.object_id, evtargs.imagemeta.ToString());
				logger.Warn("Detail exception:", ex);
			}
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
				ArraySegment<byte> rawdata = new ArraySegment<byte>(File.ReadAllBytes(filepath));
				FileStorage fs = new FileStorage(driver);

				switch (imagemeta)
				{
					case ImageMeta.Origin:
						savedFileName = attachment.object_id + Path.GetExtension(attachment.file_name);
						fs.SaveFile(savedFileName, rawdata);
						int width = 0;
						int height = 0;
						using (Image img = Image.FromStream(fs.Load(savedFileName)))
						{
							width = img.Width;
							height = img.Height;
						}
						File.Delete(filepath);

						MD5 md5 = MD5.Create();
						byte[] hash = md5.ComputeHash(rawdata.Array);
						StringBuilder md5buff = new StringBuilder();
						for (int i = 0; i < hash.Length; i++)
							md5buff.Append(hash[i].ToString("x2"));

						AttachmentCollection.Instance.Update(
							Query.EQ("_id", attachment.object_id),
							Update.Set("group_id", attachment.group_id
								).Set("file_name", attachment.file_name
								).Set("title", attachment.title
								).Set("description", attachment.description
								).Set("type", (AttachmentType)Enum.Parse(typeof(AttachmentType), attachment.type)
								).Set("url", "/v2/attachments/view?object_id=" + attachment.object_id
								).Set("mime_type", "application/octet-stream"
								).Set("saved_file_name", savedFileName
								).Set("md5", md5buff.ToString()
								).Set("image_meta.width", width
								).Set("image_meta.height", height
								).Set("modify_time", TimeHelper.ConvertToDateTime(attachment.modify_time)),
							UpdateFlags.Upsert
						);
						break;

					case ImageMeta.Small:
						savedFileName = attachment.object_id + "_small" + Path.GetExtension(attachment.file_name);
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
							Query.EQ("_id", attachment.object_id),
							Update.Set("image_meta.small", thumbnail.ToBsonDocument()),
							UpdateFlags.Upsert
						);
						break;

					case ImageMeta.Medium:
						savedFileName = attachment.object_id + "_medium" + Path.GetExtension(attachment.file_name);
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
							Query.EQ("_id", attachment.object_id),
							Update.Set("image_meta.medium", thumbnail.ToBsonDocument()),
							UpdateFlags.Upsert
						);
						break;

					case ImageMeta.Large:
						savedFileName = attachment.object_id + "_large" + Path.GetExtension(attachment.file_name);
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
							Query.EQ("_id", attachment.object_id),
							Update.Set("image_meta.large", thumbnail.ToBsonDocument()),
							UpdateFlags.Upsert
						);
						break;

					case ImageMeta.Square:
						savedFileName = attachment.object_id + "_square" + Path.GetExtension(attachment.file_name);
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
							Query.EQ("_id", attachment.object_id),
							Update.Set("image_meta.square", thumbnail.ToBsonDocument()),
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
	}
}
