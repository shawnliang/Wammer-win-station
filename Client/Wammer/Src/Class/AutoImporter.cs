using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Waveface.Upload;
using Waveface.API.V2;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Waveface
{
	public class AutoImporter
	{
		#region Var
		private AutoImportContentProvider _contentProvider;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ content provider.
		/// </summary>
		/// <value>The m_ content provider.</value>
		public AutoImportContentProvider m_ContentProvider 
		{
			get
			{
				return _contentProvider ?? (_contentProvider = new AutoImportContentProvider());
			}
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Gets the bitmap frame.
		/// </summary>
		/// <param name="photoFile">The photo file.</param>
		/// <returns></returns>
		private BitmapFrame GetBitmapFrame(string photoFile)
		{
			DebugInfo.ShowMethod();
			try
			{
				var decoder = BitmapDecoder.Create(new Uri(photoFile), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
				return decoder.Frames.FirstOrDefault();
			}
			catch (Exception e)
			{
				return null;
			}
		}

		/// <summary>
		/// Imports the specified contents.
		/// </summary>
		/// <param name="contents">The contents.</param>
		private void Import(IEnumerable<IContent> importContents)
		{
			DebugInfo.ShowMethod();
			var systemResourcePath = StationRegHelper.GetValue("ResourceFolder", "");
			var validContents = from content in importContents
								where content.Path != systemResourcePath
								let info = new FileInfo(content.FilePath)
								where info.Length >= 20 * 1024
								let frame = GetBitmapFrame(content.FilePath)
								where frame != null && frame.Height >= 256 && frame.Width >= 256
								select content;

			var contentGroup = validContents.GroupBy(content => content.Path);


			var coverUploadItems = new List<UploadItem>();
			var otherUploadItems = new List<UploadItem>();
			foreach (var contents in contentGroup)
			{
				var distinctedContents = contents.DistinctBy(content => content.FilePath);
				var postID = Guid.NewGuid().ToString();
				var uploadItems = (from content in distinctedContents
								   select new UploadItem
								   {
									   file_path = content.FilePath,
									   object_id = Guid.NewGuid().ToString(),
									   post_id = postID
								   }).ToList();

				var objectIDs = uploadItems.Select(item => item.object_id).ToArray();

				var post = Main.Current.RT.REST.Posts_New(
					postID,
					StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(contents.Key.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(), 80000)),
					"[" + string.Join(",", objectIDs.Select(id => "\"" + id + "\"").ToArray()) + "]",
					"",
					"image",
					objectIDs.FirstOrDefault());


				var sources = new Dictionary<string, string>();
				foreach (var item in uploadItems)
				{
					sources.Add(item.object_id,
						item.file_path);
				}


				Main.Current.ReloadAllData(
					new PhotoPostInfo
					{
						post_id = postID,
						sources = sources
					});

				coverUploadItems.Add(uploadItems.FirstOrDefault());

				uploadItems.RemoveAt(0);
				otherUploadItems.AddRange(uploadItems);
			}
			Main.Current.Uploader.Add(coverUploadItems);
			Main.Current.Uploader.Add(otherUploadItems);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Imports this instance.
		/// </summary>
		public void Import()
		{
			DebugInfo.ShowMethod();

			Import(m_ContentProvider.GetContents());
		}


		/// <summary>
		/// Imports the specified folder.
		/// </summary>
		/// <param name="folder">The folder.</param>
		public void Import(string folder)
		{
			DebugInfo.ShowMethod();

			var contents = from file in (new DirectoryInfo(folder)).EnumerateFiles()
						   let extension = Path.GetExtension(file)
						   where extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
						   extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase) ||
						   extension.Equals(".bmp", StringComparison.CurrentCultureIgnoreCase)
						   select (new Content(file, ContentType.Photo) as IContent);
			Import(contents);
		}
		#endregion
	}
}
