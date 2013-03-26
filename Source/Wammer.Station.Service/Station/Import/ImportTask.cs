using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Wammer.Station.Import;
namespace Wammer.Station
{
	public class ImportTask : ITask
	{
		#region Events
		public event EventHandler<TaskStartedEventArgs> TaskStarted;
		public event EventHandler<FilesEnumeratedArgs> FilesEnumerated;
		public event EventHandler<FileImportEventArgs> FileIndexed;
		public event EventHandler<FileImportEventArgs> FileSkipped;
		public event EventHandler<FileImportEventArgs> FileImportFailed;
		public event EventHandler<FileImportEventArgs> FileImported;
		public event EventHandler<ImportDoneEventArgs> ImportDone;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets or sets the m_ paths.
		/// </summary>
		/// <value>The m_ paths.</value>
		public IEnumerable<String> Paths { get; set; }

		/// <summary>
		/// Gets or sets the m_ group ID.
		/// </summary>
		/// <value>The m_ group ID.</value>
		private String m_GroupID { get; set; }

		/// <summary>
		/// Gets or sets the m_ session token.
		/// </summary>
		/// <value>The m_ session token.</value>
		private String m_SessionToken { get; set; }

		private bool m_CopyToStation { get; set; }

		/// <summary>
		/// Gets or sets the m_ API key.
		/// </summary>
		/// <value>The m_ API key.</value>
		private String m_APIKey { get; set; }

		private SearchOption m_searchOption { get; set; }

		private int timezoneDiff;
		private List<ObjectIdAndPath> allSavedFiles = new List<ObjectIdAndPath>();
		private List<ObjectIdAndPath> allFailedFiles = new List<ObjectIdAndPath>();
		#endregion

		#region Public Property
		public Guid TaskId { get; private set; }
		#endregion

		#region Constructor
		public ImportTask(string apiKey, string sessionToken, string groupID, IEnumerable<string> paths, bool copyToStation, SearchOption searchOption = SearchOption.TopDirectoryOnly)
			: this()
		{
			m_APIKey = apiKey;
			m_SessionToken = sessionToken;
			m_GroupID = groupID;
			m_CopyToStation = copyToStation;
			m_searchOption = searchOption;
			Paths = paths.Where((file) => file.Length > 0);
		}

