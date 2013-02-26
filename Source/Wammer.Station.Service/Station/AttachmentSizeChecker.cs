using MongoDB.Driver.Builders;
using System.Collections.Generic;
using System.IO;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	class AttachmentSizeChecker : NonReentrantTimer
	{
		public AttachmentSizeChecker()
			: base(3 * 60 * 1000)
		{
		}

		protected override void ExecuteOnTimedUp(object state)
		{
			foreach (var user in DriverCollection.Instance.FindAll())
			{
				if (user.ReachOriginSizeLimit())
				{
					removeLeastRecentlyUsedOriginalFiles(user);
				}
			}
		}

		private void removeLeastRecentlyUsedOriginalFiles(Driver user)
		{
			var filesToDelete = selectFilesToDelete(user);

			foreach (var file in filesToDelete)
			{
				FileStorage storage = new FileStorage(user);
				var origFile = Path.Combine(storage.basePath, file.file_path);

				var file_size = new FileInfo(origFile).Length;

				try
				{
					File.Delete(origFile);
				}
				catch
				{
					continue;
				}

				AttachmentCollection.Instance.Update(
					Query.EQ("_id", file.object_id),
					Update.Unset("saved_file_name").Unset("file_size").Unset("url"));

				DriverCollection.Instance.Update(
					Query.EQ("_id", user.user_id),
					Update.Inc("cur_origin_size", -file_size));
			}

		}

		private static List<ObjectIdAndPath> selectFilesToDelete(Driver user)
		{
			var sizeToDelete = user.cur_origin_size - user.origin_limit;

			var query = Query.And(
				Query.EQ("group_id", user.groups[0].group_id),
				Query.Exists("saved_file_name"),
				Query.EQ("body_on_cloud", true));

			var files = AttachmentCollection.Instance.Find(query)
				.SetSortOrder(SortBy.Ascending("last_access"))
				.SetLimit(1000);

			var selectedSize = 0L;
			var filesToDelete = new List<ObjectIdAndPath>();

			foreach (var file in files)
			{
				if (selectedSize >= sizeToDelete)
					break;

				selectedSize += file.file_size;
				filesToDelete.Add(new ObjectIdAndPath { file_path = file.saved_file_name, object_id = file.object_id });
			}
			return filesToDelete;
		}
	}
}
