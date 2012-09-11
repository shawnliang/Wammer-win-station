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

			var importDate = DateTimeHelp.ToISO8601(DateTime.Now);
			var processPath = string.Empty;
			var postID = string.Empty;
			var processedPaths = new HashSet<string>();

			var uploadItems = new List<UploadItem>();
			var pendingUploadItems = new List<UploadItem>();
			foreach (var content in importContents)
			{
				try
				{
					var contentPath = content.Path;
					if (content.Path == systemResourcePath)
						continue;

					if (processedPaths.Contains(contentPath))
						continue;

					var contentLength = (new FileInfo(content.FilePath)).Length;
					if (contentLength < 20 * 1024)
						continue;

					var frame = GetBitmapFrame(content.FilePath);
					if (frame == null || frame.Height < 256 || frame.Width < 256)
						continue;

					var objectID = Guid.NewGuid().ToString();

					if (processPath != contentPath)
					{
						if (processPath.Length > 0 && uploadItems.Count > 0)
						{
							CreatePost(postID, uploadItems, processPath, importDate);
							processedPaths.Add(processPath);
						}

						processPath = contentPath;
						postID = Guid.NewGuid().ToString();
					}

					var uploadItem = new UploadItem()
					{
						file_path = content.FilePath,
						object_id = objectID,
						post_id = postID
					};

					if (uploadItems.Count == 0)
						Main.Current.Uploader.Add(uploadItem);
					else
						pendingUploadItems.Add(uploadItem);

					uploadItems.Add(uploadItem);
				}
				catch (Exception)
				{
				}
			}

			if (processPath.Length > 0 && uploadItems.Count > 0)
				CreatePost(postID, uploadItems, processPath, importDate);

			Main.Current.Uploader.Add(pendingUploadItems);
		}

		/// <summary>
		/// Creates the post.
		/// </summary>
		/// <param name="postID">The post ID.</param>
		/// <param name="uploadItems">The upload items.</param>
		/// <param name="contentPath">The content path.</param>
		private static void CreatePost(string postID, List<UploadItem> uploadItems, string contentPath, string memo = null)
		{
			var objectIDs = uploadItems.Select(item => item.object_id).ToArray();
			var post = Main.Current.RT.REST.Posts_New(
				postID,
				StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(contentPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(), 80000)),
				"[" + string.Join(",", objectIDs.Select(id => "\"" + id + "\"").ToArray()) + "]",
				"",
				"image",
				objectIDs.FirstOrDefault(),
				memo);


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
			uploadItems.Clear();
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
