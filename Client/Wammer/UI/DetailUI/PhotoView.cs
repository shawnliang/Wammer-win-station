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

#endregion

namespace Waveface.DetailUI
{
    public partial class PhotoView : Form
    {
        private const string MEDIUM = "Medium";
        private const string ORIGIN = "Origin";
        private const string LOADING = "Loading";

        private Dictionary<string, string> m_filesMapping;
        private ImageListViewItem m_selectedImage;
        private bool m_onlyOnePhoto;
        private int m_loadingPhotosCount;
        private int m_loadingOriginPhotosCount;
        private List<Attachment> m_imageAttachments;
        private List<string> m_filePathOrigins;
        private List<string> m_filePathMediums;
        private int m_selectedImageIndex;
        private int m_initSelectedImageIndex;
        private MyImageListViewRenderer m_imageListViewRenderer;
        private string m_selectedImageType;
        private Post m_post;
        private FormSettings m_formSettings;

        public PhotoView()
        {
            InitializeComponent();
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

            imageListView.View = View.Gallery;
            imageListView.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

            m_imageListViewRenderer = new MyImageListViewRenderer();

            imageListView.SetRenderer(m_imageListViewRenderer);

            m_initSelectedImageIndex = selectedIndex;
        }

        private void PhotoView_Load(object sender, EventArgs e)
        {
            imageListView.Dock = DockStyle.Fill; //Hack

            timer.Interval = ((m_imageAttachments.Count / 200) + 2) * 1000;

            if (!FillImageListView(true))
                timer.Enabled = true;

            ReArrangeUI();

            setSelectedItem(m_initSelectedImageIndex);

            imageListView.Select();
        }

        private void setSelectedItem(int selectedIndex)
        {
            imageListView.ClearSelection();

            imageListView.Items[selectedIndex].Focused = true;
            imageListView.Items[selectedIndex].Selected = true;

            imageListView.EnsureVisible(selectedIndex);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            FillImageListView(false);
        }

        private bool FillImageListView(bool firstTime)
        {
            int _count = 0;
            int _origin = 0;

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (File.Exists(m_filePathOrigins[i]))
                {
                    _count++;
                    _origin++;

                    continue;
                }

                if (File.Exists(m_filePathMediums[i]))
                {
                    _count++;

                    continue;
                }
            }

            bool _show1 = (m_loadingPhotosCount != m_imageAttachments.Count);
            bool _show2 = (m_loadingOriginPhotosCount != _origin);

            m_loadingPhotosCount = _count;
            m_loadingOriginPhotosCount = _origin;

            if (_origin == m_imageAttachments.Count)
            {
                ShowImageListView(firstTime);

                timer.Enabled = false;
                return true;
            }

            if (_show1 || _show2)
            {
                ShowImageListView(firstTime);
                return false;
            }

            return false;
        }

        private void ShowImageListView(bool firstTime)
        {
            int k = 0;

            bool _reload = firstTime || checkSelectedImageTypeChange();

            if (_reload)
                imageListView.Items.Clear();

            imageListView.SuspendLayout();

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (File.Exists(m_filePathOrigins[i]))
                {
                    if (_reload)
                        imageListView.Items.Add(m_filePathOrigins[i]);
                    else
                        imageListView.Items[i].FileName = m_filePathOrigins[i];

                    imageListView.Items[i].Tag = new ImageViewItemTAG { Index = i, Type = ORIGIN };

                    k++;

                    continue;
                }

                if (File.Exists(m_filePathMediums[i]))
                {
                    if (_reload)
                        imageListView.Items.Add(m_filePathMediums[i]);
                    else
                        imageListView.Items[i].FileName = m_filePathMediums[i];

                    imageListView.Items[i].Tag = new ImageViewItemTAG { Index = i, Type = MEDIUM };

                    k++;

                    continue;
                }

                if (_reload)
                    imageListView.Items.Add(Main.Current.LoadingImagePath);
                else
                    imageListView.Items[i].FileName = Main.Current.LoadingImagePath;

                imageListView.Items[i].Tag = new ImageViewItemTAG { Index = i, Type = LOADING };
            }

            imageListView.ResumeLayout();

