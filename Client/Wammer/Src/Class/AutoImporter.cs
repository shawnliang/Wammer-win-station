using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Waveface.Upload;
using Waveface.API.V2;
using System.Drawing;

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


		#region Public Method
		/// <summary>
		/// Imports this instance.
		/// </summary>
		public void Import()
		{
			var validContents = from content in m_ContentProvider.GetContents()
								let info = new FileInfo(content.FilePath)
								//let img = Bitmap.FromFile(content.FilePath)
								where info.Length >= 20 * 1024 //&& img.Height >= 256 && img.Width >= 256
								select content;
								
			var contentGroup = validContents.DistinctBy(content => content.FilePath).GroupBy(content => content.Path);

			foreach (var contents in contentGroup)
			{
				var postID = Guid.NewGuid().ToString();
				var uploadItems = (from content in contents
								  select new UploadItem
								  {
									  file_path = content.FilePath,
									  object_id = Guid.NewGuid().ToString(),
									  post_id = postID
								  }).ToArray();

				var objectIDs = uploadItems.Select(item => item.object_id).ToArray();

				var post = Main.Current.RT.REST.Posts_New(
					postID,
					StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(contents.Key.Split(new char[]{Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(), 80000)),
					"[" + string.Join(",", objectIDs.Select(id => "\"" + id + "\"").ToArray()) + "]",
					"",
					"image",
					objectIDs.FirstOrDefault());

				Main.Current.Uploader.Add(uploadItems);
			} 
		}
		#endregion
	}
}
