#region

using System;
using System.Collections.Generic;
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
                btnSaveAll.Visible = false;
                miSaveAll.Visible = false;

                btnCoverImage.Visible = false;
                miSetAsCoverImage.Visible = false;
            }

            imageListView.View = View.Gallery;
            imageListView.CacheMode = CacheMode.Continuous;

            //imageListView.AutoRotateThumbnails = false;
            imageListView.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

            m_imageListViewRenderer = new MyImageListViewRenderer();
            m_imageListViewRenderer.Clip = false;

            imageListView.SetRenderer(m_imageListViewRenderer);

            m_initSelectedImageIndex = selectedIndex;
        }

        private void PhotoView_Load(object sender, EventArgs e)
        {
            timer.Interval = ((m_imageAttachments.Count / 200) + 2) * 1000;

            if (!FillImageListView(true))
                timer.Enabled = true;

            setSelectedItem(m_initSelectedImageIndex);
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
            }
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

        private bool CheckIfLoadingImage(ImageListViewItem imageListViewItem)
        {
            string _trueName = new FileInfo(imageListViewItem.FileName).Name;

            if (m_filesMapping.ContainsKey(_trueName))
                _trueName = m_filesMapping[_trueName];

            return (_trueName == "LoadingImage.jpg");
        }

        #region Save

        private void miSave_Click(object sender, EventArgs e)
        {
            SavePic();
        }

        private void miSaveAll_Click(object sender, EventArgs e)
        {
            SaveAllPics();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePic();
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            SaveAllPics();
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

        private void SaveAllPics()
        {
            string _fileName = string.Empty;

            using (FolderBrowserDialog _dialog = new FolderBrowserDialog())
            {
                _dialog.Description = I18n.L.T("PhotoView.SelectLoc");
                _dialog.ShowNewFolderButton = true;
                _dialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    string _folder = _dialog.SelectedPath + "\\";

                    foreach (ImageListViewItem _item in imageListView.Items)
                    {
                        if (CheckIfLoadingImage(_item))
                            continue;

                        _fileName = new FileInfo(_item.FileName).Name;

                        if (m_filesMapping.ContainsKey(_fileName))
                            _fileName = m_filesMapping[_fileName]; // 取出真實名稱

                        _fileName = FileUtility.saveFileWithoutOverwrite(_fileName, _folder);

                        try
                        {
                            File.Copy(_item.FileName, _fileName);
                        }
                        catch
                        {
                        }
                    }

                    MessageBox.Show(I18n.L.T("PhotoView.SaveAllOK"), "Waveface", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        #endregion

        private void SetAsCoverImage()
        {
            string _cover_attach = m_imageAttachments[m_selectedImageIndex].object_id;

            if (_cover_attach == m_post.cover_attach)
            {
                //ToDo
            }
            else
            {
                Dictionary<string, string> _params = new Dictionary<string, string>();
                _params.Add("cover_attach", _cover_attach);

                Post _retPost = Main.Current.PostUpdate(m_post, _params);

                if (_retPost != null)
                {
                    m_post = _retPost;

                    // ToDo
                    // Close();
                }
            }
        }

        private void btnCoverImage_Click(object sender, EventArgs e)
        {
            SetAsCoverImage();
        }

        private void miSetAsCoverImage_Click(object sender, EventArgs e)
        {
            SetAsCoverImage();
        }
    }
}