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
    public partial class PhotoView : Form
    {
        private bool m_onlyOnePhoto;
        private List<string> m_filePathMediums;
        private Post m_post;
        private FormSettings m_formSettings;

        public PhotoView()
        {
            InitializeComponent();
            Bounds = Screen.PrimaryScreen.Bounds;
            this.FullScreen();
        }

        public PhotoView(Post post, List<string> filePathMediums, int selectedIndex)
            : this()
        {
            m_formSettings = new FormSettings(this);
            m_formSettings.UseLocation = true;
            m_formSettings.UseSize = true;
            m_formSettings.UseWindowState = true;
            m_formSettings.AllowMinimized = false;
            m_formSettings.SaveOnClose = true;

            m_post = post;

            m_filePathMediums = filePathMediums;

            m_onlyOnePhoto = (m_post.attachment_id_array.Count == 1);

            thumbnailNavigator1.ThumbnailPadding = new Padding(3, 3, 3, 3);
            thumbnailNavigator1.ThumbnailWidth = 64;
            thumbnailNavigator1.SelectedIndexChanged += new EventHandler(thumbnailNavigator1_SelectedIndexChanged);
            thumbnailNavigator1.LoadThumbnails(filePathMediums, selectedIndex);
        }

        void thumbnailNavigator1_SelectedIndexChanged(object sender, EventArgs e)
        {
            imageBox.Image = new Bitmap(GetPhotoFilePath(thumbnailNavigator1.SelectedIndex));
            imageBox.ZoomToFit();
        }

        private string GetPhotoFilePath(int index)
        {
            var object_id = m_post.attachment_id_array[index];

            var originPhotoFilePath = Main.Current.RT.REST.attachments_getOriginFilePath(object_id);
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

            for (int i = 0; i < m_post.attachment_id_array.Count; i++)
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
            var _picFilePath = GetPhotoFilePath(thumbnailNavigator1.SelectedIndex);
            var object_id = m_post.attachment_id_array[thumbnailNavigator1.SelectedIndex];
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

                    //Toast.MakeText(imageBox, I18n.L.T("PhotoView.SaveOK"), Toast.LENGTH_SHORT).Show();
                    //MessageBox.Show(I18n.L.T("PhotoView.SaveOK"));
                }
                catch
                {
                    Toast.MakeText(imageBox, I18n.L.T("PhotoView.SaveError"), Toast.LENGTH_SHORT).Show();
                    //MessageBox.Show(I18n.L.T("PhotoView.SaveError"));
                }
            }
        }

        #endregion

        private void SetAsCoverImage()
        {
            string _cover_attach = m_post.attachment_id_array[thumbnailNavigator1.SelectedIndex];

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

            Toast.MakeText(imageBox, I18n.L.T("ChangedCoverImageOK"), Toast.LENGTH_SHORT).Show();
            //MessageBox.Show(I18n.L.T("ChangedCoverImageOK"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            //thumbnailNavigator1.Visible = !thumbnailNavigator1.Visible;
            //panelBottom.Visible = thumbnailNavigator1.Visible;
            //imageBox.ZoomToFit();
            imageBox.SizeToFit = false;
            imageBox.AutoPan = true;
            imageBox.AdjustLayout();
        }

        private void imageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
