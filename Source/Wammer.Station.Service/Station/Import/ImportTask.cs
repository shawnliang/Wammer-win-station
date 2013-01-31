using Microsoft.Win32;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.AttachmentUpload;
using Wammer.Utility;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
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


		/// <summary>
		/// Gets or sets the m_ API key.
		/// </summary>
		/// <value>The m_ API key.</value>
		private String m_APIKey { get; set; }

		private List<string> m_IgnorePath { get; set; }

		private int timezoneDiff;
		private List<ObjectIdAndPath> allSavedFiles = new List<ObjectIdAndPath>();
		private List<ObjectIdAndPath> allFailedFiles = new List<ObjectIdAndPath>();
		#endregion

		#region Public Property
		public Guid TaskId { get; private set; }
		#endregion
		
		#region Constructor
		public ImportTask(string apiKey, string sessionToken, string groupID, IEnumerable<string> paths)
			: this()
		{
			m_APIKey = apiKey;
			m_SessionToken = sessionToken;
			m_GroupID = groupID;
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

				var importTime = DateTime.Now;

				var photoCrawler = new PhotoCrawler();
				var inputFiles = Paths.Where(file => Path.GetExtension(file).Length > 0);
				var allFiles = photoCrawler.FindPhotos(Paths, photoCrawlerError).Select(file => new ObjectIdAndPath { file_path = file, object_id = Guid.NewGuid().ToString() }).ToList();

				raiseFilesEnumeratedEvent(allFiles.Count);

				// index files, generate metadata
				var allIndexed = new HashSet<FileMetadata>();
				var nIndexed = 0;
				do
				{
					var batch = allFiles.Skip(nIndexed).Take(50);
					var metadata = extractMetadata(batch, allIndexed);

					enqueueUploadMetadataTask(metadata);
					nIndexed += batch.Count();

				} while (nIndexed < allFiles.Count);


				if (allIndexed.Count == 0)
					throw new Exception("No file needs to import");

				var allMeta = allIndexed.ToList();
				allMeta.Sort((x, y) => y.EventTime.CompareTo(x.EventTime));


				// build collections
				var folderCollections = FolderCollection.Build(allMeta.Where(meta => !inputFiles.Contains(meta.file_path)).Cast<ObjectIdAndPath>());
				TaskQueue.Enqueue(new CreateFolderCollectionTask(folderCollections, m_GroupID), TaskPriority.High);


				// copy file to stream
				int nProc = 0;
				do
				{
					var batch = allMeta.Skip(nProc).Take(50);
					var saved = submitBatch(importTime, batch, ref allFailedFiles);

					allSavedFiles.AddRange(saved);

					nProc += batch.Count();

				} while (nProc < allMeta.Count);
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


		private bool hasDupItemInDB(FileMetadata item)
		{
			var sameSizeFiles = AttachmentCollection.Instance.Find(
							Query.And(
								Query.EQ("group_id", m_GroupID),
								Query.EQ("file_size", new FileInfo(item.file_path).Length)));

			bool hasDup = false;
			foreach (var sameSizeFile in sameSizeFiles)
			{
				if (sameSizeFile.file_path.Equals(item.file_path, StringComparison.CurrentCultureIgnoreCase) ||
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
					{CloudServer.PARAM_CONTENT, string.Format("Import {0} files", objectIDs.Count())},
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
				post_id = postID,
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

				if (hasDupItemInDB(meta) || indexedFiles.Contains(meta))
					raiseFileSkippedEvent(file);
				else
				{
					indexedFiles.Add(meta);
					batch.Add(meta);
					raiseFileIndexedEvent(file);
				}
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

		private uint getRationalValue(object[] rational)
		{
			var value = Convert.ToUInt32(rational[0]) / Convert.ToUInt32(rational[1]);
			return value;
		}

		private DateTime? _event_time;
		private long _file_size = -1;

		[JsonIgnore]
		public long file_size
		{
			get
			{
				if (_file_size < 0)
					_file_size = new FileInfo(file_path).Length;

				return _file_size;
			}
		}

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

		public override int GetHashCode()
		{
			return file_name.GetHashCode() + (int)file_size;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj is FileMetadata)
			{
				var rhs = (FileMetadata)obj;
				return file_size == rhs.file_size && file_name.Equals(rhs.file_name);
			}
			else
				return false;
		}

		private DateTime computeEventTime()
		{
			if (exif != null)
			{
				if (exif.gps != null && !string.IsNullOrEmpty(exif.gps.GPSDateStamp) && exif.gps.GPSTimeStamp != null)
				{
					var eventTime = DateTime.ParseExact(exif.gps.GPSDateStamp, "yyyy:MM:dd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);

					var hour = getRationalValue(exif.gps.GPSTimeStamp[0]);
					var min = getRationalValue(exif.gps.GPSTimeStamp[1]);
					var sec = getRationalValue(exif.gps.GPSTimeStamp[2]);

					return eventTime.AddHours((double)hour).AddMinutes((double)min).AddSeconds((double)sec);
				}

				if (exif.DateTimeOriginal != null)
					return TimeHelper.ParseGeneralDateTime(exif.DateTimeOriginal);

				if (exif.DateTimeDigitized != null)
					return TimeHelper.ParseGeneralDateTime(exif.DateTimeDigitized);

				if (exif.DateTime != null)
					return TimeHelper.ParseGeneralDateTime(exif.DateTime);
			}

			return file_create_time;
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
