#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Configuration;
using View = Manina.Windows.Forms.View;
using System.Diagnostics;
using Waveface.Libs.StationDB;
using Waveface.Properties;

#endregion

namespace Waveface.DetailUI
{
	/// <summary>
	/// 
	/// </summary>
    public partial class PhotoView : Form
    {
		#region Var
		private List<string> _filePathMediums;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the m_ file path mediums.
		/// </summary>
		/// <value>The m_ file path mediums.</value>
		private List<string> m_FilePathMediums 
		{
			get
			{
				return _filePathMediums ?? (_filePathMediums = new List<string>());
			}
		}

		/// <summary>
		/// Gets or sets the m_ post.
		/// </summary>
		/// <value>The m_ post.</value>
		private Post m_Post { get; set; }

		/// <summary>
		/// Gets or sets the index of the m_ selected.
		/// </summary>
		/// <value>The index of the m_ selected.</value>
		private int m_SelectedIndex { get; set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PhotoView"/> class.
		/// </summary>
		public PhotoView()
		{
			InitializeComponent();
			//Bounds = Screen.PrimaryScreen.Bounds;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PhotoView"/> class.
		/// </summary>
		/// <param name="post">The post.</param>
		/// <param name="filePathMediums">The file path mediums.</param>
		/// <param name="selectedIndex">Index of the selected.</param>
		public PhotoView(Post post, List<string> filePathMediums, int selectedIndex)
			: this()
		{
			m_Post = post;
			m_SelectedIndex = selectedIndex;
			m_FilePathMediums.AddRange(filePathMediums);
		}
		#endregion



		#region Private Method
		/// <summary>
		/// Saves the pic.
		/// </summary>
		private void SavePic()
		{
			var _picFilePath = GetPhotoFilePath(thumbnailNavigator1.SelectedIndex);
			var object_id = m_Post.attachment_id_array[thumbnailNavigator1.SelectedIndex];
			var trueFileName = AttachmentCollection.QueryFileName(object_id);

			if (string.IsNullOrEmpty(trueFileName))
				trueFileName = Path.GetFileName(_picFilePath);

			saveFileDialog.FileName = trueFileName;

			var extension = Path.GetExtension(trueFileName);
			if (!String.IsNullOrEmpty(extension))
				saveFileDialog.Filter = string.Format(Resources.PHOTO_SAVE_FILTER_PATTERN, extension, extension);

			DialogResult _dr = saveFileDialog.ShowDialog(this);

			if (_dr == DialogResult.OK)
			{
				try
				{
					string _destFile = saveFileDialog.FileName;

					File.Copy(_picFilePath, _destFile, true);
				}
				catch
				{
					Toast.MakeText(imageBox, Resources.SAVE_ERROR, Toast.LENGTH_SHORT).Show();
				}
			}
		}

		/// <summary>
		/// Gets the photo file path.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		private string GetPhotoFilePath(int index)
		{
			var object_id = m_Post.attachment_id_array[index];

			var originPhotoFilePath = Main.Current.RT.REST.attachments_getOriginFilePath(object_id);
			if (File.Exists(originPhotoFilePath))
				return originPhotoFilePath;

			var mediumPhotoFilePath = m_FilePathMediums[index];
			if (File.Exists(mediumPhotoFilePath))
				return mediumPhotoFilePath;

			return null;
		}

		/// <summary>
		/// Sets as cover image.
		/// </summary>
		private void SetAsCoverImage()
		{
			string _cover_attach = m_Post.attachment_id_array[thumbnailNavigator1.SelectedIndex];

			if (_cover_attach != m_Post.cover_attach)
			{
				Dictionary<string, string> _params = new Dictionary<string, string>();
				_params.Add("cover_attach", _cover_attach);

				Post _retPost = Main.Current.PostUpdate(m_Post, _params);

				if (_retPost == null)
				{
					return;
				}

				m_Post = _retPost;
			}

			Toast.MakeText(imageBox, Resources.CHANGED_COVER_IMAGE, Toast.LENGTH_SHORT).Show();
		}

		/// <summary>
		/// Sends the key to image list view.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
		private void SendKeyToImageListView(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Left:
					thumbnailNavigator1.PreviousThumbnail();
					break;
				case Keys.Right:
					thumbnailNavigator1.NextThumbnail();
					break;
				case Keys.PageUp:
					thumbnailNavigator1.PreviousPage();
					break;
				case Keys.PageDown:
					thumbnailNavigator1.NextPage();
					break;
				case Keys.Home:
					thumbnailNavigator1.FirstThumbnail();
					break;
				case Keys.End:
					thumbnailNavigator1.LastThumbnail();
					break;
				case Keys.Escape:
					this.Close();
					break;
			}
		}
		#endregion

		
		#region Event Process
		/// <summary>
		/// Handles the SelectedIndexChanged event of the thumbnailNavigator1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void thumbnailNavigator1_SelectedIndexChanged(object sender, EventArgs e)
		{
			imageBox.Image = new Bitmap(GetPhotoFilePath(thumbnailNavigator1.SelectedIndex));
			imageBox.ZoomToFit();
		}

		/// <summary>
		/// Handles the Click event of the btnSlideShow control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnSlideShow_Click(object sender, EventArgs e)
		{
			List<string> _imageFilesPath = new List<string>();

			for (int i = 0; i < m_Post.attachment_id_array.Count; i++)
			{
				var photoFilePath = GetPhotoFilePath(i);
				if (string.IsNullOrEmpty(photoFilePath))
					continue;

				_imageFilesPath.Add(photoFilePath);
			}

			if (_imageFilesPath.Count > 0)
			{
				using (SlideShowForm _form = new SlideShowForm(_imageFilesPath, thumbnailNavigator1.SelectedIndex))
				{
					_form.StartPosition = FormStartPosition.CenterParent;
					_form.ShowDialog(this);
					thumbnailNavigator1.SelectedIndex = _form.m_index;
				}
			}
		}

		/// <summary>
		/// Handles the Click event of the miSave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void miSave_Click(object sender, EventArgs e)
		{
			SavePic();
		}

		/// <summary>
		/// Handles the Click event of the btnSave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnSave_Click(object sender, EventArgs e)
		{
			SavePic();
		}

		/// <summary>
		/// Handles the Click event of the btnCoverImage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnCoverImage_Click(object sender, EventArgs e)
		{
			SetAsCoverImage();
		}

		/// <summary>
		/// Handles the Click event of the miSetAsCoverImage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void miSetAsCoverImage_Click(object sender, EventArgs e)
		{
			SetAsCoverImage();
		}

		/// <summary>
		/// Handles the Resize event of the PhotoView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void PhotoView_Resize(object sender, EventArgs e)
		{
			pnlPhotoViewToolBar.Left = (this.Width - pnlPhotoViewToolBar.Width) / 2;
		}

		/// <summary>
		/// Handles the KeyDown event of the PhotoView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
		private void PhotoView_KeyDown(object sender, KeyEventArgs e)
		{
			SendKeyToImageListView(e);
		}

		/// <summary>
		/// Handles the Click event of the imageBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void imageBox_Click(object sender, EventArgs e)
		{
			imageBox.SizeToFit = false;
			imageBox.AutoPan = true;
			//imageBox.AdjustLayout();
		}

		/// <summary>
		/// Handles the Click event of the imageButton1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void imageButton1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Handles the Load event of the PhotoView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void PhotoView_Load(object sender, EventArgs e)
		{
			this.FullScreen();

			try
			{
				thumbnailNavigator1.SuspendLayout();
				thumbnailNavigator1.ThumbnailPadding = new Padding(3, 3, 3, 3);
				thumbnailNavigator1.ThumbnailWidth = 64;
				thumbnailNavigator1.SelectedIndexChanged += new EventHandler(thumbnailNavigator1_SelectedIndexChanged);
				thumbnailNavigator1.LoadThumbnails(m_FilePathMediums, m_SelectedIndex);
			}
			finally
			{
				thumbnailNavigator1.ResumeLayout();
			}
		}
		#endregion
    }
}
