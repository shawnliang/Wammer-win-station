﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.IO;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class ImportTaskStaus
	{
		#region Propertis
		[BsonId]
		public Guid Id { get; set; }

		[BsonIgnoreIfNull]
		public string UserId { get; set; }

		[BsonIgnoreIfNull]
		public List<ObjectIdAndPath> CopyFailed { get; set; }
		
		[BsonIgnoreIfNull]
		public List<string> Sources { get; set; }

		[BsonIgnoreIfNull]
		public string Error { get; set; }

		public DateTime Time { get; set; }

		public bool Hidden { get; set; }

		public bool IsStarted { get; set; }
		public bool IsCopyComplete { get; set; }
		
		public int Total { get; set; }
		public int Skipped { get; set; }
		public int Indexed { get; set; }
		public int Copied { get; set; }
		public int Thumbnailed { get; set; }
		public int UploadedCount { get; set; }
		public int UploadedSize { get; set; }

		public int UploadCount { get; set; }
		public int UploadSize { get; set; }
		#endregion

		public ImportTaskStaus()
		{
			CopyFailed = new List<ObjectIdAndPath>();
			Sources = new List<string>();
		}

		#region State getters
		public bool IsPending()
		{
			return !IsStarted;
		}

		public bool IsEnumerating()
		{
			return !IsPending() && Total == 0 && string.IsNullOrEmpty(Error);
		}

		public bool IsEnumerated()
		{
			return Total > 0;
		}

		public bool IsIndexing()
		{
			return IsEnumerated() && Skipped + Indexed < Total && string.IsNullOrEmpty(Error);
		}

		public bool IsIndexed()
		{
			return IsEnumerated() && Skipped + Indexed == Total;
		}

		public bool IsCopying()
		{
			return IsIndexed() && Copied + CopyFailed.Count < Indexed && string.IsNullOrEmpty(Error);
		}

		public bool IsCopied()
		{
			return IsIndexed() && Copied + CopyFailed.Count == Indexed;
		}

		public bool IsThumbnailing()
		{
			return IsCopied() && Thumbnailed < Copied && string.IsNullOrEmpty(Error);
		}

		public bool IsThumbnailed()
		{
			return IsCopied() && Thumbnailed == Copied;
		}

		public bool IsUploading()
		{
			return IsThumbnailed() && UploadedCount < UploadCount && string.IsNullOrEmpty(Error);
		}

		public bool IsUploaded()
		{
			return IsThumbnailed() && UploadedCount == UploadCount;
		}

		public bool IsCompleteSuccessfully()
		{
			return IsUploaded();
		}

		public bool IsInProgress()
		{
			return IsStarted && !IsCompleteSuccessfully() && string.IsNullOrEmpty(Error);
		}

		public bool GetProgress(out int maximum, out int current)
		{
			maximum = current = 0;
	
			if (IsIndexing())
			{
				maximum = Total;
				current = Indexed + Skipped;
			}
			else if (IsCopying())
			{
				maximum = Indexed;
				current = Copied;
			}
			else if (IsThumbnailing())
			{
				maximum = Indexed;
				current = Thumbnailed;
			}
			else if (IsUploading())
			{
				maximum = UploadSize;
				current = UploadedSize;
			}
			else
				return false;

			return true;
		}

		#endregion


		public static ImportTaskStaus operator+(ImportTaskStaus lhs, ImportTaskStaus rhs)
		{
			var result = new ImportTaskStaus
			{
				Total = lhs.Total + rhs.Total,
				Indexed = lhs.Indexed + rhs.Indexed,
				Skipped = lhs.Skipped + rhs.Skipped,

				Copied = lhs.Copied + rhs.Copied,
				Thumbnailed = lhs.Thumbnailed + rhs.Thumbnailed,
				UploadCount = lhs.UploadCount + rhs.UploadCount,
				UploadedCount = lhs.UploadedCount + rhs.UploadedCount,
				UploadSize = lhs.UploadSize + rhs.UploadSize,
				UploadedSize = lhs.UploadedSize + rhs.UploadedSize,

				Error = (lhs.Error != null) ? lhs.Error : rhs.Error,			
				IsCopyComplete = (lhs.IsCopyComplete && rhs.IsCopyComplete),
				IsStarted = lhs.IsStarted || rhs.IsStarted,
				UserId = lhs.UserId
			};

			result.CopyFailed.AddRange(lhs.CopyFailed);
			result.CopyFailed.AddRange(rhs.CopyFailed);

			return result;
		}
	}

	[BsonIgnoreExtraElements]
	public class ObjectIdAndPath
	{
		[BsonIgnoreIfNull]
		public string object_id { get; set; }
		[BsonIgnoreIfNull]
		public string file_path { get; set; }
		[BsonIgnoreIfNull]
		public string Error { get; set; }


		private DateTime? _createTime;

		[BsonIgnore]
		public DateTime CreateTime
		{
			get
			{
				if (_createTime.HasValue)
					return _createTime.Value;
				else
				{
					_createTime = File.GetCreationTime(file_path).TrimToSec();
					return _createTime.Value;
				}
			}
		}
	}
}