		private ImportTask()
		{
			timezoneDiff = (int)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;

			TaskId = Guid.NewGuid();
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes this instance.
		/// </summary>
		public void Execute()
		{
			if (string.IsNullOrEmpty(m_APIKey) || string.IsNullOrEmpty(m_SessionToken) || string.IsNullOrEmpty(m_GroupID) || !Paths.Any())
				return;

			this.LogInfoMsg("Importing from: \n" + string.Join("\n", Paths.ToArray()));
			Exception error = null;

			try
			{
				raiseTaskStartedEvent();

				var user = DriverCollection.Instance.FindDriverByGroupId(m_GroupID);

				var importTime = DateTime.Now;


				// Scan all photo
				var photoCrawler = new PhotoCrawler();
				var inputFiles = Paths.Where(file => Path.GetExtension(file).Length > 0);
				var allFiles = photoCrawler.FindPhotos(Paths, photoCrawlerError, m_searchOption).Select(file => new ObjectIdAndPath { file_path = file, object_id = Guid.NewGuid().ToString() }).ToList();
				raiseFilesEnumeratedEvent(allFiles.Count);


				// de-dup
				allFiles = dedup(allFiles);


				// index files, generate metadata
				var allMeta = indexFiles(allFiles, user);


				// build collections
				var folderCollections = FolderCollection.Build(allMeta.Where(meta => !inputFiles.Contains(meta.file_path)).Cast<ObjectIdAndPath>());
				(new CreateFolderCollectionTask(folderCollections, m_GroupID)).Execute();

				var coverObjectIDs = folderCollections.Values.Select(item => item.Objects.FirstOrDefault());

				if (m_CopyToStation)
				{
					var coverMetas = allMeta.Where(meta => coverObjectIDs.Contains(meta.object_id)).ToList();
					var nonCoverMetas = allMeta.Except(coverMetas).ToList();

					copyFilesToAostream(importTime, coverMetas);
					copyFilesToAostream(importTime, nonCoverMetas);

					handleCopyFailedFiles(allFailedFiles, user);
				}
				else
				{
					foreach (var file in allMeta)
					{
						var attDoc = new Attachment
						{
							creator_id = user.user_id,
							device_id = StationRegistry.StationId,
							event_time = file.EventTime,
							file_create_time = file.file_create_time,
							file_modify_time = File.GetLastWriteTime(file.file_path),
							file_name = file.file_name,
							file_path = file.file_path,
							file_size = new FileInfo(file.file_path).Length,
							fromLocal = true,
							group_id = m_GroupID,
							image_meta = new ImageProperty { exif = file.exif },
							mime_type = MimeTypeHelper.GetMIMEType(file.file_name),
							MD5 = MD5Helper.ComputeMD5(File.ReadAllBytes(file.file_path)),
							modify_time = DateTime.Now,
							object_id = file.object_id,
							timezone = file.timezone,
							type = AttachmentType.image,
							IndexOnly = true
						};
						AttachmentCollection.Instance.Save(attDoc);

						raiseFileImportedEvent(file);

						if (user.isPaidUser)
						{
							var backupTask = new UpstreamTask(file.object_id, ImageMeta.Origin, TaskPriority.VeryLow, TaskId);
							AttachmentUpload.AttachmentUploadQueueHelper.Instance.Enqueue(backupTask, TaskPriority.VeryLow);

							var medium = new MakeThumbnailTask(file.object_id, ImageMeta.Medium, TaskPriority.Medium, TaskId);
							TaskQueue.Enqueue(medium, medium.Priority, true);
						}
						else
						{
							var medium = new MakeThumbnailAndUpstreamTask(file.object_id, ImageMeta.Medium, TaskPriority.Medium, TaskId);
							TaskQueue.Enqueue(medium, medium.Priority, true);
						}


						var small = new MakeThumbnailTask(file.object_id, ImageMeta.Small, TaskPriority.Medium, TaskId);
						TaskQueue.Enqueue(small, small.Priority, true);

					}
				}
			}
			catch (Exception e)
			{
				this.LogWarnMsg("Import not completed", e);
				error = e;
			}
			finally
			{
				raiseImportDoneEvent(error);
			}
		}

		private List<ObjectIdAndPath> dedup(List<ObjectIdAndPath> allFiles)
		{
			var dedupResult = new HashSet<FileDedupItem>();

			foreach (var file in allFiles)
			{
				var item = new FileDedupItem(file);

				if (hasDupItemInDB(file) || dedupResult.Contains(item))
					raiseFileSkippedEvent(file);
				else
					dedupResult.Add(item);
			}

			return dedupResult.Select(x => x.file).ToList();
		}

		private void handleCopyFailedFiles(List<ObjectIdAndPath> allFailedFiles, Driver user)
		{
			if (allFailedFiles.Count > 0)
			{
				enqueueDeleteAttachmentTask(allFailedFiles, user);

				foreach (var file in allFailedFiles)
				{
					AttachmentCollection.Instance.Remove(Query.EQ("_id", file.object_id));
				}
			}
		}

		private void copyFilesToAostream(DateTime importTime, List<FileMetadata> allMeta)
		{
			int nProc = 0;
			do
			{
				var batch = allMeta.Skip(nProc).Take(50);
				var saved = submitBatch(importTime, batch, ref allFailedFiles);

				allSavedFiles.AddRange(saved);

				nProc += batch.Count();

			} while (nProc < allMeta.Count);
		}

		private List<FileMetadata> indexFiles(List<ObjectIdAndPath> allFiles, Driver user)
		{
			var metadataSubmitted = new List<ObjectIdAndPath>();

			try
			{
				var allIndexed = new HashSet<FileMetadata>();
				var nIndexed = 0;
				do
				{
					var batch = allFiles.Skip(nIndexed).Take(50);
					var metadata = extractMetadata(batch, allIndexed);

					enqueueUploadMetadataTask(metadata);
					metadataSubmitted.AddRange(batch);
					nIndexed += batch.Count();

				} while (nIndexed < allFiles.Count);


				if (allIndexed.Count == 0)
					throw new Exception("No file needs to import");

				var allMeta = allIndexed.ToList();
				allMeta.Sort((x, y) => y.EventTime.CompareTo(x.EventTime));
				return allMeta;
			}
			catch
			{
				enqueueDeleteAttachmentTask(metadataSubmitted, user);
				throw;
			}
		}

		private void enqueueDeleteAttachmentTask(List<ObjectIdAndPath> metadataSubmitted, Driver user)
		{
			var task = new AttachmentDeleteTask(metadataSubmitted.Select(x => x.object_id).ToList(), user.user_id, true);
			AttachmentUploadQueueHelper.Instance.Enqueue(task, TaskPriority.High);
		}


		private bool hasDupItemInDB(ObjectIdAndPath item)
		{
			var sameSizeFiles = AttachmentCollection.Instance.Find(
							Query.And(
								Query.EQ("group_id", m_GroupID),
								Query.EQ("file_size", new FileInfo(item.file_path).Length)));

			bool hasDup = false;
			foreach (var sameSizeFile in sameSizeFiles)
			{
				if (!string.IsNullOrEmpty(sameSizeFile.file_name) &&
					sameSizeFile.file_name.Equals(Path.GetFileName(item.file_path), StringComparison.InvariantCultureIgnoreCase))
				{
					hasDup = true;
					break;
				}
			}
			return hasDup;
		}

		private List<ObjectIdAndPath> submitBatch(DateTime importTime, IEnumerable<FileMetadata> batch, ref List<ObjectIdAndPath> failed)
		{
			List<ObjectIdAndPath> saved = new List<ObjectIdAndPath>();

			var post_id = Guid.NewGuid().ToString();
			foreach (var file in batch)
			{
				try
				{
					saveToStream(importTime, post_id, file);
					saved.Add(file);
				}
				catch (Exception e)
				{
					file.Error = e.Message;
					failed.Add(file);
					raiseFileImportFailedEvent(file);
				}
			}

			var objectIDs = saved.Select(x => x.object_id);
			createPostContainer(importTime, post_id, objectIDs);

			return saved;
		}
		#endregion

		#region Private Method

		private void photoCrawlerError(string path, Exception e)
		{
			this.LogWarnMsg("Unable to enumerate files under " + path, e);
		}

		private void createPostContainer(DateTime importTime, string postID, IEnumerable<string> objectIDs)
		{
			var parameters = new NameValueCollection()
				{
					{CloudServer.PARAM_SESSION_TOKEN, m_SessionToken},
					{CloudServer.PARAM_API_KEY, m_APIKey},
					{CloudServer.PARAM_POST_ID, postID},
					{CloudServer.PARAM_TYPE, "import"},
					{CloudServer.PARAM_TIMESTAMP, importTime.ToUTCISO8601ShortString()},
					{CloudServer.PARAM_GROUP_ID, m_GroupID},
					{CloudServer.PARAM_ATTACHMENT_ID_ARRAY, string.Format("[{0}]",string.Join(",", objectIDs.Select((x)=> "\""+x+"\"").ToArray()))},
					{CloudServer.PARAM_CONTENT, string.Format("Import {0} files", objectIDs.Count().ToString())},
					{CloudServer.PARAM_COVER_ATTACH, objectIDs.FirstOrDefault()},
				};
			PostUploadTaskController.Instance.AddPostUploadAction(postID, PostUploadActionType.NewPost, parameters, importTime, importTime);
		}

		private void saveToStream(DateTime importTime, string postID, FileMetadata file)
		{
			long begin = Stopwatch.GetTimestamp();

			var imp = new AttachmentUploadHandlerImp(
				new AttachmentUploadHandlerDB(),
				new AttachmentUploadStorage(new AttachmentUploadStorageDB()));

			var postProcess = new AttachmentProcessedHandler(new AttachmentUtility(TaskId));

			imp.AttachmentProcessed += postProcess.OnProcessed;


			var uploadData = new UploadData()
			{
				object_id = file.object_id,
				raw_data = new ArraySegment<byte>(File.ReadAllBytes(file.file_path)),
				file_name = Path.GetFileName(file.file_path),
				mime_type = MimeTypeHelper.GetMIMEType(file.file_path),
				group_id = m_GroupID,
				api_key = m_APIKey,
				session_token = m_SessionToken,
				file_path = file.file_path,
				import_time = importTime,
				file_create_time = file.file_create_time,
				type = AttachmentType.image,
				imageMeta = ImageMeta.Origin,
				fromLocal = true,
				timezone = timezoneDiff,
				exif = file.exif.ToFastJSON()
			};
			imp.Process(uploadData);

			SystemEventSubscriber.Instance.TriggerAttachmentArrivedEvent(file.object_id);

			long end = Stopwatch.GetTimestamp();
			long duration = end - begin;
			if (duration < 0)
				duration += long.MaxValue;

			Wammer.PerfMonitor.UploadDownloadMonitor.Instance.OnAttachmentProcessed(this, new HttpHandlerEventArgs(duration));
			raiseFileImportedEvent(file);
		}

		private List<FileMetadata> extractMetadata(IEnumerable<ObjectIdAndPath> files, HashSet<FileMetadata> indexedFiles)
		{
			var exifExtractor = new ExifExtractor();
			var batch = new List<FileMetadata>();

			foreach (var file in files)
			{
				var meta = new FileMetadata
				{
					object_id = file.object_id,
					type = "image",
					file_path = file.file_path,
					file_name = Path.GetFileName(file.file_path),
					file_create_time = file.CreateTime,
					timezone = timezoneDiff,
					exif = exifExtractor.extract(file.file_path)
				};

				indexedFiles.Add(meta);
				batch.Add(meta);
				raiseFileIndexedEvent(file);
			}

			return batch;
		}

		private void enqueueUploadMetadataTask(IEnumerable<FileMetadata> batch)
		{
			try
			{
				var serializeSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc };
				var batchMetadata = JsonConvert.SerializeObject(batch, Formatting.None, serializeSetting);

				var task = new UploadMetadataTask(m_GroupID, batchMetadata, batch.Count());
				AttachmentUploadQueueHelper.Instance.Enqueue(task, TaskPriority.High);
			}
			catch (Exception e)
			{
				this.LogWarnMsg("metadata upload failed", e);
			}
		}