            if (!firstTime)
            {
                if (m_selectedImage != null)
                {
                    setSelectedItem(m_selectedImageIndex);
                }
            }
        }

        private bool checkSelectedImageTypeChange()
        {
            if (m_selectedImage == null)
                return true;

            bool _originExist = File.Exists(m_filePathOrigins[m_selectedImageIndex]);
            bool _mediumsExist = File.Exists(m_filePathMediums[m_selectedImageIndex]);

            switch (m_selectedImageType)
            {
                case LOADING:

                    if (_originExist || _mediumsExist)
                        return true;

                    break;
                case MEDIUM:

                    if (_originExist)
                        return true;

                    break;
            }

            return false;
        }

        private void imageListView_SelectionChanged(object sender, EventArgs e)
        {
            if (imageListView.SelectedItems.Count != 0)
            {
                m_selectedImage = imageListView.SelectedItems[0];
                m_selectedImageIndex = ((ImageViewItemTAG)imageListView.SelectedItems[0].Tag).Index;
                m_selectedImageType = ((ImageViewItemTAG)imageListView.SelectedItems[0].Tag).Type;

                string _trueName = new FileInfo(m_selectedImage.FileName).Name;

                if (m_filesMapping.ContainsKey(_trueName)) //取得原始檔名
                    _trueName = m_filesMapping[_trueName];

                if (_trueName == "LoadingImage.jpg")
                {
                    StatusLabelFileName.Text = "";

                    StatusLabelCurrentSize.Text = "";

                    btnSave.Visible = false;
                    miSave.Visible = false;
                }
                else
                {
                    StatusLabelFileName.Text = _trueName;

                    StatusLabelCurrentSize.Text = m_selectedImage.Dimensions.Width + " x " +
                                                  m_selectedImage.Dimensions.Height;

                    btnSave.Visible = true;
                    miSave.Visible = true;
                }

                SetImageBox();
            }
        }

        private void SetImageBox()
        {
            imageBox.Image = new Bitmap(m_selectedImage.FileName);

            imageBox.ZoomToFit();
        }

        private void btnSlideShow_Click(object sender, EventArgs e)
        {
            List<string> _imageFilesPath = new List<string>();

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (File.Exists(m_filePathOrigins[i]))
                {
                    _imageFilesPath.Add(m_filePathOrigins[i]);
                    continue;
                }

                if (File.Exists(m_filePathMediums[i]))
                {
                    _imageFilesPath.Add(m_filePathMediums[i]);
                    continue;
                }
            }

            if (_imageFilesPath.Count > 0)
            {
                using (SlideShowForm _form = new SlideShowForm(_imageFilesPath, m_selectedImageIndex))
                {
                    _form.ShowDialog();
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
            if (imageListView.SelectedItems.Count != 0)
            {
                m_selectedImage = imageListView.SelectedItems[0];
            }

            string _picFilePath = m_selectedImage.FileName;
            string _trueName = new FileInfo(_picFilePath).Name;

            if (m_filesMapping.ContainsKey(_trueName)) //取得原始檔名
                _trueName = m_filesMapping[_trueName];

            saveFileDialog.FileName = _trueName;
            DialogResult _dr = saveFileDialog.ShowDialog();

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
            string _cover_attach = m_imageAttachments[m_selectedImageIndex].object_id;

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
            ReArrangeUI();
        }

        private void ReArrangeUI()
        {
            int _x = (Width - 240 - 20) / 2;

            btnCoverImage.Left = _x;
            btnSlideShow.Left = _x + 120;
            btnSave.Left = _x + 240;

            btnCoverImage.Visible = true;
            btnSlideShow.Visible = true;
            btnSave.Visible = true;

            imageBox.Left = 0;
            imageBox.Top = panelTop.Height;
            imageBox.Width = imageListView.Width;
            imageBox.Height = imageListView.Height - 164;
            imageBox.Refresh();
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

        private void imageBox_KeyDown(object sender, KeyEventArgs e)
        {
            SendKeyToImageListView(e);
        }

        private void SendKeyToImageListView(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    imageListView.Focus();
                    SendKeys.Send("{LEFT}");

                    break;
                case Keys.Right:
                    imageListView.Focus();
                    SendKeys.Send("{RIGHT}");

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
    }
}