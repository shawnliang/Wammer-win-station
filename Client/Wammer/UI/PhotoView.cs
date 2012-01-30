#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
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

        public PhotoView()
        {
            InitializeComponent();
        }

        public PhotoView(List<string> files, Dictionary<string, string> filesMapping, string fileName)
        {
            InitializeComponent();

            m_filesMapping = filesMapping;

            foreach (string _file in files)
            {
                imageListView.Items.Add(_file);
            }

            m_onlyOnePhoto = (imageListView.Items.Count == 1);

            if (m_onlyOnePhoto)
                btnSaveAll.Visible = false;

            imageListView.View = View.Gallery;
            imageListView.CacheMode = CacheMode.Continuous;

            imageListView.AutoRotateThumbnails = false;
            imageListView.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

            MyImageListViewRenderer _imageListViewRenderer = new MyImageListViewRenderer();
            _imageListViewRenderer.Clip = false;

            imageListView.SetRenderer(_imageListViewRenderer);

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

        private void imageListView_SelectionChanged(object sender, EventArgs e)
        {
            if (imageListView.SelectedItems.Count != 0)
            {
                m_selectedImage = imageListView.SelectedItems[0];

                string _trueName = new FileInfo(m_selectedImage.FileName).Name;

                if (m_filesMapping.ContainsKey(_trueName)) //取得原始檔名
                    _trueName = m_filesMapping[_trueName];

                StatusLabelFileName.Text = _trueName;
                //StatusLabelOriginSize.Text = ;
                //StatusLabelCurrentSize.Text = ;
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