		private void raiseFileImportedEvent(ObjectIdAndPath file)
		{
			var handler = FileImported;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}

		private void raiseImportDoneEvent(Exception e)
		{
			var handler = ImportDone;
			if (handler != null)
				handler(this,
					new ImportDoneEventArgs
					{
						Error = e,
						TaskId = TaskId,
						FailedFiles = allFailedFiles.Select(x => x.file_path).ToList()
					});
		}

		private void raiseFilesEnumeratedEvent(int count)
		{
			EventHandler<FilesEnumeratedArgs> handler = FilesEnumerated;
			if (handler != null)
				handler(this, new FilesEnumeratedArgs { TotalCount = count, TaskId = TaskId });
		}

		private void raiseFileImportFailedEvent(ObjectIdAndPath file)
		{
			EventHandler<FileImportEventArgs> handler = FileImportFailed;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}

		private void raiseFileIndexedEvent(ObjectIdAndPath file)
		{
			var handler = FileIndexed;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}

		private void raiseFileSkippedEvent(ObjectIdAndPath file)
		{
			var handler = FileSkipped;
			if (handler != null)
				handler(this, new FileImportEventArgs { file = file, TaskId = TaskId });
		}

		private void raiseTaskStartedEvent()
		{
			var handler = TaskStarted;
			if (handler != null)
				handler(this, new TaskStartedEventArgs { TaskId = TaskId });
		}
		#endregion
	}

