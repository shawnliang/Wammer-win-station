#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
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

            imageListView.View = View.Gallery;

            //MyImageListViewRenderer _imageListViewRenderer = new MyImageListViewRenderer();
            //_imageListViewRenderer.Clip = false;

            imageListView.SetRenderer(new ImageListViewRenderers.NoirRenderer());

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                if (fileName == _item.FileName)
                {
                    _item.Selected = true;
                    return;
                }
            }

            if (imageListView.Items.Count > 0)
                imageListView.Items[0].Selected = true;
        }

        private void imageListView_SelectionChanged(object sender, EventArgs e)
        {
            if (imageListView.SelectedItems.Count != 0)
            {
                m_selectedImage = imageListView.SelectedItems[0];
            }
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

            //Hack
            if (_trueName.Contains("%"))
                _trueName = HttpUtility.UrlDecode(_trueName);

            saveFileDialog.FileName = _trueName;
            DialogResult _dr = saveFileDialog.ShowDialog();

            if (_dr == DialogResult.OK)
            {
                try
                {
                    string _destFile = saveFileDialog.FileName;

                    File.Copy(_picFilePath, _destFile);

                    MessageBox.Show("File Save Successful!");
                }
                catch
                {
                    MessageBox.Show("File Save Error!");
                }
            }
        }

        private void SaveAllPics()
        {
            string _fileName = string.Empty;

            using (FolderBrowserDialog _dialog = new FolderBrowserDialog())
            {
                _dialog.Description = "Select location to save files";
                _dialog.ShowNewFolderButton = true;
                _dialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    string _folder = _dialog.SelectedPath + "\\";

                    foreach (ImageListViewItem _item in imageListView.Items)
                    {
                        _fileName = m_filesMapping[new FileInfo(_item.FileName).Name]; // 取出真實名稱

                        //Hack
                        if (_fileName.Contains("%"))
                            _fileName = HttpUtility.UrlDecode(_fileName);

                        _fileName = FileUtility.saveFileWithoutOverwrite(_fileName, _folder);

                        try
                        {
                            File.Copy(_item.FileName, _fileName);
                        }
                        catch
                        {
                        }
                    }

                    MessageBox.Show("All images are saved.", "Waveface", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        #endregion
    }
}