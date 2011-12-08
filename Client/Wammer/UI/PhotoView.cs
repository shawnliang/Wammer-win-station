#region

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.Component;

#endregion

namespace Waveface
{
    public partial class PhotoView : Form
    {
        public PhotoView()
        {
            InitializeComponent();
        }

        public PhotoView(List<string> files, string fileName)
        {
            InitializeComponent();

            foreach (string _file in files)
            {
                imageListView.Items.Add(_file);
            }

            imageListView.View = Manina.Windows.Forms.View.Gallery;

            imageListView.SetRenderer(new MyImageListViewRenderer());

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                if (fileName == _item.FileName)
                {
                    _item.Selected = true;
                    return;
                }
            }
        }

        #region Save

        private void miSave_Click(object sender, System.EventArgs e)
        {
            SavePic();
        }

        private void miSaveAll_Click(object sender, System.EventArgs e)
        {
            SaveAllPics();
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            SavePic();
        }

        private void btnSaveAll_Click(object sender, System.EventArgs e)
        {
            SaveAllPics();
        }

        private void SavePic()
        {
            if (imageListView.SelectedItems.Count == 0)
            {
                return;
            }

            string _picFile = imageListView.SelectedItems[0].FileName;

            saveFileDialog.FileName = new FileInfo(_picFile).Name;
            DialogResult _dr = saveFileDialog.ShowDialog();

            if (_dr == DialogResult.OK)
            {
                try
                {
                    string _destFile = saveFileDialog.FileName;

                    File.Copy(_picFile, _destFile);

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

        }

        #endregion
    }
}