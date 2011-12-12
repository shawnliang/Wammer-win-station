
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component.IconHelper;

namespace Waveface.PostUI
{
    public partial class Document : UserControl
    {
        private ImageList m_smallImageList = new ImageList();
        private ImageList m_largeImageList = new ImageList();
        private IconListManager m_iconListManager;
        private string m_previewFile = string.Empty;

        private ProgressDialog m_progressDialog;

        public PostForm MyParent { get; set; }

        public Document()
        {
            InitializeComponent();

            InitIconListManager();
        }

        private void InitIconListManager()
        {
            m_smallImageList.ColorDepth = ColorDepth.Depth32Bit;
            m_largeImageList.ColorDepth = ColorDepth.Depth32Bit;

            m_smallImageList.ImageSize = new Size(16, 16);
            m_largeImageList.ImageSize = new Size(32, 32);

            m_iconListManager = new IconListManager(m_smallImageList, m_largeImageList);

            listViewFiles.SmallImageList = m_smallImageList;
            listViewFiles.LargeImageList = m_largeImageList;
        }

        private void addFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string _file = openFileDialog.FileName;

                addNewFileToListView(_file, true);
            }
        }

        private void addNewFileToListView(string file, bool refreshUI)
        {
            if (!checkIfFileAlreadyExist(file))
            {
                ListViewItem _item = new ListViewItem(new FileInfo(file).Name, m_iconListManager.AddFileIcon(file));
                _item.Tag = file;
                _item.Selected = true;
                _item.Focused = true;

                if (refreshUI)
                    resetListviewSelectedItems();

                listViewFiles.Items.Add(_item);

                if (refreshUI)
                    setPreviewAndFileInfoUI(file);
            }
        }

        private void selectLastestListViewItem()
        {
            if (listViewFiles.Items.Count > 0)
            {
                resetListviewSelectedItems();

                ListViewItem _itemNew0 = listViewFiles.Items[0];
                _itemNew0.Selected = true;
                _itemNew0.Focused = true;
                _itemNew0.EnsureVisible();

                setPreviewAndFileInfoUI(_itemNew0.Tag.ToString());
            }
        }

        private void removeFileButton_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                ListViewItem _item = listViewFiles.SelectedItems[0];

                string _msg = String.Format("Are you sure you want to remove the item '{0}'?", _item.Text);

                if (MessageBox.Show(this, _msg, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    listViewFiles.Items.Remove(_item);

                    resetUI(false);

                    // 移除之後若還有項目, 就選取
                    selectLastestListViewItem();
                }
            }
        }

        private void removeAllFilesButton_Click(object sender, EventArgs e)
        {
            /*
            if (listViewFiles.Items.Count > 0)
            {
                string _msg = String.Format("Are you sure you want to remove All item(s)?");

                if (MessageBox.Show(this, _msg, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    resetUI(true);
                }
            }
            */

            resetUI(true);

            MyParent.toPureText_Mode();
        }

        private void listViewFiles_Resize(object sender, EventArgs e)
        {
            listViewFiles.TileSize = new Size(listViewFiles.Width - 32, 48);
        }

        private void listViewFiles_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                string _file = listViewFiles.SelectedItems[0].Tag.ToString();

                setPreviewAndFileInfoUI(_file);
            }
        }

        #region Misc

        private void resetListviewSelectedItems()
        {
            foreach (ListViewItem _item in listViewFiles.Items)
            {
                _item.Selected = false;
                _item.Focused = false;
            }
        }

        private bool checkIfFileAlreadyExist(string filePath)
        {
            string _file;

            foreach (ListViewItem _item in listViewFiles.Items)
            {
                _file = _item.Tag.ToString();

                if (_file == filePath)
                    return true;
            }

            return false;
        }

        private void resetUI(bool all)
        {
            labelFSize.Text = "";
            labelFTime.Text = "";
            labeFilelSize.Text = "";
            labelFileTime.Text = "";

            if (all)
            {
                listViewFiles.Clear();
            }

            try
            {
                if (!isWinXP())
                {
                    previewHandlerHost.UnloadPreviewHandler();
                }
            }
            catch
            {
            }
        }

        public static bool isWinXP()
        {
            OperatingSystem _os = Environment.OSVersion;

            return (_os.Platform == PlatformID.Win32NT) && (_os.Version.Major == 5) && (_os.Version.Minor >= 1);
        }

        private void setFileInfo(string filePath)
        {
            labelFSize.Text = "Size:";
            labelFTime.Text = "Last accessed:";

            labeFilelSize.Text = getFileSizeString(filePath);
            labelFileTime.Text = new FileInfo(filePath).LastAccessTimeUtc.ToString();
        }

        private void setPreviewAndFileInfoUI(string filePath)
        {
            if (!isWinXP())
            {
                m_previewFile = filePath;
                previewHandlerHost.Open(m_previewFile);
            }

            setFileInfo(filePath);
        }

        private string getFileSizeString(string fileName)
        {
            string[] _sizes = { "B", "KB", "MB", "GB" };
            double _len = new FileInfo(fileName).Length;
            int _order = 0;

            while (_len >= 1024 && _order + 1 < _sizes.Length)
            {
                _order++;
                _len = _len / 1024;
            }

            return String.Format("{0:0.##} {1}", _len, _sizes[_order]);
        }

        #endregion

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (listViewFiles.Items.Count > 0)
            {
                Dictionary<string, string> _uploadFiles = new Dictionary<string, string>();

                foreach (ListViewItem _item in listViewFiles.Items)
                {
                    _uploadFiles.Add(_item.Tag.ToString(), new FileInfo(_item.Tag.ToString()).Name); // Path, FileName
                }

                resetUI(true);

                m_progressDialog = new ProgressDialog("Upload Document ..."
                         , delegate
                               {
                                   string _ids = "[";

                                   int i = 0;

                                   foreach (string _path in _uploadFiles.Keys)
                                   {
                                       if (m_progressDialog.WasCancelled)
                                           return "*ERROR*";

                                       try
                                       {
                                           MR_attachments_upload _uf = Main.Current.RT.REST.File_UploadFile(_uploadFiles[_path], _path, "", false);

                                           if (_uf == null)
                                           {
                                               return "*ERROR*";
                                           }

                                           _ids += "\"" + _uf.object_id + "\"" + ",";
                                       }
                                       catch
                                       {
                                           return "*ERROR*";
                                       }

                                       i++;

                                       m_progressDialog.RaiseUpdateProgress(i * 100 / _uploadFiles.Keys.Count);
                                   }

                                   _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
                                   _ids += "]";

                                   return (_ids);
                               }); // This value will be passed to the method

                // Then all you need to do is 
                m_progressDialog.ShowDialog();

                if (!m_progressDialog.WasCancelled && ((string)m_progressDialog.Result) != "*ERROR*")
                {
                    try
                    {
                        if (DoRealPost((string)m_progressDialog.Result))
                        {
                            MyParent.SetDialogResult_Yes_AndClose();
                        }

                        return;
                    }
                    catch (Exception _e)
                    {
                        MessageBox.Show(_e.Message);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Action canceled");

                    //Restore File List
                    foreach (string _filePath in _uploadFiles.Keys)
                    {
                        addNewFileToListView(_filePath, false);
                    }

                    selectLastestListViewItem();
                }

                return;
            }

            //單純文字
            if (MyParent.richTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("Text cannot be empty!");
            }
            else
            {
                if (DoRealPost(""))
                {
                    MyParent.SetDialogResult_Yes_AndClose();
                }
            }
        }

        private bool DoRealPost(string files)
        {
            string _type = (files != "") ? "doc" : "text";

            try
            {
                MR_posts_new _np = Main.Current.RT.REST.Posts_New(MyParent.richTextBox.Text, files, "", _type);

                if (_np == null)
                {
                    MessageBox.Show("Post Error!");
                    return false;
                }

                MessageBox.Show("Post success!");
                return true;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);
                return false;
            }
        }

        public void ResizeUI()
        {
            previewHandlerHost.Invalidate();
        }
    }
}