	public class TaskStartedEventArgs : EventArgs
	{
		public Guid TaskId { get; set; }
	}

	public class FileImportEventArgs : EventArgs
	{
		public ObjectIdAndPath file { get; set; }
		public Guid TaskId { get; set; }
	}

	public class ImportDoneEventArgs : EventArgs
	{
		public Guid TaskId { get; set; }
		public Exception Error { get; set; }
		public List<string> FailedFiles { get; set; }
	}

	public class FilesEnumeratedArgs : EventArgs
	{
		public Guid TaskId { get; set; }
		public int TotalCount { get; set; }
	}


	public class FileMetadata : ObjectIdAndPath
	{
		public string type { get; set; }
		public string file_name { get; set; }
		public DateTime file_create_time { get; set; }
		public int timezone { get; set; }
		public exif exif { get; set; }

		private DateTime? _event_time;

		[JsonIgnore]
		public DateTime EventTime
		{
			get
			{
				if (!_event_time.HasValue)
					_event_time = computeEventTime();

				return _event_time.Value;
			}
		}

		private DateTime computeEventTime()
		{
			return AttachmentUpload.EventTime.GuessFromExif(exif, file_create_time, timezone, file_path);
		}
	}

	[Serializable]
	public class FolderCollection
	{
		public string FolderPath { get; private set; }
		public string FolderName { get; set; }
		public List<string> Objects { get; private set; }

		public FolderCollection(string folderPath)
		{
			FolderPath = folderPath;
			FolderName = string.IsNullOrEmpty(Path.GetFileName(folderPath)) ? folderPath : Path.GetFileName(folderPath);
			Objects = new List<string>();
		}

		public FolderCollection(string folderPath, string object_id)
			: this(folderPath)
		{
			Objects.Add(object_id);
		}

		public override int GetHashCode()
		{
			return FolderPath.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return FolderPath.Equals(obj);
		}

		public static Dictionary<string, FolderCollection> Build(IEnumerable<ObjectIdAndPath> files)
		{
			var folderCollections = new Dictionary<string, FolderCollection>();

			foreach (var file in files)
			{
				var folderPath = Path.GetDirectoryName(file.file_path);

				if (folderCollections.ContainsKey(folderPath))
					folderCollections[folderPath].Objects.Add(file.object_id);
				else
					folderCollections.Add(folderPath, new FolderCollection(folderPath, file.object_id));
			}

			return folderCollections;
		}
	}
}
