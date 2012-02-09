#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using View = Manina.Windows.Forms.View;

#endregion

namespace Waveface
{
    public partial class PhotoView : Form
    {
        private Dictionary<string, string> m_filesMapping;
        private ImageListViewItem m_selectedImage;
        private bool m_onlyOnePhoto;
        private int m_loadingPhotosCount;
        private List<Attachment> m_imageAttachments;
        private List<string> m_filePathOrigins;
        private List<string> m_filePathMediums;
        private int m_selectedImageIndex;

        public PhotoView()
        {
            InitializeComponent();
        }

        public PhotoView(List<Attachment> imageAttachments, List<string> filePathOrigins, List<string> filePathMediums,
                         Dictionary<string, string> filesMapping, string fileName)
        {
            InitializeComponent();

            m_imageAttachments = imageAttachments;
            m_filesMapping = filesMapping;
            m_filePathOrigins = filePathOrigins;
            m_filePathMediums = filePathMediums;

            m_onlyOnePhoto = (m_imageAttachments.Count == 1);

            if (m_onlyOnePhoto)
                btnSaveAll.Visible = false;

            imageListView.View = View.Gallery;
            imageListView.CacheMode = CacheMode.Continuous;

            //imageListView.AutoRotateThumbnails = false;
            imageListView.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

            MyImageListViewRenderer _imageListViewRenderer = new MyImageListViewRenderer();
            _imageListViewRenderer.Clip = false;

            imageListView.SetRenderer(_imageListViewRenderer);


            timer.Interval = ((m_imageAttachments.Count / 200) + 2) * 1000;

            if (!FillImageListView(true))
                timer.Enabled = true;

            initSelectedItem(fileName);
        }

        private void initSelectedItem(string fileName)
        {
            int i = 0;

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                if (fileName == _item.FileName)
                {
                    _item.Selected = true;
                    _item.Focused = true;

                    imageListView.EnsureVisible(i);

                    return;
                }

                i++;
            }

            if (imageListView.Items.Count > 0)
            {
                imageListView.Items[0].Selected = true;
                imageListView.Items[0].Focused = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            FillImageListView(false);

            Application.DoEvents();
        }

        private bool FillImageListView(bool firstTime)
        {
            int _count = 0;
            int _orig = 0;

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (File.Exists(m_filePathOrigins[i]))
                {
                    _count++;
                    _orig++;

                    continue;
                }

                if (File.Exists(m_filePathMediums[i]))
                {
                    _count++;

                    continue;
                }
            }

            bool _show = (m_loadingPhotosCount != m_imageAttachments.Count);

            m_loadingPhotosCount = _count;

            if (firstTime || _show)
            {
                ShowImageListView(firstTime);
                return false;
            }

            if (_orig == m_imageAttachments.Count)
            {
                timer.Enabled = false;

                ShowImageListView(firstTime);

                return true;
            }

            return false;
        }

        private bool ShowImageListView(bool firstTime)
        {
            int k = 0;

            imageListView.SuspendLayout();

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (File.Exists(m_filePathOrigins[i]))
                {
                    if (firstTime)
                    {
                        imageListView.Items.Add(m_filePathOrigins[i]);
                    }
                    else
                    {
                        imageListView.Items[i].FileName = m_filePathOrigins[i];
                    }

                    imageListView.Items[i].Tag = i.ToString();

                    k++;

                    continue;
                }

                if (File.Exists(m_filePathMediums[i]))
                {
                    if (firstTime)
                    {
                        imageListView.Items.Add(m_filePathMediums[i]);
                    }
                    else
                    {
                        imageListView.Items[i].FileName = m_filePathMediums[i];
                    }

                    imageListView.Items[i].Tag = i.ToString();

                    k++;

                    continue;
                }

                if (firstTime)
                    imageListView.Items.Add(Main.GCONST.CachePath + "LoadingImage" + ".jpg");
                else
                    imageListView.Items[i].FileName = Main.GCONST.CachePath + "LoadingImage" + ".jpg";

                imageListView.Items[i].Tag = i.ToString();
            }

            imageListView.ResumeLayout();

            UpdateMainImage();

            Application.DoEvents();

            return (k == m_imageAttachments.Count);
        }

        private void UpdateMainImage()
        {
            /*
            if (m_selectedImage != null)
            {
                imageListView.Items.FocusedItem = imageListView.Items[m_selectedImageIndex];
                imageListView.Update();
            }
            */
        }

        private void imageListView_SelectionChanged(object sender, EventArgs e)
        {
            if (imageListView.SelectedItems.Count != 0)
            {
                m_selectedImage = imageListView.SelectedItems[0];
                m_selectedImageIndex = int.Parse(imageListView.SelectedItems[0].Tag.ToString());

                string _trueName = new FileInfo(m_selectedImage.FileName).Name;

                if (m_filesMapping.ContainsKey(_trueName)) //取得原始檔名
                    _trueName = m_filesMapping[_trueName];

                StatusLabelFileName.Text = _trueName;
            }
        }

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_onlyOnePhoto)
                miSaveAll.Visible = false;
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
    }
}