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

#endregion

namespace Waveface.DetailUI
{
    public partial class PhotoView : Form
    {
        private const string MEDIUM = "Medium";
        private const string ORIGIN = "Origin";
        private const string LOADING = "Loading";

        private Dictionary<string, string> m_filesMapping;
        private bool m_onlyOnePhoto;
        private int m_loadingPhotosCount;
        private int m_loadingOriginPhotosCount;
        private List<Attachment> m_imageAttachments;
        private List<string> m_filePathOrigins;
        private List<string> m_filePathMediums;
        private string m_selectedImageType;
        private Post m_post;
        private FormSettings m_formSettings;

        public PhotoView()
        {
            InitializeComponent();
			Bounds = Screen.PrimaryScreen.Bounds;
			this.FullScreen();
        }

        public PhotoView(Post post, List<Attachment> imageAttachments, List<string> filePathOrigins, List<string> filePathMediums, Dictionary<string, string> filesMapping, int selectedIndex)
            : this()
        {
            m_formSettings = new FormSettings(this);
            m_formSettings.UseLocation = true;
            m_formSettings.UseSize = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;

            m_post = post;

            m_imageAttachments = imageAttachments;
            m_filesMapping = filesMapping;
            m_filePathOrigins = filePathOrigins;
            m_filePathMediums = filePathMediums;

            m_onlyOnePhoto = (m_imageAttachments.Count == 1);

            if (m_onlyOnePhoto)
            {
                btnCoverImage.Visible = false;
                miSetAsCoverImage.Visible = false;
            }

			thumbnailNavigator1.ThumbnailPadding = new Padding(3, 3, 3, 3);
			thumbnailNavigator1.ThumbnailWidth = 64;
			thumbnailNavigator1.SelectedIndexChanged += new EventHandler(thumbnailNavigator1_SelectedIndexChanged);
			thumbnailNavigator1.LoadThumbnails(filePathMediums, selectedIndex);
        }

		void thumbnailNavigator1_SelectedIndexChanged(object sender, EventArgs e)
		{
			imageBox.Image = new Bitmap(m_filePathMediums[thumbnailNavigator1.SelectedIndex]);

			imageBox.ZoomToFit();
		}

		private string GetPhotoFilePath(int index)
		{
			var originPhotoFilePath = m_filePathOrigins[index];
			if (File.Exists(originPhotoFilePath))
				return originPhotoFilePath;

			var mediumPhotoFilePath = m_filePathMediums[index];
			if (File.Exists(mediumPhotoFilePath))
				return mediumPhotoFilePath;

			return null;
		}
	

        private void btnSlideShow_Click(object sender, EventArgs e)
        {
            List<string> _imageFilesPath = new List<string>();

			for (int i = 0; i < m_imageAttachments.Count; i++)
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

        #region Save

        private void miSave_Click(object sender, EventArgs e)
        {
            SavePic();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePic();
        }

		private void SavePic()
		{
			string _picFilePath = GetPhotoFilePath(thumbnailNavigator1.SelectedIndex);
			string _trueName = new FileInfo(_picFilePath).Name;

			if (m_filesMapping.ContainsKey(_trueName)) //取得原始檔名
				_trueName = m_filesMapping[_trueName];

			saveFileDialog.FileName = _trueName;
			DialogResult _dr = saveFileDialog.ShowDialog(this);
			
			if (_dr == DialogResult.OK)
			{
				try
				{
					string _destFile = saveFileDialog.FileName;

					File.Copy(_picFilePath, _destFile, true);

					MessageBox.Show(I18n.L.T("PhotoView.SaveOK"));
				}
				catch
				{
					MessageBox.Show(I18n.L.T("PhotoView.SaveError"));
				}
			}
		}

        #endregion

        private void SetAsCoverImage()
        {
			string _cover_attach = m_imageAttachments[thumbnailNavigator1.SelectedIndex].object_id;

            if (_cover_attach != m_post.cover_attach)
            {
                Dictionary<string, string> _params = new Dictionary<string, string>();
                _params.Add("cover_attach", _cover_attach);

                Post _retPost = Main.Current.PostUpdate(m_post, _params);

                if (_retPost == null)
                {
                    return;
                }

                m_post = _retPost;
            }

            MessageBox.Show(I18n.L.T("ChangedCoverImageOK"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCoverImage_Click(object sender, EventArgs e)
        {
            SetAsCoverImage();
        }

        private void miSetAsCoverImage_Click(object sender, EventArgs e)
        {
            SetAsCoverImage();
        }

        private void PhotoView_Resize(object sender, EventArgs e)
        {
			pnlPhotoViewToolBar.Left = (this.Width - pnlPhotoViewToolBar.Width) / 2;
        }


        private void UpdateStatusBar()
        {
            positionToolStripStatusLabel.Text = imageBox.AutoScrollPosition.ToString();
            imageSizeToolStripStatusLabel.Text = imageBox.GetImageViewPort().ToString();
            zoomToolStripStatusLabel.Text = string.Format("{0}%", imageBox.Zoom);
        }

        private void imageBox_ZoomChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void imageBox_Resize(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateStatusBar();
        }

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
        private void PhotoView_KeyDown(object sender, KeyEventArgs e)
        {
			SendKeyToImageListView(e);
        }

        private void imageBox_Click(object sender, EventArgs e)
        {
            imageBox.SizeToFit = false;
            imageBox.AutoPan = true;
            imageBox.AdjustLayout();
        }

		private void imageButton1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void imageBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				thumbnailNavigator1.NextThumbnail();
			else if (e.Button == System.Windows.Forms.MouseButtons.Right)
				thumbnailNavigator1.PreviousThumbnail();
		}
    }
}