using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wammer.Station
{
	public class FolderMover
	{
		private IFolderUtility util;

		public FolderMover(IFolderUtility util)
		{
			this.util = util;
		}

		public void MoveFolder(string src, string dest)
		{
			var srcDirs = util.GetSubDirectories(src);
			var entriesInDest = util.GetSubEntries(dest).Select(x => Path.GetFileName(x));
			foreach (var srcDir in srcDirs)
			{
				var dirname = Path.GetFileName(srcDir);

				if (entriesInDest.Contains(dirname))
					throw new DestinationExistException(dirname + " already exists under " + dest);
			}

			if (util.IsOnSameDrive(src, dest))
			{
				util.MoveOnSameDrive(src, dest);
			}
			else
			{
				util.RecursiveCopy(src, dest);

				try
				{
					util.RecursiveDelete(src);
				}
				catch (Exception e)
				{
					this.LogWarnMsg("Unable to remove old folder: " + src, e);
				}
			}
		}
	}

	public interface IFolderUtility
	{
		bool IsOnSameDrive(string path1, string path2);
		void MoveOnSameDrive(string src, string dest);
		void RecursiveCopy(string src, string dest);
		void RecursiveDelete(string path);

		IEnumerable<string> GetSubDirectories(string path);
		IEnumerable<string> GetSubEntries(string path);
	}

	public class DestinationExistException : ApplicationException
	{
		public DestinationExistException(string msg)
			: base(msg)
		{
		}
	}
}
